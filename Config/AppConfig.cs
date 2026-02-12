namespace QuickBootWindows.Config
{
    public static class AppConfig
    {
        public const string AppName = "QuickBoot Windows";
        public const string Version = "1.0.0";
        
        // 1. SAFE LIST: Specific Distros + The Bootloader Name (Grub)
        public static readonly string[] TargetKeywords = 
        { 
            // Distro Names
            "Ubuntu", 
            "Kali",
            "Arch", 
            "Manjaro",
            "Fedora", 
            "Debian", 
            "Pop!_OS",
            "Mint",
            "Elementary",
            "OpenSUSE",
            
            // The Universal Linux Key
            "Grub", 
            "systemd-boot" // Used by Arch sometimes
        };
    }
}