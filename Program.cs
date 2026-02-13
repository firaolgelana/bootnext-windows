using System.Diagnostics;
using QuickBootWindows.Config;
using QuickBootWindows.Core;
using QuickBootWindows.UI;
using QuickBootWindows.Models;

class Program
{
    static void Main(string[] args)
    {
        // 1. GLOBAL ERROR HANDLING
        try
        {
            // Force UTF-8 to fix emoji display (✅, 🚀, etc.)
            Console.OutputEncoding = System.Text.Encoding.UTF8; 
            RunApp();
        }
        catch (Exception ex)
        {
            ConsoleUI.PrintError($"Fatal Crash: {ex.Message}");
            
            // Simple file logging
            try 
            {
                File.AppendAllText("quickboot_error.log", $"{DateTime.Now}: {ex}\n");
            }
            catch { /* Ignore logging errors if disk is full/locked */ }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    static void RunApp()
    {
        ConsoleUI.PrintBanner(AppConfig.AppName, AppConfig.Version);

        // 2. AUTO-ELEVATION (Admin Check)
        if (OperatingSystem.IsWindows() && !PrivilegeManager.IsAdmin())
        {
            Console.WriteLine("🔐 Requesting Administrator privileges...");
            
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = Environment.ProcessPath, // Relaunch itself
                UseShellExecute = true,
                Verb = "runas" // Triggers UAC Prompt
            };

            try 
            {
                Process.Start(psi);
            }
            catch 
            {
                ConsoleUI.PrintError("User declined Administrator privileges.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            
            return; // Close this non-admin instance
        }

        // --- Standard Logic Below ---
        Console.WriteLine("🔍 Scanning boot entries...");
        var manager = new BcdManager();
        string output = manager.GetFirmwareList();

        // 3. MULTIPLE OS DETECTION
        // Note: Make sure BcdParser.cs has been updated to return List<BootEntry>
        List<BootEntry> foundEntries = BcdParser.FindLinuxEntries(output, AppConfig.TargetKeywords);
        BootEntry? target = null;

        if (foundEntries.Count == 0)
        {
            ConsoleUI.PrintError("No Linux boot entries found.");
            Console.WriteLine("Checked keywords: " + string.Join(", ", AppConfig.TargetKeywords));
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }
        else if (foundEntries.Count == 1)
        {
            // Only one found, auto-select it
            target = foundEntries[0];
        }
        else
        {
            // Multiple found (e.g., Ubuntu + Kali), show menu
            // Note: Make sure ConsoleUI.cs has SelectFromMenu() implemented
            target = ConsoleUI.SelectFromMenu(foundEntries);
        }

        // If user cancelled the menu or selection failed
        if (target == null) return;

        // 4. BITLOCKER & REBOOT LOGIC
        ConsoleUI.PrintSuccess(target.Description, target.Guid);

        // Check BitLocker Status
        bool needsSuspension = false;
        
        if (BitLockerManager.IsBitLockerEnabled())
        {
            // Ask user if they want to suspend protection
            if (ConsoleUI.AskBitLockerSuspension())
            {
                needsSuspension = true;
            }
            else
            {
                ConsoleUI.PrintError("Aborted. Rebooting without suspending BitLocker may lock you out.");
                Console.ReadKey();
                return; // Exit for safety
            }
        }

        // Final Confirmation
        if (ConsoleUI.AskConfirmation(target.Description))
        {
            Console.WriteLine("⚙️  Setting boot sequence...");
            
            // Set the UEFI variable
            manager.SetNextBoot(target.Guid);

            // Handle BitLocker Suspension if needed
            if (needsSuspension)
            {
                Console.WriteLine("🔓 Suspending BitLocker for one reboot...");
                BitLockerManager.SuspendForReboot();
            }
            
            // Reboot
            Console.WriteLine("👋 Rebooting...");
            
            // Log success before dying
            try { File.AppendAllText("quickboot.log", $"{DateTime.Now}: Rebooting to {target.Description}\n"); } catch {}

            new RebootManager().RebootNow();
        }
    }
}