using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using MCPServer.Models;


namespace MCPServer.Services;

public interface IAuthenticationService
{
    void ConfigureHttpClient(HttpClient httpClient);
    string? GetBearerToken();
}

public class AuthenticationService : IAuthenticationService
{
    private readonly ItsmConfig _itsmConfig;

    public AuthenticationService(IOptions<ItsmConfig> itsmConfig)
    {
        _itsmConfig = itsmConfig.Value;
    }

    public void ConfigureHttpClient(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _itsmConfig.BearerToken);
    }

    public string? GetBearerToken()
    {
        return _itsmConfig.BearerToken;
    }
}
