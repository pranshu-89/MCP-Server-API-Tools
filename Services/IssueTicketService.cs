using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using DomainModel.Models;
using MCPServer.Models;
using MCPServer.Utils;

namespace MCPServer.Services;

public class IssueTicketService
{
    private readonly HttpClient httpClient;
    private readonly ItsmConfig itsmConfig;

    public IssueTicketService(IHttpClientFactory httpClientFactory, IOptions<ItsmConfig> options, IAuthenticationService authenticationService)
    {
        httpClient = httpClientFactory.CreateClient();
        itsmConfig = options.Value;
        
        httpClient.BaseAddress = new Uri(itsmConfig.BaseUrl);
        
        // Configure authentication
        authenticationService.ConfigureHttpClient(httpClient);
    }

    /// <summary>
    /// Gets all issue tickets without pagination
    /// </summary>
    public async Task<List<IssueTicket>> GetAllIssueTickets()
    {
        var response = await httpClient.GetAsync($"/api/IssueTicket/GetAll");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<IssueTicket>>(content, JsonOptions.Default);
            return result ?? new List<IssueTicket>();
        }

        throw new HttpRequestException($"Failed to get issue tickets: {response.StatusCode}");
    }

    /// <summary>
    /// Gets issue tickets based on filters (My Tickets, History, Assigned to Me)
    /// </summary>
    public async Task<List<IssueTicket>> GetIssueTicketTickets(bool isMyTickets = false, bool isHistory = false, bool isAssignedToMe = false)
    {
        var url = $"/api/IssueTicket/GetTickets?isMyTickets={isMyTickets}&isHistory={isHistory}&isAssignedToMe={isAssignedToMe}";
        var response = await httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<IssueTicket>>(content, JsonOptions.Default);
            return result ?? new List<IssueTicket>();
        }

        throw new HttpRequestException($"Failed to get issue tickets: {response.StatusCode}");
    }

    /// <summary>
    /// Gets a specific issue ticket by ID
    /// </summary>
    public async Task<IssueTicket?> GetIssueTicketById(int id)
    {
        var response = await httpClient.GetAsync($"/api/IssueTicket/GetById/{id}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IssueTicket>(content, JsonOptions.Default);
            return result;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new HttpRequestException($"Failed to get issue ticket: {response.StatusCode}");
    }

    /// <summary>
    /// Gets issue tickets for a specific asset
    /// </summary>
    public async Task<List<IssueTicket>> GetIssueTicketsByAsset(int assetId)
    {
        var response = await httpClient.GetAsync($"/api/IssueTicket/GetIssueTicketByAsset/{assetId}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<IssueTicket>>(content, JsonOptions.Default);
            return result ?? new List<IssueTicket>();
        }

        throw new HttpRequestException($"Failed to get issue tickets for asset: {response.StatusCode}");
    }

    /// <summary>
    /// Creates a new issue ticket
    /// </summary>
    public async Task<IssueTicket> CreateIssueTicket(CreateIssueTicketRequest request)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions.Default);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync("/api/IssueTicket/CreateIssueTicket", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IssueTicket>(responseContent, JsonOptions.Default);
            return result ?? throw new InvalidOperationException("Failed to deserialize created issue ticket");
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Failed to create issue ticket: {response.StatusCode} - {errorContent}");
    }

    /// <summary>
    /// Updates an existing issue ticket
    /// </summary>
    public async Task<IssueTicket> UpdateIssueTicket(int id, UpdateIssueTicketRequest request)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions.Default);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        
        var response = await httpClient.PutAsync($"/api/IssueTicket/UpdateIssueTicket/{id}", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IssueTicket>(responseContent, JsonOptions.Default);
            return result ?? throw new InvalidOperationException("Failed to deserialize updated issue ticket");
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Failed to update issue ticket: {response.StatusCode} - {errorContent}");
    }

    /// <summary>
    /// Gets count of issue tickets
    /// </summary>
    public async Task<int> GetIssueTicketCount()
    {
        var response = await httpClient.GetAsync("/api/IssueTicket?count=true");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CountResponse>(content, JsonOptions.Default);
            return result?.Count ?? 0;
        }

        throw new HttpRequestException($"Failed to get issue ticket count: {response.StatusCode}");
    }

    /// <summary>
    /// Search issue tickets by text content
    /// </summary>
    public async Task<List<IssueTicket>> SearchIssueTickets(string searchQuery, string? status = null, string? priority = null, string? assignedTo = null)
    {
        var queryParams = new List<string>
        {
            $"searchQuery={Uri.EscapeDataString(searchQuery)}"
        };

        if (!string.IsNullOrEmpty(status))
            queryParams.Add($"status={Uri.EscapeDataString(status)}");
        if (!string.IsNullOrEmpty(priority))
            queryParams.Add($"priority={Uri.EscapeDataString(priority)}");
        if (!string.IsNullOrEmpty(assignedTo))
            queryParams.Add($"assignedTo={Uri.EscapeDataString(assignedTo)}");

        var url = $"/api/IssueTicket/Search?{string.Join("&", queryParams)}";
        var response = await httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<IssueTicket>>(content, JsonOptions.Default);
            return result ?? new List<IssueTicket>();
        }

        throw new HttpRequestException($"Failed to search issue tickets: {response.StatusCode}");
    }

    /// <summary>
    /// Get issue tickets by status
    /// </summary>
    public async Task<List<IssueTicket>> GetIssueTicketsByStatus(string status)
    {
        var response = await httpClient.GetAsync($"/api/IssueTicket/ByStatus?status={Uri.EscapeDataString(status)}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<IssueTicket>>(content, JsonOptions.Default);
            return result ?? new List<IssueTicket>();
        }

        throw new HttpRequestException($"Failed to get issue tickets by status: {response.StatusCode}");
    }

    /// <summary>
    /// Get issue tickets by priority
    /// </summary>
    public async Task<List<IssueTicket>> GetIssueTicketsByPriority(string priority)
    {
        var response = await httpClient.GetAsync($"/api/IssueTicket/ByPriority?priority={Uri.EscapeDataString(priority)}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<IssueTicket>>(content, JsonOptions.Default);
            return result ?? new List<IssueTicket>();
        }

        throw new HttpRequestException($"Failed to get issue tickets by priority: {response.StatusCode}");
    }

    /// <summary>
    /// Close an issue ticket
    /// </summary>
    public async Task<IssueTicket> CloseIssueTicket(int id, string resolution, string? closureNotes = null)
    {
        var request = new CloseIssueTicketRequest
        {
            Id = id,
            Resolution = resolution,
            ClosureNotes = closureNotes
        };

        var response = await httpClient.PostAsJsonAsync($"/api/IssueTicket/{id}/Close", request);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IssueTicket>(content, JsonOptions.Default);
            return result ?? throw new InvalidOperationException("Failed to deserialize closed issue ticket");
        }

        throw new HttpRequestException($"Failed to close issue ticket: {response.StatusCode}");
    }

    /// <summary>
    /// Assign an issue ticket to a user
    /// </summary>
    public async Task<IssueTicket> AssignIssueTicket(int id, string assignedTo, string? assignmentNotes = null)
    {
        var request = new AssignIssueTicketRequest
        {
            Id = id,
            AssignedTo = assignedTo,
            AssignmentNotes = assignmentNotes
        };

        var response = await httpClient.PostAsJsonAsync($"/api/IssueTicket/{id}/Assign", request);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IssueTicket>(content, JsonOptions.Default);
            return result ?? throw new InvalidOperationException("Failed to deserialize assigned issue ticket");
        }

        throw new HttpRequestException($"Failed to assign issue ticket: {response.StatusCode}");
    }

    /// <summary>
    /// Update issue ticket priority
    /// </summary>
    public async Task<IssueTicket> UpdateIssueTicketPriority(int id, string priority, string reason)
    {
        var request = new UpdateIssueTicketPriorityRequest
        {
            Id = id,
            Priority = priority,
            Reason = reason
        };

        var response = await httpClient.PostAsJsonAsync($"/api/IssueTicket/{id}/UpdatePriority", request);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IssueTicket>(content, JsonOptions.Default);
            return result ?? throw new InvalidOperationException("Failed to deserialize updated issue ticket");
        }

        throw new HttpRequestException($"Failed to update issue ticket priority: {response.StatusCode}");
    }
}

