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
            var chineseCharsRegex = new Regex(@"[一-龥＂＃＄％＆＇（）＊＋，－／：；＜＝＞＠［＼］＾＿｀｛｜｝～｟｠｢｣､　、〃〈〉《》「」『』【】〔〕〖〗〘〙〚〛〜〝〞〟〰〾〿–—‘’‛“”„‟…‧﹏﹑﹔·！？｡。」﹂”』’》）］｝〕〗〙〛〉】]+");
            var noSpaceBehindRegex = new Regex(@"(?<![ ""])(§[a-z])(?!!)", RegexOptions.IgnoreCase);
            var noSpaceAfterRegex = new Regex(@"(§!)(?![ ""])");
            var chineseWords = chineseCharsRegex.Matches(content).Cast<Match>().Select(c => c.Value);

            content = noSpaceBehindRegex.Replace(content, " $1");
            content = noSpaceAfterRegex.Replace(content, "$1 ");
            var newContent = await chineseCharsRegex.ReplaceAsync(content, ReplaceChinese);

            return newContent;
        }

        async Task<string> ReplaceChinese(Match r)
        {
            string value = r.Value;
            if (string.IsNullOrEmpty(value.Trim()))
                return r.Value;
            if (r.Value.StartsWith("\""))
                value = r.Value.Substring(1);

            string translation = await Translate(r.Value);

            if (r.Value.StartsWith("\""))
                return "\"" + translation;
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
