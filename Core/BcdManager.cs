using System.Diagnostics;
using System.Text.RegularExpressions;

namespace QuickBootWindows.Core
{
    public class BcdManager
    {
        // SECURITY: Use absolute path to prevent malware from hijacking "bcdedit" command
        private readonly string _bcdPath = Path.Combine(Environment.SystemDirectory, "bcdedit.exe");

        public string GetFirmwareList()
        {
            return RunCommand("/enum firmware");
        }

        public void SetNextBoot(string guid)
        {
            // VALIDATION: Never run a system command with unchecked user input
            if (string.IsNullOrWhiteSpace(guid) || !Regex.IsMatch(guid, @"^\{[a-fA-F0-9\-]+\}$"))
            {
                throw new ArgumentException($"Invalid GUID format: {guid}");
            }

            // /addfirst ensures it runs next
            RunCommand($"/set {{fwbootmgr}} bootsequence {guid} /addfirst");
        }

        private string RunCommand(string arguments)
        {
            if (!File.Exists(_bcdPath))
                throw new FileNotFoundException("Critical System Error: bcdedit.exe not found.");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = _bcdPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true, // ERROR HANDLING: Capture errors
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) throw new Exception("Failed to start bcdedit process.");

            process.WaitForExit();

            // ERROR HANDLING: Check exit code
            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                throw new Exception($"BCD Error: {error}");
            }

            return process.StandardOutput.ReadToEnd();
        }
    }
}