using System.Security.Principal;

namespace QuickBootWindows.Core
{
    public static class PrivilegeManager
    {
        public static bool IsAdmin()
        {
            if (!OperatingSystem.IsWindows()) return false; // Dev safety

            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}