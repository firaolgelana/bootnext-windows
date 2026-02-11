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
    }
}