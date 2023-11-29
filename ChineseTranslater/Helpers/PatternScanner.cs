using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChineseTranslater.Helpers
{
    internal static class PatternScanner
    {
        readonly static string _anyChineseChar = /*lang=regex*/@"[一-龥＂＃＄％＆＇（）＊＋，－／：；＜＝＞＠［＼］＾＿｀｛｜｝～｟｠｢｣､　、〃〈〉《》「」『』【】〔〕〖〗〘〙〚〛〜〝〞〟〰〾〿–—‘’‛“”„‟…‧﹏﹑﹔·！？｡。」﹂”』’》）］｝〕〗〙〛〉】\u0011]";

        internal static Regex GetPatternChineseWords()
        {
            return new Regex($@"{_anyChineseChar}+", RegexOptions.Compiled);
        }

        internal static Regex GetPatternChineseWordsAsSetValue()
        {
            return new Regex($@"(?<==\s*"")(?={_anyChineseChar}+).+(?="")", RegexOptions.Compiled);
        }

        internal static Regex GetPatternChineseWordsAsArrayValue()
        {
            return new Regex($@"(?<=^[ \t]*|{{\s*)(?={_anyChineseChar.Insert(1, "\\-")}+)[^}}#]+", RegexOptions.Multiline | RegexOptions.Compiled);
        }

        internal static Regex GetPatternChineseWordsAsSetValueWithoutQuotes()
        {
            return new Regex($@"(?<==\s*)(?={_anyChineseChar}+).+", RegexOptions.Compiled);
        }

        internal static string AddSpaceBeforeColorOperator(string content)
        {
            var noSpaceBehindRegex = new Regex(@"(?<![ ""])(§[a-z])(?!!)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return noSpaceBehindRegex.Replace(content, " $1");
        }

        internal static string AddSpaceAfterColorOperator(string content)
        {
            var noSpaceAfterRegex = new Regex(@"(§!)(?![ ""])", RegexOptions.Compiled);
            return noSpaceAfterRegex.Replace(content, "$1 ");
        }
    }
}
