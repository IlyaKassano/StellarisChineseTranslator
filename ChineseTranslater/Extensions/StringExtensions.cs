﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseTranslater.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string FirstCharToUpper(this string input)
        {
            if (input?.Length < 1)
                return input;

            return input[0].ToString().ToUpper() + input.Substring(1);
        }
    }
}
