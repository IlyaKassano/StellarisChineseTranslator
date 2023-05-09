using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChineseTranslater.Extensions
{
    public static class RegexExtensions
    {
        public async static Task<string> ReplaceAsync(this Regex regex, string input, Func<Match, Task<string>> replacementFn)
        {
            var replacements = new Dictionary<Match, string>();

            foreach (var match in regex.Matches(input).Cast<Match>())
            {
                var replacement = await replacementFn(match);
                replacements.Add(match, replacement);
            }

            var sb = new StringBuilder();
            var lastIndex = 0;

            foreach (var repl in replacements.OrderBy(r => r.Key.Index))
            {
                sb.Append(input, lastIndex, repl.Key.Index - lastIndex);
                sb.Append(repl.Value);

                lastIndex = repl.Key.Index + repl.Key.Length;
            }

            sb.Append(input, lastIndex, input.Length - lastIndex);
            return sb.ToString();
        }
    }
}
