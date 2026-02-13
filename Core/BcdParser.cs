using System.Text.RegularExpressions;
using QuickBootWindows.Models;

namespace QuickBootWindows.Core
{
    public static class BcdParser
    {
        public static List<BootEntry> FindLinuxEntries(string bcdOutput, string[] keywords)
        {
            var results = new List<BootEntry>();
            var blocks = bcdOutput.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Regex to capture Identifier, Description, AND Path
            string pattern = @"identifier\s+({[a-zA-Z0-9\-]+})[\s\S]*?path\s+(.*?)(\r|\n)[\s\S]*?description\s+(.*)";

            foreach (var block in blocks)
            {
                if (block.Contains("Windows Boot Manager") || block.Contains("Microsoft")) continue;

                foreach (var keyword in keywords)
                {
                    if (block.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        var match = Regex.Match(block, pattern);
                        
                        // Note: If regex fails (some entries don't have paths), fallback to basic
                        if (match.Success)
                        {
                            var guid = match.Groups[1].Value;
                            var path = match.Groups[2].Value.Trim();
                            var desc = match.Groups[4].Value.Trim();

                            // Avoid duplicates
                            if (!results.Any(r => r.Guid == guid))
                            {
                                results.Add(new BootEntry(guid, desc, path));
                            }
                        }
                    }
                }
            }
            return results;
        }
    }
}