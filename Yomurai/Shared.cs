namespace Yomurai;

public static class Shared
{
    public const string FAILED = "{FAILED}";
    public const string EMPTY_CONTENT = "{EMPTY_CONTENT}";
    public static BaseScraper[] Scrapers { get; }

    static Shared()
    {
        Scrapers = Utils.LoadScrapers();
    }
}