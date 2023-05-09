using ChineseTranslater.FileTranslaters;
using GTranslate.Translators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChineseTranslater
{
    internal class Program
    {
        readonly static MicrosoftTranslator _translater = new MicrosoftTranslator();
        readonly static Dictionary<FileTranslaters, Func<ITranslator, string, FileTranslater>> _fileTranslaters = 
            new Dictionary<FileTranslaters, Func<ITranslator, string, FileTranslater>>
            {
                { FileTranslaters.YAML, (translater, filePath) => new YamlTranslator(translater, filePath) },
                { FileTranslaters.AMPL, (translater, filePath) => new AmplTranslator(translater, filePath) },
            };

        static Program()
        {
            Console.OutputEncoding = Encoding.Unicode;
        }

        static void Main(string[] args)
        {
            string dir = args[0];
            if (string.IsNullOrEmpty(dir))
                throw new ArgumentNullException(nameof(dir));

            var txt = Directory.EnumerateFiles(dir, $"*.txt", SearchOption.AllDirectories);
            var yml = Directory.EnumerateFiles(dir, $"*.yml", SearchOption.AllDirectories);
            var tasks = new List<Task>();
            foreach (var filePath in txt.Concat(yml))
            {
                tasks.Add(Task.Run(async () => await TranslateFile(filePath)));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Successfully translated");
        }

        async static Task TranslateFile(string filePath)
        {
            var content = File.ReadAllText(filePath);
            FileTranslater fileTranslater = GetFileTranslater(content, filePath);

            string newContent = await fileTranslater.TranslateFile();

            if (content != newContent)
                fileTranslater.Save(filePath, newContent);
        }

        static FileTranslater GetFileTranslater(string content, string filePath)
        {
            if (content.TrimStart().StartsWith("l_simp_chinese"))
                return _fileTranslaters[FileTranslaters.YAML](_translater, filePath);

            return _fileTranslaters[FileTranslaters.AMPL](_translater, filePath);
        }

        internal enum FileTranslaters
        {
            YAML,
            AMPL,
        }
    }
}
