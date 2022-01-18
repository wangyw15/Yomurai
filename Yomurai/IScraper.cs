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
    /// <summary>
    /// 获取目录页Url
    /// </summary>
    /// <param name="document">小说详情页</param>
    /// <returns>目录页Url</returns>
    public abstract Url GetTocPageUrl(IDocument document);
    /// <summary>
    /// 获取章节目录及对应的Url
    /// </summary>
    /// <param name="document">小说目录页</param>
    /// <returns>Key为章节名称 Value为对应的Url</returns>
    public abstract KeyValuePair<string, Url>[] GetTableOfContent(IDocument document);
    /// <summary>
    /// 获取章节页面内容
    /// </summary>
    /// <param name="document">小说章节内容页</param>
    /// <returns>章节内容与下一页的Url 空则已经是最后一页</returns>
    public abstract (Novel.Paragraph[], Url) GetParagraphs(IDocument document);
    /// <summary>
    /// 获取小说标题
    /// </summary>
    /// <param name="document">小说详情页</param>
    /// <returns>标题</returns>
    public abstract string GetTitle(IDocument document);
    /// <summary>
    /// 获取小说作者
    /// </summary>
    /// <param name="document">小说详情页</param>
    /// <returns>作者</returns>
    public abstract string GetAuthor(IDocument document);
    /// <summary>
    /// 获取小说标签
    /// </summary>
    /// <param name="document">小说详情页</param>
    /// <returns>标签</returns>
    public abstract string[] GetTags(IDocument document);
    /// <summary>
    /// 获取小说简介
    /// </summary>
    /// <param name="document">小说详情页</param>
    /// <returns>简介</returns>
    public abstract string GetIntroduction(IDocument document);
    /// <summary>
    /// 获取小说封面图Url
    /// </summary>
    /// <param name="document">小说详情页</param>
    /// <returns>封面图Url</returns>
    public abstract Url GetCoverUrl(IDocument document);
    
}