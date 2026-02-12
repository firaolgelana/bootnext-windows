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
            RunApp();
        }
        catch (Exception ex)
        {
            ConsoleUI.PrintError($"Fatal Crash: {ex.Message}");
            // LOGGING (Simple implementation)
            File.AppendAllText("quickboot_error.log", $"{DateTime.Now}: {ex}\n");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    static void RunApp()
    {
        ConsoleUI.PrintBanner(AppConfig.AppName, AppConfig.Version);

        // 2. AUTO-ELEVATION (UX Improvement)
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
            }
            
            return; // Close this non-admin instance
        }

        // --- Standard Logic Below ---
        Console.WriteLine("🔍 Scanning boot entries...");
        var manager = new BcdManager();
        string output = manager.GetFirmwareList();

        BootEntry? target = BcdParser.FindLinuxEntry(output, AppConfig.TargetKeywords);

        if (target != null)
        {
            ConsoleUI.PrintSuccess(target.Description, target.Guid);

            // --- NEW BITLOCKER CHECK START ---
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
                    return; // Exit for safety
                }
            }
            // --- NEW BITLOCKER CHECK END ---

            if (ConsoleUI.AskConfirmation(target.Description))
            {
                Console.WriteLine("⚙️  Setting boot sequence...");
                manager.SetNextBoot(target.Guid);

                if (needsSuspension)
                {
                    Console.WriteLine("🔓 Suspending BitLocker for one reboot...");
                    BitLockerManager.SuspendForReboot();
                }
                
                Console.WriteLine("👋 Rebooting...");
                new RebootManager().RebootNow();
            }
        }
        else
        {
            ConsoleUI.PrintError("Could not find any Linux/Ubuntu entry in BCD.");
            Console.ReadKey();
        }
    }
}