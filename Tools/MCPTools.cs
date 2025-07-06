using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using MCPServer.Services;
using MCPServer.Utils;

namespace MCPServer.Tools;

[McpServerToolType]
public sealed class ItsmTools
{
    private readonly ServiceRequestService serviceRequestService;
    private readonly IssueTicketService issueTicketService;

    public ItsmTools(ServiceRequestService serviceRequestService, IssueTicketService issueTicketService)
    {
        this.serviceRequestService = serviceRequestService;
        this.issueTicketService = issueTicketService;
    }

    #region Service Request Tools

    [McpServerTool, Description("Get all service requests. Returns a complete list of service requests in the system.")]
    public async Task<string> GetAllServiceRequests()
    {
        try
        {
            var result = await serviceRequestService.GetAllServiceRequests();
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get filtered service requests based on user context - My Tickets, History, or Assigned to Me.")]
    public async Task<string> GetServiceRequestTickets(
        [Description("Filter for tickets requested by the current user")] bool isMyTickets = false,
        [Description("Filter for historical/closed tickets")] bool isHistory = false,
        [Description("Filter for tickets assigned to the current user")] bool isAssignedToMe = false)
    {
        try
        {
            var result = await serviceRequestService.GetServiceRequestTickets(isMyTickets, isHistory, isAssignedToMe);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get a specific service request by its ID.")]
    public async Task<string> GetServiceRequestById([Description("The ID of the service request to retrieve")] int id)
    {
        try
        {
            var result = await serviceRequestService.GetServiceRequestById(id);
            if (result == null)
            {
                return JsonSerializer.Serialize(new { error = "Service request not found" }, JsonOptions.Default);
            }
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get all service requests for a specific asset.")]
    public async Task<string> GetServiceRequestsByAsset([Description("The ID of the asset")] int assetId)
    {
        try
        {
            var result = await serviceRequestService.GetServiceRequestsByAsset(assetId);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Create a new service request.")]
    public async Task<string> CreateServiceRequest(
        [Description("Title/name of the service request")] string customServiceName,
        [Description("Business justification for the request")] string businessJustification,
        [Description("Email of the person requesting the service")] string requestedFor,
        [Description("Country where the request is made")] string country,
        [Description("Service catalogue ID (optional)")] int? serviceCatalogueId = null,
        [Description("Category ID (optional)")] int? categoryId = null,
        [Description("Sub-category ID (optional)")] int? subCategoryId = null,
        [Description("Asset ID (optional)")] int? assetId = null,
        [Description("Cost center (optional)")] string? costCenter = null,
        [Description("User severity level (optional)")] string? userSeverityLevel = null)
    {
        try
        {
            var request = new CreateServiceRequestRequest
            {
                CustomServiceName = customServiceName,
                BusinessJustification = businessJustification,
                RequestedFor = requestedFor,
                Country = country,
                ServiceCatalogueId = serviceCatalogueId,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                AssetId = assetId,
                CostCenter = costCenter,
                UserSeverityLevel = userSeverityLevel
            };

            var result = await serviceRequestService.CreateServiceRequest(request);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Update an existing service request.")]
    public async Task<string> UpdateServiceRequest(
        [Description("ID of the service request to update")] int id,
        [Description("Title/name of the service request")] string customServiceName,
        [Description("Business justification for the request")] string businessJustification,
        [Description("Email of the person requesting the service")] string requestedFor,
        [Description("Country where the request is made")] string country,
        [Description("Service catalogue ID (optional)")] int? serviceCatalogueId = null,
        [Description("Category ID (optional)")] int? categoryId = null,
        [Description("Sub-category ID (optional)")] int? subCategoryId = null,
        [Description("Asset ID (optional)")] int? assetId = null,
        [Description("Cost center (optional)")] string? costCenter = null,
        [Description("User severity level (optional)")] string? userSeverityLevel = null,
        [Description("Status (optional)")] string? status = null,
        [Description("Priority (optional)")] string? priority = null,
        [Description("Assigned to (optional)")] string? assignedTo = null,
        [Description("Escalated to (optional)")] string? escalatedTo = null)
    {
        try
        {
            var request = new UpdateServiceRequestRequest
            {
                Id = id,
                CustomServiceName = customServiceName,
                BusinessJustification = businessJustification,
                RequestedFor = requestedFor,
                Country = country,
                ServiceCatalogueId = serviceCatalogueId,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                AssetId = assetId,
                CostCenter = costCenter,
                UserSeverityLevel = userSeverityLevel,
                Status = status,
                Priority = priority,
                AssignedTo = assignedTo,
                EscalatedTo = escalatedTo
            };

            var result = await serviceRequestService.UpdateServiceRequest(id, request);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get the total count of service requests.")]
    public async Task<string> GetServiceRequestCount()
    {
        try
        {
            var count = await serviceRequestService.GetServiceRequestCount();
            return JsonSerializer.Serialize(new { count }, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    #endregion

    #region Issue Ticket Tools

    [McpServerTool, Description("Get all issue tickets. Returns a complete list of issue tickets in the system.")]
    public async Task<string> GetAllIssueTickets()
    {
        try
        {
            var result = await issueTicketService.GetAllIssueTickets();
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get filtered issue tickets based on user context - My Tickets, History, or Assigned to Me.")]
    public async Task<string> GetIssueTicketTickets(
        [Description("Filter for tickets requested by the current user")] bool isMyTickets = false,
        [Description("Filter for historical/closed tickets")] bool isHistory = false,
        [Description("Filter for tickets assigned to the current user")] bool isAssignedToMe = false)
    {
        try
        {
            var result = await issueTicketService.GetIssueTicketTickets(isMyTickets, isHistory, isAssignedToMe);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get a specific issue ticket by its ID.")]
    public async Task<string> GetIssueTicketById([Description("The ID of the issue ticket to retrieve")] int id)
    {
        try
        {
            var result = await issueTicketService.GetIssueTicketById(id);
            if (result == null)
            {
                return JsonSerializer.Serialize(new { error = "Issue ticket not found" }, JsonOptions.Default);
            }
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get all issue tickets for a specific asset.")]
    public async Task<string> GetIssueTicketsByAsset([Description("The ID of the asset")] int assetId)
    {
        try
        {
            var result = await issueTicketService.GetIssueTicketsByAsset(assetId);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Create a new issue ticket.")]
    public async Task<string> CreateIssueTicket(
        [Description("Title of the issue ticket")] string issueTitle,
        [Description("Detailed description of the issue")] string issueDescription,
        [Description("Email of the person reporting the issue")] string requestedFor,
        [Description("Country where the issue is reported")] string country,
        [Description("Issue catalogue ID (optional)")] int? issueCatalogueId = null,
        [Description("Category ID (optional)")] int? categoryId = null,
        [Description("Sub-category ID (optional)")] int? subCategoryId = null,
        [Description("Asset ID (optional)")] int? assetId = null,
        [Description("Cost center (optional)")] string? costCenter = null,
        [Description("User severity level (optional)")] string? userSeverityLevel = null)
    {
        try
        {
            var request = new CreateIssueTicketRequest
            {
                IssueTitle = issueTitle,
                IssueDescription = issueDescription,
                RequestedFor = requestedFor,
                Country = country,
                IssueCatalogueId = issueCatalogueId,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                AssetId = assetId,
                CostCenter = costCenter,
                UserSeverityLevel = userSeverityLevel
            };

            var result = await issueTicketService.CreateIssueTicket(request);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Update an existing issue ticket.")]
    public async Task<string> UpdateIssueTicket(
        [Description("ID of the issue ticket to update")] int id,
        [Description("Title of the issue ticket")] string issueTitle,
        [Description("Detailed description of the issue")] string issueDescription,
        [Description("Email of the person reporting the issue")] string requestedFor,
        [Description("Country where the issue is reported")] string country,
        [Description("Issue catalogue ID (optional)")] int? issueCatalogueId = null,
        [Description("Category ID (optional)")] int? categoryId = null,
        [Description("Sub-category ID (optional)")] int? subCategoryId = null,
        [Description("Asset ID (optional)")] int? assetId = null,
        [Description("Cost center (optional)")] string? costCenter = null,
        [Description("User severity level (optional)")] string? userSeverityLevel = null,
        [Description("Status (optional)")] string? status = null,
        [Description("Priority (optional)")] string? priority = null,
        [Description("Assigned to (optional)")] string? assignedTo = null,
        [Description("Escalated to (optional)")] string? escalatedTo = null)
    {
        try
        {
            var request = new UpdateIssueTicketRequest
            {
                Id = id,
                IssueTitle = issueTitle,
                IssueDescription = issueDescription,
                RequestedFor = requestedFor,
                Country = country,
                IssueCatalogueId = issueCatalogueId,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                AssetId = assetId,
                CostCenter = costCenter,
                UserSeverityLevel = userSeverityLevel,
                Status = status,
                Priority = priority,
                AssignedTo = assignedTo,
                EscalatedTo = escalatedTo
            };

            var result = await issueTicketService.UpdateIssueTicket(id, request);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get the total count of issue tickets.")]
    public async Task<string> GetIssueTicketCount()
    {
        try
        {
            var count = await issueTicketService.GetIssueTicketCount();
            return JsonSerializer.Serialize(new { count }, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    #endregion

    #region Combined Tools

    [McpServerTool, Description("Get dashboard summary with counts of both service requests and issue tickets.")]
    public async Task<string> GetDashboardSummary()
    {
        try
        {
            var serviceRequestCount = await serviceRequestService.GetServiceRequestCount();
            var issueTicketCount = await issueTicketService.GetIssueTicketCount();
            
            var summary = new
            {
                ServiceRequestCount = serviceRequestCount,
                IssueTicketCount = issueTicketCount,
                TotalTickets = serviceRequestCount + issueTicketCount,
                GeneratedAt = DateTime.UtcNow
            };

            return JsonSerializer.Serialize(summary, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    #endregion

    #region Enhanced Search and Management Tools

    [McpServerTool, Description("Search service requests by title, description, or any text content. Provides comprehensive search across all service request fields.")]
    public async Task<string> SearchServiceRequests(
        [Description("Search query to find in service request titles, descriptions, or content")] string searchQuery,
        [Description("Filter by status (optional)")] string? status = null,
        [Description("Filter by priority (optional)")] string? priority = null,
        [Description("Filter by assigned user (optional)")] string? assignedTo = null)
    {
        try
        {
            var result = await serviceRequestService.SearchServiceRequests(searchQuery, status, priority, assignedTo);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Search issue tickets by title, description, or any text content. Provides comprehensive search across all issue ticket fields.")]
    public async Task<string> SearchIssueTickets(
        [Description("Search query to find in issue ticket titles, descriptions, or content")] string searchQuery,
        [Description("Filter by status (optional)")] string? status = null,
        [Description("Filter by priority (optional)")] string? priority = null,
        [Description("Filter by assigned user (optional)")] string? assignedTo = null)
    {
        try
        {
            var result = await issueTicketService.SearchIssueTickets(searchQuery, status, priority, assignedTo);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get service requests by status. Helps to filter and manage tickets by their current state.")]
    public async Task<string> GetServiceRequestsByStatus(
        [Description("Status to filter by (e.g., 'Open', 'In Progress', 'Closed', 'Pending')")] string status)
    {
        try
        {
            var result = await serviceRequestService.GetServiceRequestsByStatus(status);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get issue tickets by status. Helps to filter and manage tickets by their current state.")]
    public async Task<string> GetIssueTicketsByStatus(
        [Description("Status to filter by (e.g., 'Open', 'In Progress', 'Closed', 'Pending')")] string status)
    {
        try
        {
            var result = await issueTicketService.GetIssueTicketsByStatus(status);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get service requests by priority level. Helps prioritize and manage critical requests.")]
    public async Task<string> GetServiceRequestsByPriority(
        [Description("Priority level to filter by (e.g., 'Low', 'Medium', 'High', 'Critical')")] string priority)
    {
        try
        {
            var result = await serviceRequestService.GetServiceRequestsByPriority(priority);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Get issue tickets by priority level. Helps prioritize and manage critical issues.")]
    public async Task<string> GetIssueTicketsByPriority(
        [Description("Priority level to filter by (e.g., 'Low', 'Medium', 'High', 'Critical')")] string priority)
    {
        try
        {
            var result = await issueTicketService.GetIssueTicketsByPriority(priority);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Close a service request by marking it as resolved. Requires providing resolution details.")]
    public async Task<string> CloseServiceRequest(
        [Description("ID of the service request to close")] int id,
        [Description("Resolution details explaining how the request was resolved")] string resolution,
        [Description("Optional closure notes")] string? closureNotes = null)
    {
        try
        {
            var result = await serviceRequestService.CloseServiceRequest(id, resolution, closureNotes);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Close an issue ticket by marking it as resolved. Requires providing resolution details.")]
    public async Task<string> CloseIssueTicket(
        [Description("ID of the issue ticket to close")] int id,
        [Description("Resolution details explaining how the issue was resolved")] string resolution,
        [Description("Optional closure notes")] string? closureNotes = null)
    {
        try
        {
            var result = await issueTicketService.CloseIssueTicket(id, resolution, closureNotes);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Assign a service request to a specific user or team. Helps with workload distribution.")]
    public async Task<string> AssignServiceRequest(
        [Description("ID of the service request to assign")] int id,
        [Description("User or team to assign the request to")] string assignedTo,
        [Description("Optional assignment notes")] string? assignmentNotes = null)
    {
        try
        {
            var result = await serviceRequestService.AssignServiceRequest(id, assignedTo, assignmentNotes);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Assign an issue ticket to a specific user or team. Helps with workload distribution.")]
    public async Task<string> AssignIssueTicket(
        [Description("ID of the issue ticket to assign")] int id,
        [Description("User or team to assign the ticket to")] string assignedTo,
        [Description("Optional assignment notes")] string? assignmentNotes = null)
    {
        try
        {
            var result = await issueTicketService.AssignIssueTicket(id, assignedTo, assignmentNotes);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Update the priority of a service request. Helps escalate or de-escalate requests based on business needs.")]
    public async Task<string> UpdateServiceRequestPriority(
        [Description("ID of the service request to update")] int id,
        [Description("New priority level (e.g., 'Low', 'Medium', 'High', 'Critical')")] string priority,
        [Description("Reason for priority change")] string reason)
    {
        try
        {
            var result = await serviceRequestService.UpdateServiceRequestPriority(id, priority, reason);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    [McpServerTool, Description("Update the priority of an issue ticket. Helps escalate or de-escalate issues based on business impact.")]
    public async Task<string> UpdateIssueTicketPriority(
        [Description("ID of the issue ticket to update")] int id,
        [Description("New priority level (e.g., 'Low', 'Medium', 'High', 'Critical')")] string priority,
        [Description("Reason for priority change")] string reason)
    {
        try
        {
            var result = await issueTicketService.UpdateIssueTicketPriority(id, priority, reason);
            return JsonSerializer.Serialize(result, JsonOptions.Default);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions.Default);
        }
    }

    #endregion
}
