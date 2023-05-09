using ChineseTranslater.Extensions;
using GTranslate.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseTranslater.FileTranslaters
{
    internal abstract class FileTranslater
    {
        protected ITranslator Translator { get; set; }
        protected string Path { get; set; }
        protected Dictionary<string, string> Cache { get; set; } = new Dictionary<string, string>();

        internal FileTranslater(ITranslator translater, string filePath)
        {
            Translator = translater;
            Path = filePath;
        }

        internal abstract Task<string> TranslateFile();
        protected virtual async Task<string> Translate(string str)
        {
            if (Cache.TryGetValue(str, out var result))
            {
                Console.WriteLine($"Translate from cache: {str}\t->\t{result}");
                return result;
            }

            var translation = await Translator.TranslateAsync(str, "en", "zh-CN");
            var translated = translation.Translation.FirstCharToUpper();
            Cache.Add(str, translated);
            Console.WriteLine($"Translate: {str}\t->\t{translated}");

            return translated;
        }
        internal abstract void Save(string filePath, string content);
    }
}
