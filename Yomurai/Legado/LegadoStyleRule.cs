namespace Yomurai.Legado.Types;
public record struct BasicRule(string Type, string Name, string Position);

public class FullRuleGroup
{
    public enum GroupType
    {
        And,
        Or
    }

    public GroupType Type { get; set; } = GroupType.And;
    public BasicRule[][] Rules { get; set; }
    public string ReplaceFrom { get; set; }
    public string ReplaceTo { get; set; }
}
