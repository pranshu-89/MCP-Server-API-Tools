using System.ComponentModel;
using MCPServer.Services;
using MCPServer.Tools;
using ModelContextProtocol.Server;
using MCPServer.Models;


var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ItsmConfig>(builder.Configuration.GetSection(ItsmConfig.SectionName));
// Configure MCP Server with HTTP transport for SSE
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<ItsmTools>(); // Add ITSM tools only

// Add HTTP client for backend communication
builder.Services.AddHttpClient();

// Add authentication service
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Add ITSM services
builder.Services.AddScoped<ServiceRequestService>();
builder.Services.AddScoped<IssueTicketService>();

var app = builder.Build();
await app.RunAsync();

[McpServerToolType]
public class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"Hello from C#: {message}";

    [McpServerTool, Description("Echoes in reverse the message sent by the client.")]
    public static string ReverseEcho(string message) => new string(message.Reverse().ToArray());
}