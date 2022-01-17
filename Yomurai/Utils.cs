﻿using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Terminal.Gui;

namespace Yomurai;

public static class Utils
{
    private static BrowsingContext _context = new BrowsingContext(Configuration.Default.WithDefaultLoader());
    
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

    public static void WriteToJson(object value, string fileName)
    {
        var writer = new StreamWriter(fileName);
        writer.WriteLine(JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        }));
        writer.Close();
    }

    public static void DownloadNovel(string url, ProgressBar pBar)
    {
        var uri = new Uri(url);
        var scraper = (from x in Shared.Scrapers where x.Host == uri.Host select x).First();
        
        var novel = new Novel() {Url = url};
        var introDoc = Utils.GetDocumentFromUrl(url);
        novel.Info = new Novel.NovelInfo()
        {
            Title = scraper.GetTitle(introDoc),
            Author = scraper.GetAuthor(introDoc),
            CoverUrl = scraper.GetCoverUrl(introDoc),
            Introduction = scraper.GetIntroduction(introDoc),
            Tags = scraper.GetTags(introDoc)
        };
        var basePath = "yomurai/novels/" + novel.Info.Title;
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }
        if (!Directory.Exists(Path.Combine(basePath, "sections")))
        {
            Directory.CreateDirectory(Path.Combine(basePath, "sections"));
        }
        WriteToJson(novel.Info,Path.Combine(basePath, "metadata.json"));
        
        var tocDoc = Utils.GetDocumentFromUrl(scraper.GetTocPageUrl(introDoc));
        var toc = scraper.GetTableOfContent(tocDoc);
        WriteToJson(toc, Path.Combine(basePath, "toc.json"));
        
        var numberedToc = new Dictionary<int, KeyValuePair<string, string>>();
        var index = 0;
        foreach (var pair in toc)
        {
            if (!pair.Value.StartsWith("javascript"))
            {
                numberedToc.Add(index++, pair);
            }
        }
        
        var sections = new Dictionary<int, Novel.Section>();
        Parallel.ForEach(numberedToc, pair =>
        {
            try
            {
                var paras = scraper.GetSection(pair.Value.Value);
                lock (sections)
                {
                    sections.Add(pair.Key, new Novel.Section() {Title = pair.Value.Key, Paragraphs = paras});
                    Application.MainLoop.Invoke(() =>
                    {
                        pBar.Fraction = sections.Count / numberedToc.Count;
                    });
                }
                WriteToJson(paras, Path.Combine(basePath, "sections", pair.Value.Key + ".json"));
            }
            catch
            {
                /*lock (sections)
                {
                    sections.Add(pair.Key,
                        new Novel.Section()
                        {
                            Title = pair.Value.Key,
                            Paragraphs = new[] {new Novel.Paragraph() {Type = Novel.ParagraphType.Text, Content = Shared.FAILED}}
                        });
                    Console.WriteLine($"{sections.Count} / {numberedToc.Count}");
                }*/
            }
        });
        novel.Sections = (from x in sections.OrderBy(p => p.Key) select x.Value).ToArray();
        //WriteToJson(novel, "a.json");
        //Utils.ExportNovel(novel);
    }

    public static void TestMain()
    {
        if (!Directory.Exists("yomurai"))
        {
            Directory.CreateDirectory("yomurai");
        }

        var url = "https://www.linovelib.com/novel/2507.html";
        //DownloadNovel(url);
    }
}