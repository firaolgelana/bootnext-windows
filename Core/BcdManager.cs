using System.Diagnostics;

namespace QuickBootWindows.Core
{
    public class BcdManager
    {
        public string GetFirmwareList()
        {
            return RunCommand("/enum firmware");
        }

        public void SetNextBoot(string guid)
        {
            // /addfirst ensures it runs next
            RunCommand($"/set {{fwbootmgr}} bootsequence {guid} /addfirst");
        }

        private string RunCommand(string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "bcdedit",
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