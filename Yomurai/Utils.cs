using System.Reflection;

namespace Yomurai;

public static class Utils
{
    private static BrowsingContext _context = new BrowsingContext(Configuration.Default.WithDefaultLoader());
    
    public static void WriteNovel(Novel novel)
    {
        var writer = new StreamWriter("out.md");
        writer.WriteLine($"# {novel.Info.Title}\n");
        writer.WriteLine($" > 标签：{string.Join(", ", novel.Info.Tags)}");
        writer.WriteLine(" >");
        writer.WriteLine($" > 作者：{novel.Info.Author}\n");
        writer.WriteLine($"![封面图片]({novel.Info.CoverUrl})\n");
        writer.WriteLine($"{novel.Info.Introduction}\n");
        writer.WriteLine("---\n");
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
            writer.WriteLine("---\n");
        }

        writer.Close();
    }

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
                Console.WriteLine(current.ScraperName);
            }
        }
        return ret.ToArray();
    }
    
    public static IDocument GetDocumentFromUrl(string url)
    {
        var ret = _context.OpenAsync(url).Result;
        return ret;
    }

    public static string JoinUrl(string host, string path, string protocal = "https")
    {
        if (path.StartsWith("http"))
        {
            return path;
        }
        return protocal + "://" + Path.Join(host, path).Replace("\\", "/");
    }
}