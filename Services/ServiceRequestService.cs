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

public class ServiceRequestService
{
    private readonly HttpClient httpClient;
    private readonly ItsmConfig itsmConfig;

    public ServiceRequestService(IHttpClientFactory httpClientFactory, IOptions<ItsmConfig> options, IAuthenticationService authenticationService)
    {
        httpClient = httpClientFactory.CreateClient();
        itsmConfig = options.Value;
        
        httpClient.BaseAddress = new Uri(itsmConfig.BaseUrl);
        
        // Configure authentication
        authenticationService.ConfigureHttpClient(httpClient);
    }

    /// <summary>
    /// Gets all service requests without pagination
    /// </summary>
    public async Task<List<ServiceRequest>> GetAllServiceRequests()
    {
        var response = await httpClient.GetAsync($"/api/ServiceRequest/GetAll");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ServiceRequest>>(content, JsonOptions.Default);
            return result ?? new List<ServiceRequest>();
        }

        throw new HttpRequestException($"Failed to get service requests: {response.StatusCode}");
    }

    /// <summary>
    /// Gets service requests based on filters (My Tickets, History, Assigned to Me)
    /// </summary>
    public async Task<List<ServiceRequest>> GetServiceRequestTickets(bool isMyTickets = false, bool isHistory = false, bool isAssignedToMe = false)
    {
        var url = $"/api/ServiceRequest/GetTickets?isMyTickets={isMyTickets}&isHistory={isHistory}&isAssignedToMe={isAssignedToMe}";
        var response = await httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ServiceRequest>>(content, JsonOptions.Default);
            return result ?? new List<ServiceRequest>();
        }

        throw new HttpRequestException($"Failed to get service request tickets: {response.StatusCode}");
    }

    /// <summary>
    /// Gets a specific service request by ID
    /// </summary>
    public async Task<ServiceRequest?> GetServiceRequestById(int id)
    {
        var response = await httpClient.GetAsync($"/api/ServiceRequest/{id}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ServiceRequest>(content, JsonOptions.Default);
            return result;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new HttpRequestException($"Failed to get service request: {response.StatusCode}");
    }

    /// <summary>
    /// Gets service requests for a specific asset
    /// </summary>
    public async Task<List<ServiceRequest>> GetServiceRequestsByAsset(int assetId)
    {
        var response = await httpClient.GetAsync($"/api/ServiceRequest/GetServiceRequestByAsset/{assetId}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ServiceRequest>>(content, JsonOptions.Default);
            return result ?? new List<ServiceRequest>();
        }

        throw new HttpRequestException($"Failed to get service requests for asset: {response.StatusCode}");
    }

    /// <summary>
    /// Creates a new service request
    /// </summary>
    public async Task<ServiceRequest> CreateServiceRequest(CreateServiceRequestRequest request)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions.Default);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync("/api/ServiceRequest/CreateServiceRequest", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ServiceRequest>(responseContent, JsonOptions.Default);
            return result ?? throw new InvalidOperationException("Failed to deserialize created service request");
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Failed to create service request: {response.StatusCode} - {errorContent}");
    }

    /// <summary>
    /// Updates an existing service request
    /// </summary>
    public async Task<ServiceRequest> UpdateServiceRequest(int id, UpdateServiceRequestRequest request)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions.Default);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        
        var response = await httpClient.PutAsync($"/api/ServiceRequest/UpdateServiceRequest/{id}", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ServiceRequest>(responseContent, JsonOptions.Default);
            return result ?? throw new InvalidOperationException("Failed to deserialize updated service request");
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Failed to update service request: {response.StatusCode} - {errorContent}");
    }

    /// <summary>
    /// Gets count of service requests
    /// </summary>
    public async Task<int> GetServiceRequestCount()
    {
        var response = await httpClient.GetAsync("/api/ServiceRequest?count=true");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CountResponse>(content, JsonOptions.Default);
            return result?.Count ?? 0;
        }

        throw new HttpRequestException($"Failed to get service request count: {response.StatusCode}");
    }

    /// <summary>
    /// Search service requests by text content
    /// </summary>
    public async Task<List<ServiceRequest>> SearchServiceRequests(string searchQuery, string? status = null, string? priority = null, string? assignedTo = null)
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

        var url = $"/api/ServiceRequest/Search?{string.Join("&", queryParams)}";
        var response = await httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ServiceRequest>>(content, JsonOptions.Default);
            return result ?? new List<ServiceRequest>();
        }

        throw new HttpRequestException($"Failed to search service requests: {response.StatusCode}");
    }

    /// <summary>
    /// Get service requests by status
    /// </summary>
    public async Task<List<ServiceRequest>> GetServiceRequestsByStatus(string status)
    {
        var response = await httpClient.GetAsync($"/api/ServiceRequest/ByStatus?status={Uri.EscapeDataString(status)}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ServiceRequest>>(content, JsonOptions.Default);
            return result ?? new List<ServiceRequest>();
        }

        throw new HttpRequestException($"Failed to get service requests by status: {response.StatusCode}");
    }

    /// <summary>
    /// Get service requests by priority
    /// </summary>
    public async Task<List<ServiceRequest>> GetServiceRequestsByPriority(string priority)
    {
        var response = await httpClient.GetAsync($"/api/ServiceRequest/ByPriority?priority={Uri.EscapeDataString(priority)}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ServiceRequest>>(content, JsonOptions.Default);
            return result ?? new List<ServiceRequest>();
        }

        throw new HttpRequestException($"Failed to get service requests by priority: {response.StatusCode}");
    }

    /// <summary>
    /// Close a service request
    /// </summary>
    public async Task<ServiceRequest> CloseServiceRequest(int id, string resolution, string? closureNotes = null)
    {
        var request = new CloseServiceRequestRequest
        {
            Id = id,
            Resolution = resolution,
            ClosureNotes = closureNotes
        };

        var response = await httpClient.PostAsJsonAsync($"/api/ServiceRequest/{id}/Close", request);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ServiceRequest>(content, JsonOptions.Default);
            return result ?? throw new InvalidOperationException("Failed to deserialize closed service request");
        }

        throw new HttpRequestException($"Failed to close service request: {response.StatusCode}");
    }

    /// <summary>
    /// Assign a service request to a user
    /// </summary>
    public async Task<ServiceRequest> AssignServiceRequest(int id, string assignedTo, string? assignmentNotes = null)
    {
        var request = new AssignServiceRequestRequest
        {
            Id = id,
            AssignedTo = assignedTo,
            AssignmentNotes = assignmentNotes
        };

        var response = await httpClient.PostAsJsonAsync($"/api/ServiceRequest/{id}/Assign", request);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ServiceRequest>(content, JsonOptions.Default);
            return result ?? throw new InvalidOperationException("Failed to deserialize assigned service request");
        }

        throw new HttpRequestException($"Failed to assign service request: {response.StatusCode}");
    }

    /// <summary>
    /// Update service request priority
    /// </summary>
    public async Task<ServiceRequest> UpdateServiceRequestPriority(int id, string priority, string reason)
    {
        var request = new UpdateServiceRequestPriorityRequest
        {
            Id = id,
            Priority = priority,
            Reason = reason
        };

        var response = await httpClient.PostAsJsonAsync($"/api/ServiceRequest/{id}/UpdatePriority", request);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ServiceRequest>(content, JsonOptions.Default);
            return result ?? throw new InvalidOperationException("Failed to deserialize updated service request");
        }

        throw new HttpRequestException($"Failed to update service request priority: {response.StatusCode}");
    }
}

