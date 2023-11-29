using ChineseTranslater.Extensions;
using GTranslate.Results;
using GTranslate.Translators;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChineseTranslater.FileTranslaters
{
    internal abstract class FileTranslater
    {
        protected ITranslator Translator { get; set; }
        protected string Path { get; set; }
        protected ConcurrentDictionary<string, string> Cache { get; set; } = new();

        internal FileTranslater(ITranslator translater, string filePath)
        {
            Translator = translater;
            Path = filePath;
        }

        internal abstract Task<string> TranslateFile();
        protected virtual async Task<string> Translate(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (Cache.TryGetValue(str, out var result))
            {
                Console.WriteLine($"Translate from cache: {str}\t->\t{result}");
                return result;
            }

            var builder = new StringBuilder();
            int step = 50_000;
            for (int i = 0; i < str.Length; i += step)
            {
                int availableLength = str.Length - i;
                string chunk = str.Substring(i, availableLength >= step ? step : availableLength);

                ITranslationResult translation;
                try
                {
                    translation = await Translator.TranslateAsync(chunk + "\n", "en", "zh-CN"); // TODO Fix 429
                }
                catch (Exception ex)
                {
                    i -= step;
                    continue;
                }

                builder.Append(translation.Translation.FirstCharToUpper());
            }
            var translated = builder.ToString();
            Cache.TryAdd(str, translated);
            Console.WriteLine($"Translate: {str}\t->\t{translated}");

            return translated.Trim('\n').Trim();
        }
        internal abstract void Save(string filePath, string content);
    }
}
