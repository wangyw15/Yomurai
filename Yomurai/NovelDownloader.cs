using System.ComponentModel;

namespace Yomurai;

public class NovelDownloader
{
    public delegate void ProgressChanged(double value, double maximum);
    public event ProgressChanged UpdateProgress;

    public void DownloadNovel(Url url)
    {
        UpdateProgress(-1, 0);
        var scraper = (from x in Shared.Scrapers where new Url(x.Host).Host == url.Host select x).First();
        
        var novel = new Novel() {Url = url.Href};
        var introDoc = WebUtils.GetDocumentFromUrl(url);
        novel.Info = new Novel.NovelInfo()
        {
            Title = scraper.GetTitle(introDoc),
            Author = scraper.GetAuthor(introDoc),
            CoverUrl = scraper.GetCoverUrl(introDoc).Href,
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
        Utils.WriteToJson(novel.Info,Path.Combine(basePath, "metadata.json"));
        
        var tocDoc = WebUtils.GetDocumentFromUrl(scraper.GetTocPageUrl(introDoc));
        var toc = scraper.GetTableOfContent(tocDoc);
        Utils.WriteToJson(toc, Path.Combine(basePath, "toc.json"));
        
        var numberedToc = new Dictionary<int, KeyValuePair<string, Url>>();
        var index = 0;
        foreach (var pair in toc)
        {
            if (pair.Value != null)
            {
                numberedToc.Add(index++, pair);
            }
        }
        
        var sections = new Dictionary<int, Novel.Section>();
        Parallel.ForEach(numberedToc, pair =>
        {
            try
            {
                var paras = new List<Novel.Paragraph>();
                var result = scraper.GetParagraphs(WebUtils.GetDocumentFromUrl(pair.Value.Value));
                while (result.Item2 != null)
                {
                    paras.AddRange(result.Item1);
                    result = scraper.GetParagraphs(WebUtils.GetDocumentFromUrl(result.Item2));
                }
                lock (sections)
                {
                    sections.Add(pair.Key, new Novel.Section() {Title = pair.Value.Key, Paragraphs = paras.ToArray()});
                    UpdateProgress(sections.Count, numberedToc.Count);
                }
                Utils.WriteToJson(paras, Path.Combine(basePath, "sections", pair.Value.Key + ".json"));
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
    }
}