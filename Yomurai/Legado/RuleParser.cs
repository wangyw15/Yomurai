using Yomurai.Legado.Types;

namespace Yomurai.Legado.Parser.Rules;

public static class LegadoStyleParser
{
    public static FullRuleGroup Parse(string rule)
    {
        var ret = new FullRuleGroup(){Type = rule.Contains("||") ? FullRuleGroup.GroupType.Or : FullRuleGroup.GroupType.And};
        var rules = new List<BasicRule[]>();

        var ruleParts = rule.Split("##");
        if (ruleParts.Length == 1)
        {
            ret.ReplaceFrom = ret.ReplaceTo = string.Empty;
        }
        else if (ruleParts.Length == 2)
        {
            ret.ReplaceFrom = ruleParts[1];
            ret.ReplaceTo = string.Empty;
        }
        else if (ruleParts.Length == 3)
        {
            ret.ReplaceFrom = ruleParts[1];
            ret.ReplaceTo = ruleParts[2];
        }

        foreach (var basicRuleStr in ruleParts[0].Split(ret.Type == FullRuleGroup.GroupType.And ? "&&" : "||"))
        {
            var basicRules = new List<BasicRule>();
            foreach (var singleRuleStr in basicRuleStr.Split("@"))
            {
                var currentRule = new BasicRule();
                var basicRuleParts = singleRuleStr.Split(".");
                if (basicRuleParts.Length == 3)
                {
                    currentRule.Type = basicRuleParts[0];
                    currentRule.Name = basicRuleParts[1];
                    currentRule.Position = basicRuleParts[2];
                }
                else if (basicRuleParts.Length == 2)
                {
                    currentRule.Type = "{UNDEFINED}";
                    currentRule.Name = basicRuleParts[0];
                    currentRule.Position = basicRuleParts[1];
                }
                else if (basicRuleParts.Length == 1)
                {
                    currentRule.Type = "{UNDEFINED}";
                    currentRule.Name = basicRuleParts[0];
                    currentRule.Position = string.Empty;
                }
                basicRules.Add(currentRule);
            }
            rules.Add(basicRules.ToArray());
        }

        ret.Rules = rules.ToArray();
        return ret;
    }
}
