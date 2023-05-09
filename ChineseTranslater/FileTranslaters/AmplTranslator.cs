using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChineseTranslater.Extensions;
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
            var regex = new Regex(@"(?<!#.*)(?<![\w\-])(?=[一-龥]+)[一-龥\d\-]+.*?(?![\w\-])|(?<=[A-Z])[一-龥]+(?=!)");
            var chineseWords = regex.Matches(content).Cast<Match>().Select(c => c.Value);

            var newContent = regex.ReplaceAsync(content, async (r) => await Translate(r.Value));

            return newContent;
        }

        internal override void Save(string filePath, string content)
        {
            Console.WriteLine("Saving AMPL: " + filePath);
            var encoding = new UTF8Encoding(false);
            File.WriteAllText(filePath, content, encoding);
        }
    }
}
