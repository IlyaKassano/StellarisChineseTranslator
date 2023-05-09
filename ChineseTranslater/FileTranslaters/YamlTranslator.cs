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
    internal class YamlTranslator : FileTranslater
    {
        public YamlTranslator(ITranslator translater, string filePath) : base(translater, filePath)
        {
        }

        internal async override Task<string> TranslateFile()
        {
            Console.WriteLine("Translating YAML strings: " + Path);

            var content = File.ReadAllText(Path);
            content = content.ReplaceFirst("l_simp_chinese", "l_english");
            //var regex = new Regex(@"(?<=\w+ *: *0? *)[^0 \s]""?[\w ""']+""?\b");
            var chineseCharsRegex = PatternScanner.GetPatternChineseWords(content);
            var newContent = await chineseCharsRegex.ReplaceAsync(content, ReplaceChinese);
            newContent = PatternScanner.AddSpaceBeforeColorOperator(newContent);
            newContent = PatternScanner.AddSpaceAfterColorOperator(newContent);

            return newContent;
        }

        async Task<string> ReplaceChinese(Match r)
        {
            string value = r.Value;
            if (string.IsNullOrEmpty(value.Trim()))
                return value;

            string translation = await Translate(value);

            return translation;
        }

        internal override void Save(string filePath, string content)
        {
            Console.WriteLine("Saving YAML: " + filePath);
            filePath = filePath.Replace("l_simp_chinese", "l_english");
            var encoding = new UTF8Encoding(true);
            File.WriteAllText(filePath, content, encoding);
        }
    }
}
