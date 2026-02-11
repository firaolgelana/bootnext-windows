using System.Diagnostics;

namespace QuickBootWindows.Core
{
    public class RebootManager
    {
        public void RebootNow()
        {
            // /r = reboot
            // /t 0 = time delay 0 seconds (immediate)
            // /f = force close running applications (optional, but recommended)
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/r /t 0",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(psi);
        }
    }
}