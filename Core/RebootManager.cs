using System.Diagnostics;

namespace QuickBootWindows.Core
{
    public class RebootManager
    {
        public void RebootNow()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "shutdown",
                // /r = reboot
                // /f = force close apps (prevents getting stuck)
                // /t 0 = immediate
                Arguments = "/r /f /t 0", 
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(psi);
        }
    }
}