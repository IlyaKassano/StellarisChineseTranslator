using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

        internal async override Task<string> TranslateFile()
        {
            Console.WriteLine("Translating AMPL strings: " + Path);
            if (Path.Contains("00_random"))
                ;

            var content = File.ReadAllText(Path);
            content = AddQuotes(content);

            Regex setValueRegex = PatternScanner.GetPatternChineseWordsAsSetValue();
            Regex arrayValueRegex = PatternScanner.GetPatternChineseWordsAsArrayValue();

            string newContent = await MultiThreading.TranslateByRegex(content, setValueRegex, Translate);
            newContent = await MultiThreading.TranslateByRegex(content, arrayValueRegex, (input) => Translate(AddQuotesToArrayValues(input)));

            return newContent;
        }

        string AddQuotes(string content)
        {
            Regex noQuotesRegex = PatternScanner.GetPatternChineseWordsAsSetValueWithoutQuotes();
            return noQuotesRegex.Replace(content, (r) =>
                {
                    string newValue = $@"""{r.Value}""";
                    Console.WriteLine("Adding quotes to: " + r.Value);
                    return newValue;
                });
        }

        static int i = 0;
        string AddQuotesToArrayValues(string value)
        {
            return "\"" + value.Trim()
                .Replace("\r", "")
                .Replace("\n\n", "\n")
                .Replace("\n", "\"\n")
                .Replace("\" ", " ")
                .Replace(" \"", " ")
                .Replace(" ", "\" \"")
                .Replace("\t", "\t\"") + "\"";
        }

        internal override void Save(string filePath, string content)
        {
            Console.WriteLine("Saving AMPL: " + filePath);
            var encoding = new UTF8Encoding(false);
            File.WriteAllText(filePath, content, encoding);
        }
    }
}
