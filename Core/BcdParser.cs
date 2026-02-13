using System.Text.RegularExpressions;
using QuickBootWindows.Models;

namespace QuickBootWindows.Core
{
    public static class BcdParser
    {
        // Change return type to List<BootEntry>
        public static List<BootEntry> FindLinuxEntries(string bcdOutput, string[] keywords)
        {
            var results = new List<BootEntry>();
            
            // Regex for Identifier and Description
            string pattern = @"identifier\s+({[a-zA-Z0-9\-]+})[\s\S]*?description\s+(.*)";
            
            var blocks = bcdOutput.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var block in blocks)
            {
                // SAFETY: Ignore Windows entries
                if (block.Contains("Windows Boot Manager") || block.Contains("Microsoft")) continue;

                foreach (var keyword in keywords)
                {
                    if (block.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        var match = Regex.Match(block, pattern);
                        if (match.Success)
                        {
                            var entry = new BootEntry(
                                Guid: match.Groups[1].Value,
                                Description: match.Groups[2].Value.Trim()
                            );

                            // Prevent duplicates (if multiple keywords match the same entry)
                            if (!results.Any(r => r.Guid == entry.Guid))
                            {
                                results.Add(entry);
                            }
                        }
                    }
                }
            }
            return results;
        }
    }
}