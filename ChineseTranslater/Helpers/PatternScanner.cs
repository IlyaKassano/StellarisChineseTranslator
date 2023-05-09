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
        readonly static string _anyChineseChar = "[一-龥＂＃＄％＆＇（）＊＋，－／：；＜＝＞＠［＼］＾＿｀｛｜｝～｟｠｢｣､　、〃〈〉《》「」『』【】〔〕〖〗〘〙〚〛〜〝〞〟〰〾〿–—‘’‛“”„‟…‧﹏﹑﹔·！？｡。」﹂”』’》）］｝〕〗〙〛〉】\u0011]";

        internal static Regex GetPatternChineseWords(string content)
        {
            return new Regex($@"{_anyChineseChar}+");
        }

        internal static Regex GetPatternChineseWordsAsSetValue(string content)
        {
            return new Regex($@"(?<==\s*"")(?={_anyChineseChar}+).+(?="")");
        }

        internal static Regex GetPatternChineseWordsAsSetValueWithoutQuotes(string content)
        {
            return new Regex($@"(?<==\s*)(?={_anyChineseChar}+).+");
        }

        internal static string AddSpaceBeforeColorOperator(string content)
        {
            var noSpaceBehindRegex = new Regex(@"(?<![ ""])(§[a-z])(?!!)", RegexOptions.IgnoreCase);
            return noSpaceBehindRegex.Replace(content, " $1");
        }

        internal static string AddSpaceAfterColorOperator(string content)
        {
            var noSpaceAfterRegex = new Regex(@"(§!)(?![ ""])");
            return noSpaceAfterRegex.Replace(content, "$1 ");
        }
    }
}
