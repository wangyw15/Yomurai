namespace Yomurai.Scraper;

public class Linovelib : BaseScraper
{
    public override string ScraperName { get; set; } = "哩哔轻小说";
    public override string Host { get; set; } = "www.linovelib.com";

    public override string GetTocPageUrl(IDocument document)
    {
        return JoinUrl(document.QuerySelector(
                "body > div.wrap > div.book-html-box.clearfix > div:nth-child(1) > div.book-detail.clearfix > div.book-info > div.btn-group > a.btn.read-btn")
            .GetAttribute("href"));
    }

    public override KeyValuePair<string, string>[] GetTableOfContent(IDocument document)
    {
        var ret = new List<KeyValuePair<string, string>>();
        var toc = document.QuerySelector("body > div.wrap > div.container > div:nth-child(2) > div.volume-list > div > ul");
        var currentChapter = "";
        foreach (var element in toc.Children)
        {
            if (element.TagName.ToLower() == "div")
            {
                currentChapter = element.TextContent;
            }
            else
            {
                ret.Add(new KeyValuePair<string, string>(currentChapter + " " + element.TextContent, element.FirstElementChild.GetAttribute("href")));
            }
        }

        return ret.ToArray();
    }

    public override Novel.Paragraph[] GetSection(string url)
    {
        var ret = new List<Novel.Paragraph>();
        var currentPage = url;
        while (true)
        {
            var doc = GetDocumentFromUrl(currentPage);
            var contentElements = doc.QuerySelector("#TextContent").Children;
            foreach (var element in contentElements)
            {
                if (element.TagName.ToLower() == "p")
                {
                    ret.Add(new Novel.Paragraph(){Type = Novel.ParagraphType.Text, Content = element.TextContent.Trim()});
                }
                else if (element.TagName.ToLower() == "br")
                {
                    ret.Add(new Novel.Paragraph(){Type = Novel.ParagraphType.Text, Content = ""});
                }
                else if (element.TagName.ToLower() == "div")
                {
                    if (element.ClassList.Contains("divimage"))
                    {
                        ret.Add(new Novel.Paragraph(){Type = Novel.ParagraphType.Image, Content = element.FirstElementChild.GetAttribute("src")});
                    }
                }
            }
            
            var nextPage = doc.QuerySelector("#readbg > p > a:nth-child(5)");
            if (nextPage.TextContent == "下一页")
            {
                currentPage = nextPage.GetAttribute("href");
            }
            else
            {
                break;
            }
        }
        return ret.ToArray();
    }

    public override string GetTitle(IDocument document) => document
        .QuerySelector(
            "body > div.wrap > div.book-html-box.clearfix > div:nth-child(1) > div.book-detail.clearfix > div.book-info > h1")
        .TextContent;

    public override string GetAuthor(IDocument document) => document
        .QuerySelector(
            "body > div.wrap > div.book-html-box.clearfix > div.book-side.fr > div.book-author > div.au-name > a")
        .TextContent;

    public override string[] GetTags(IDocument document)
    {
        var ret = new List<string>();
        var tagContainer =
            document.QuerySelector(
                "body > div.wrap > div.book-html-box.clearfix > div:nth-child(1) > div.book-detail.clearfix > div.book-info > div.book-label");
        foreach (var element in tagContainer.GetElementsByTagName("a"))
        {
            ret.Add(element.TextContent);
        }

        return ret.ToArray();
    }

    public override string GetIntroduction(IDocument document) => document
        .QuerySelector(
            "body > div.wrap > div.book-html-box.clearfix > div:nth-child(1) > div.book-detail.clearfix > div.book-info > div.book-dec.Jbook-dec.hide > p")
        .TextContent;

    public override string GetCoverUrl(IDocument document) => document
        .QuerySelector(
            "body > div.wrap > div.book-html-box.clearfix > div:nth-child(1) > div.book-detail.clearfix > div.book-img.fl > img")
        .GetAttribute("src");
}