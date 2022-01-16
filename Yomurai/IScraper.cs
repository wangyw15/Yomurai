namespace Yomurai;

public class Novel
{
    public enum ParagraphType
    {
        Text,
        Image
    }
    
    public class Section
    {
        public string Title { get; set; }
        public Paragraph[] Paragraphs { get; set; }
    }

    public class Paragraph
    {
        public ParagraphType Type { get; set; }
        public string Content { get; set; }
    }
    
    public record struct NovelInfo(string Title, string Author, string[] Tags, string Introduction, string CoverUrl);
    public NovelInfo Info { get; set; }
    public Section[] Sections { get; set; }
}


public abstract class BaseScraper
{
    public abstract string ScraperName { get; set; }
    public abstract string Host { get; set; }
    protected IDocument GetDocumentFromUrl(string url) => Utils.GetDocumentFromUrl(JoinUrl(url));

    protected string JoinUrl(string path, string protocal = "https") => Utils.JoinUrl(Host, path, protocal);
    
    public abstract string GetTocPageUrl(IDocument document);
    public abstract KeyValuePair<string, string>[] GetTableOfContent(IDocument document);
    public abstract Novel.Paragraph[] GetSection(string url);
    public abstract string GetTitle(IDocument document);
    public abstract string GetAuthor(IDocument document);
    public abstract string[] GetTags(IDocument document);
    public abstract string GetIntroduction(IDocument document);
    public abstract string GetCoverUrl(IDocument document);
    //public abstract Novel.NovelInfo GetInfo(IDocument document);
}