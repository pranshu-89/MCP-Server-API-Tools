using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCPServer.Utils;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}

public class CountResponse
{
    public int Count { get; set; }
}
