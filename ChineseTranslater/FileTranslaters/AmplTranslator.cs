using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChineseTranslater.Extensions;
using ChineseTranslater.Helpers;
using GTranslate.Translators;

namespace ChineseTranslater.FileTranslaters
{
    internal class AmplTranslator : FileTranslater
    {
        public AmplTranslator(ITranslator translater, string filePath) : base(translater, filePath)
        {
        }

        internal override Task<string> TranslateFile()
        {
            Console.WriteLine("Translating AMPL strings: " + Path);

            var content = File.ReadAllText(Path);
            content = AddQuotes(content);

            Regex regex = PatternScanner.GetPatternChineseWordsAsSetValue(content);
            var chineseWords = regex.Matches(content)
                .Cast<Match>()
                .Select(c => c.Value);

            var newContent = regex.ReplaceAsync(content, async (r) => await Translate(r.Value));

            return newContent;
        }

        string AddQuotes(string content)
        {
            Regex noQuotesRegex = PatternScanner.GetPatternChineseWordsAsSetValueWithoutQuotes(content);
            return noQuotesRegex.Replace(content, (r) =>
                {
                    string newValue = $@"""{r.Value}""";
                    Console.WriteLine("Adding quotes to: " + r.Value);
                    return newValue;
                });
        }

        internal override void Save(string filePath, string content)
        {
            Console.WriteLine("Saving AMPL: " + filePath);
            var encoding = new UTF8Encoding(false);
            File.WriteAllText(filePath, content, encoding);
        }
    }
}
