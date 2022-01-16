using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Yomurai;
using Yomurai.Scraper;

/*var url = "https://www.linovelib.com/novel/2507.html";
Novel novel = null;

foreach (var i in scrapers)
{
    if (new Uri(url).Host == i.Host)
    {
        Console.WriteLine(i.ScraperName);
        novel = i.GetNovel(url);
    }
}

var sw = new StreamWriter("a.json");
sw.WriteLine(JsonSerializer.Serialize(novel));
sw.Close();
NovelHelper.WriteNovel(novel);*/

var url = "https://www.linovelib.com/novel/2507.html";
var a = new Linovelib();
var novel = new Novel();
var introDoc = Utils.GetDocumentFromUrl(url);
novel.Info = new Novel.NovelInfo()
{
    Title = a.GetTitle(introDoc),
    Author = a.GetAuthor(introDoc),
    CoverUrl = a.GetCoverUrl(introDoc),
    Introduction = a.GetIntroduction(introDoc),
    Tags = a.GetTags(introDoc)
};
var tocDoc = Utils.GetDocumentFromUrl(a.GetTocPageUrl(introDoc));
var toc = a.GetTableOfContent(tocDoc);
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
    var paras = a.GetSection(pair.Value.Value);
    lock (sections)
    {
        sections.Add(pair.Key, new Novel.Section() {Title = pair.Value.Key, Paragraphs = paras});
        Console.WriteLine($"{sections.Count} / {numberedToc.Count}");
    }
});

var writer = new StreamWriter("a.json");
writer.WriteLine(JsonSerializer.Serialize(from x in sections.OrderBy(p => p.Key) select x.Value, new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
}));
writer.Close();