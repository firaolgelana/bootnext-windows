using System.Text.RegularExpressions;
using QuickBootWindows.Models;

namespace QuickBootWindows.Core
{
    public static class BcdParser
    {
        public static BootEntry? FindLinuxEntry(string bcdOutput, string[] keywords)
        {
            // Regex to capture Identifier and Description
            // Looks for: identifier {GUID} ... description Something
            string pattern = @"identifier\s+({[a-zA-Z0-9\-]+})[\s\S]*?description\s+(.*)";
            
            // Split by double newline to get blocks
            var entries = bcdOutput.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var block in entries)
            {
                foreach (var keyword in keywords)
                {
                    if (block.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        var match = Regex.Match(block, pattern);
                        if (match.Success)
                        {
                            return new BootEntry(
                                Guid: match.Groups[1].Value,
                                Description: match.Groups[2].Value.Trim()
                            );
                        }
                    }
                }
            }
            return null;
        }
    }
}