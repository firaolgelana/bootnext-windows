using System.Diagnostics;

namespace QuickBootWindows.Core
{
    public static class BitLockerManager
    {
        // Check if C: drive is encrypted and protected
        public static bool IsBitLockerEnabled()
        {
            var output = RunManageBde("-status C:");
            
            // Look for "Protection On" in the output
            // Output format usually contains: "    Protection Status:    Protection On"
            return output.Contains("Protection On", StringComparison.OrdinalIgnoreCase);
        }

        // Suspend BitLocker for exactly ONE reboot
        // It will auto-enable after the user enters Linux or comes back to Windows
        public static void SuspendForReboot()
        {
            // -protectors -disable C: : Turns off protection
            // -rc 1                   : For "Reboot Count 1"
            RunManageBde("-protectors -disable C: -rc 1");
        }

        private static string RunManageBde(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(Environment.SystemDirectory, "manage-bde.exe"),
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process?.WaitForExit();
            return process?.StandardOutput.ReadToEnd() ?? string.Empty;
        }
    }
}