// Response models for API calls
public class PagedServiceRequestResponse
{
    public List<ServiceRequest> Items { get; set; } = new();
    public int TotalCount { get; set; }
}

public class CreateServiceRequestRequest
{
    public string CustomServiceName { get; set; } = "";
    public string BusinessJustification { get; set; } = "";
    public string RequestedFor { get; set; } = "";
    public string Country { get; set; } = "";
    public int? ServiceCatalogueId { get; set; }
    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public int? AssetId { get; set; }
    public string? CostCenter { get; set; }
    public string? UserSeverityLevel { get; set; }
}

public class UpdateServiceRequestRequest
{
    public int Id { get; set; }
    public string CustomServiceName { get; set; } = "";
    public string BusinessJustification { get; set; } = "";
    public string RequestedFor { get; set; } = "";
    public string Country { get; set; } = "";
    public int? ServiceCatalogueId { get; set; }
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

public class CloseServiceRequestRequest
{
    public int Id { get; set; }
    public string Resolution { get; set; } = "";
    public string? ClosureNotes { get; set; }
}

public class AssignServiceRequestRequest
{
    public int Id { get; set; }
    public string AssignedTo { get; set; } = "";
    public string? AssignmentNotes { get; set; }
}

public class UpdateServiceRequestPriorityRequest
{
    public int Id { get; set; }
    public string Priority { get; set; } = "";
    public string Reason { get; set; } = "";
}
