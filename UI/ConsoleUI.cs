namespace QuickBootWindows.UI
{
    public static class ConsoleUI
    {
        public static void PrintBanner(string name, string version)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"--- {name} v{version} ---");
            Console.ResetColor();
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error: {message}");
            Console.ResetColor();
        }

        public static void PrintSuccess(string description, string guid)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Found Target: {description}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"   (ID: {guid})");
            Console.ResetColor();
        }

        public static bool AskConfirmation(string targetName)
        {
            Console.Write($"\n🚀 Reboot to {targetName} now? (y/n): ");
            var key = Console.ReadKey();
            Console.WriteLine(); // New line
            return key.KeyChar == 'y' || key.KeyChar == 'Y';
        }
        public static bool AskBitLockerSuspension()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n🔒 BitLocker Encryption is detected!");
            Console.WriteLine("   Modifying boot settings might trigger a BitLocker Recovery Key prompt.");
            Console.ResetColor();
            
            Console.WriteLine("   We can temporarily suspend BitLocker for this ONE reboot.");
            Console.WriteLine("   (It will automatically turn back on afterwards).");
            
            Console.Write("\n   Suspend BitLocker and reboot safely? (y/n): ");
            var key = Console.ReadKey();
            Console.WriteLine();
            return key.KeyChar == 'y' || key.KeyChar == 'Y';
        }
        
    }
}