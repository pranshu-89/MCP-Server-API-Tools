namespace MCPServer.Models;

public class ItsmConfig
{
    public const string SectionName = "ITSM";
    
    public string BaseUrl { get; set; } = string.Empty;
    public string BearerToken { get; set; } = string.Empty;
}
