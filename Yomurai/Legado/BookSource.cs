using System.Text.Json;
using System.Text.Json.Nodes;

namespace Yomurai.Legado;

public static class BookSource
{
    private static JsonElement _json = JsonSerializer.Deserialize<JsonElement>(File.ReadAllText("yomurai/bookSource.json"));

    public static string[] GetSourceNames()
    {
        var ret = new List<string>();
        foreach (var sources in _json.EnumerateArray())
        {
            var name = (from x in sources.EnumerateObject() where x.Name == "bookSourceName" select x.Value.GetString()).First();
            ret.Add(name);
        }
        return ret.ToArray();
    }
}