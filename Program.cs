using System.Text.RegularExpressions;
using System.Xml.Linq;
using WordFrequency;

static void ExtractWords(string sourceDir, string outputFile)
{
    PorterStemmer stem = new PorterStemmer();
    var stopwords = File.ReadLines("stopwords.txt").ToHashSet(StringComparer.OrdinalIgnoreCase);
    var files = Directory.GetFiles(sourceDir, "*.XML", SearchOption.AllDirectories);
    using var output = new StreamWriter(outputFile);

    foreach (var file in files)
    {
        var doc = XDocument.Load(file);
        var content = string.Join(' ', doc.Descendants()
            .Where(e => e.Name.LocalName is "title" or "headline" or "text")
            .Select(e => e.Value));

        foreach (Match m in Regex.Matches(content, "[a-zA-Z]+"))
        {
            if (stopwords.Contains(m.Value)) continue;
            output.WriteLine(stem.StemWord(m.Value));
        }
        
        foreach (var cls in doc.Descendants("codes").Attributes("class"))
        {
            output.WriteLine(stem.StemWord(cls.Value));
        }
    }
    
    Console.WriteLine("Done");
}

ExtractWords("Reuters/Reuters_34/Training", "extracted_words.txt");
