namespace Yomurai;

/// <summary>
/// 小说类
/// </summary>
public class Novel
{
    /// <summary>
    /// 段落类型
    /// </summary>
    public enum ParagraphType
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text,
        /// <summary>
        /// 图片
        /// </summary>
        Image
    }
    
    /// <summary>
    /// 章节类
    /// </summary>
    /// <param name="Title">章节标题</param>
    /// <param name="Paragraphs">章节内段落</param>
    public record struct Section(string Title, Paragraph[] Paragraphs);
    
    /// <summary>
    /// 段落类
    /// </summary>
    /// <param name="Type">段落种类</param>
    /// <param name="Content">段落内容</param>
    public record struct Paragraph(ParagraphType Type, string Content);
    
    /// <summary>
    /// 指示小说基本信息
    /// </summary>
    /// <param name="Title">标题</param>
    /// <param name="Author">作者</param>
    /// <param name="Tags">标签</param>
    /// <param name="Introduction">简介</param>
    /// <param name="CoverUrl">封面图Url</param>
    public record struct NovelInfo(string Title, string Author, string[] Tags, string Introduction, string CoverUrl);
    
    /// <summary>
    /// 小说详细页地址
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// 小说信息
    /// </summary>
    public NovelInfo Info { get; set; }
    /// <summary>
    /// 小说的正文章节部分
    /// </summary>
    public Section[] Sections { get; set; }
}

/// <summary>
/// 爬取器规则基类
/// </summary>
public abstract class BaseScraper
{
    /// <summary>
    /// 爬取器名称
    /// </summary>
    public abstract string ScraperName { get; }
    
    /// <summary>
    /// 网站主页
    /// </summary>
    public abstract string Host { get; }
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
    
}