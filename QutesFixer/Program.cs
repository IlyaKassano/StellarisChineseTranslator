using System;
using System.IO;
using System.Text.RegularExpressions;

public struct Test : ITest
{
    public int Num;
    public void Change(int num)
    {
        Num = num;
    }
    public override string ToString()
    {
        return Num.ToString();
    }
}

public interface ITest
{
    void Change(int num);
}

public class Program
{
    public static void Main(string[] args)
    {
        var keys = new string[] {
            "name",
            "desc",
            "title",
            "fail_text",
            "response_text",
        };

        string dir = args[0];
        if (string.IsNullOrEmpty(dir))
            throw new ArgumentNullException(nameof(dir));

        foreach (var filePath in Directory.EnumerateFiles(dir, "*.txt", SearchOption.AllDirectories))
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var key in keys)
            {
                var regex = new Regex(key + /*lang=regex*/@"\s*=\s*(?!^"")(?<value>[^""{}\n]+)(?!"")");
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = regex.Replace(lines[i], key + " = \"$1\"");
                }
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}