// Response models for API calls
public class PagedIssueTicketResponse
{
    public List<IssueTicket> Items { get; set; } = new();
    public int TotalCount { get; set; }
}

public class CreateIssueTicketRequest
{
    public string IssueTitle { get; set; } = "";
    public string IssueDescription { get; set; } = "";
    public string RequestedFor { get; set; } = "";
    public string Country { get; set; } = "";
    public int? IssueCatalogueId { get; set; }
    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public int? AssetId { get; set; }
    public string? CostCenter { get; set; }
    public string? UserSeverityLevel { get; set; }
}

public class UpdateIssueTicketRequest
{
    public int Id { get; set; }
    public string IssueTitle { get; set; } = "";
    public string IssueDescription { get; set; } = "";
    public string RequestedFor { get; set; } = "";
    public string Country { get; set; } = "";
    public int? IssueCatalogueId { get; set; }
    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public int? AssetId { get; set; }
    public string? CostCenter { get; set; }
    public string? UserSeverityLevel { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? AssignedTo { get; set; }
    public string? EscalatedTo { get; set; }
}

public class CloseIssueTicketRequest
{
    public int Id { get; set; }
    public string Resolution { get; set; } = "";
    public string? ClosureNotes { get; set; }
}

public class AssignIssueTicketRequest
{
    public int Id { get; set; }
    public string AssignedTo { get; set; } = "";
    public string? AssignmentNotes { get; set; }
}

public class UpdateIssueTicketPriorityRequest
{
    public int Id { get; set; }
    public string Priority { get; set; } = "";
    public string Reason { get; set; } = "";
}
