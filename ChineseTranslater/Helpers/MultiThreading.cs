using ChineseTranslater.FileTranslaters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChineseTranslater.Helpers
{
    internal class MultiThreading
    {
        internal static async Task<string> TranslateByRegex(string content, Regex regex, Func<string, Task<string>> translateMethod)
        {
            var dict = new ConcurrentDictionary<MatchInfo, string>();
            var matches = regex.Matches(content);
            string newContent = content;
            var lastIndex = 0;
            var sb = new StringBuilder();

            await Parallel.ForEachAsync(matches.Cast<Match>(), async (m, ct) => 
                dict.TryAdd(new MatchInfo(m.Index, m.Value), await translateMethod(m.Value)));

            foreach (var item in dict.OrderBy(d => d.Key.Index))
            {
                sb.Append(content, lastIndex, item.Key.Index - lastIndex);
                sb.Append(item.Value);

                lastIndex = item.Key.Index + item.Key.Value.Length;
            }
            sb.Append(content, lastIndex, content.Length - lastIndex);

            return sb.ToString();
        }
    }
}
