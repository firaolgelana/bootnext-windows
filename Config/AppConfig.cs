namespace QuickBootWindows.Config
{
    public static class AppConfig
    {
        public const string AppName = "QuickBoot Windows";
        public const string Version = "1.0.0";
        
        // Keywords to identify Linux boot entries
        public static readonly string[] TargetKeywords = 
        { 
            "Ubuntu", "Linux", "Grub", "Fedora", "Debian", "Pop!_OS" 
        };
    }
}