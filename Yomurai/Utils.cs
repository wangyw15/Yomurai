using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Yomurai;

public static class WebUtils
{
    private static BrowsingContext _context = new BrowsingContext(Configuration.Default.WithDefaultLoader());

    public static IDocument GetDocumentFromUrl(Url url)
    {
        var ret = _context.OpenAsync(url).Result;
        return ret;
    }
}

public static class NovelUtils
{
    public static Novel.NovelInfo[] GetDownloadedNovelInfos()
    {
        var ret = new List<Novel.NovelInfo>();
        if (Directory.Exists("yomurai/novels"))
        {
            var dirs = Directory.GetDirectories("yomurai/novels");
            foreach (var dir in dirs)
            {
                var info = JsonSerializer.Deserialize<Novel.NovelInfo>(
                    File.ReadAllText(Path.Combine(dir, "metadata.json")));
                ret.Add(info);
            }
        }
        
        return ret.ToArray();
    }
    public static void ExportNovel(Novel novel, string fileName)
    {
        var writer = new StreamWriter(fileName);
        writer.WriteLine($"# {novel.Info.Title}\n");
        writer.WriteLine($" > 标签：{string.Join(", ", novel.Info.Tags)}");
        writer.WriteLine(" >");
        writer.WriteLine($" > 作者：{novel.Info.Author}\n");
        writer.WriteLine($"![封面图片]({novel.Info.CoverUrl})\n");
        writer.WriteLine($"{novel.Info.Introduction}\n");
        writer.WriteLine("---\n");
        writer.WriteLine("<span id=\"toc\"></span>");
        writer.WriteLine("[TOC]\n");
        writer.WriteLine("---\n");
        foreach (var section in novel.Sections)
        {
            writer.WriteLine($"## {section.Title}\n");
            foreach (var paragraph in section.Paragraphs)
            {
                if (paragraph.Type == Novel.ParagraphType.Image)
                {
                    writer.WriteLine($"![插图]({paragraph.Content})\n");
                }
                else if (paragraph.Type == Novel.ParagraphType.Text)
                {
                    writer.WriteLine($"{paragraph.Content}\n");
                }
            }
            writer.WriteLine("[回到目录](#toc)\n");
            writer.WriteLine("---\n");
        }

        writer.Close();
    }
}

public static class Utils
{
    public static BaseScraper[] LoadScrapers()
    {
        var ret = new List<BaseScraper>();
        var asm = Assembly.GetExecutingAssembly();
        foreach (var t in asm.GetTypes())
        {
            if (t.IsSubclassOf(typeof(BaseScraper)))
            {
                var current = asm.CreateInstance(t.FullName) as BaseScraper;
                ret.Add(current);
            }
        }
        return ret.ToArray();
    }

    public static void WriteToJson(object value, string fileName)
    {
        var writer = new StreamWriter(fileName);
        writer.WriteLine(JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        }));
        writer.Close();
    }

    public static void Init()
    {
        if (!Directory.Exists("yomurai"))
        {
            Directory.CreateDirectory("yomurai");
        }
        if (!Directory.Exists("yomurai/novels"))
        {
            Directory.CreateDirectory("yomurai/novels");
        }
        if (!Directory.Exists("yomurai/export"))
        {
            Directory.CreateDirectory("yomurai/export");
        }
    }
}