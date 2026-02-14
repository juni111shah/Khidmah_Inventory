using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.API.Models;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Automation.Base)]
[Authorize]
public class AutomationController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AutomationController(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet(ApiRoutes.Automation.Rules)]
    public async Task<IActionResult> GetRules()
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Unauthorized(ApiResponse<List<AutomationRuleDto>>.FailureResponse("Company context is required", 401));

        var items = await _context.AutomationRules
            .Where(x => x.CompanyId == companyId.Value && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AutomationRuleDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = $"Trigger: {x.Trigger}",
                IsActive = x.IsActive,
                Trigger = x.Trigger,
                Condition = TryParseJson(x.ConditionJson),
                Action = TryParseJson(x.ActionJson),
                Priority = 0,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<AutomationRuleDto>>.SuccessResponse(items));
    }

    [HttpGet(ApiRoutes.Automation.RuleById)]
    public async Task<IActionResult> GetRule(Guid id)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Unauthorized(ApiResponse<AutomationRuleDto>.FailureResponse("Company context is required", 401));

        var entity = await _context.AutomationRules
            .Where(x => x.CompanyId == companyId.Value && !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync();
        if (entity == null)
            return NotFound(ApiResponse<AutomationRuleDto>.FailureResponse("Rule not found", 404));

        var dto = new AutomationRuleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = $"Trigger: {entity.Trigger}",
            IsActive = entity.IsActive,
            Trigger = entity.Trigger,
            Condition = TryParseJson(entity.ConditionJson),
            Action = TryParseJson(entity.ActionJson),
            Priority = 0,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
        return Ok(ApiResponse<AutomationRuleDto>.SuccessResponse(dto));
    }

    [HttpPost(ApiRoutes.Automation.Rules)]
    public async Task<IActionResult> CreateRule([FromBody] SaveAutomationRuleRequest request)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Unauthorized(ApiResponse<AutomationRuleDto>.FailureResponse("Company context is required", 401));

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(ApiResponse<AutomationRuleDto>.FailureResponse("Rule name is required"));
        if (string.IsNullOrWhiteSpace(request.Trigger))
            return BadRequest(ApiResponse<AutomationRuleDto>.FailureResponse("Trigger is required"));
        if (request.Action == null)
            return BadRequest(ApiResponse<AutomationRuleDto>.FailureResponse("Action is required"));

        var entity = new AutomationRule(
            companyId.Value,
            request.Name.Trim(),
            request.Trigger.Trim(),
            request.Condition.HasValue ? request.Condition.Value.GetRawText() : null,
            request.Action.Value.GetRawText(),
            _currentUser.UserId);

        if (!request.IsActive)
            entity.SetActive(false, _currentUser.UserId);

        _context.AutomationRules.Add(entity);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<AutomationRuleDto>.SuccessResponse(new AutomationRuleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = request.Description,
            IsActive = entity.IsActive,
            Trigger = entity.Trigger,
            Condition = request.Condition,
            Action = request.Action,
            Priority = request.Priority,
            CreatedAt = entity.CreatedAt
        }));
    }

    [HttpPut(ApiRoutes.Automation.RuleById)]
    public async Task<IActionResult> UpdateRule(Guid id, [FromBody] SaveAutomationRuleRequest request)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Unauthorized(ApiResponse<AutomationRuleDto>.FailureResponse("Company context is required", 401));

        var entity = await _context.AutomationRules
            .Where(x => x.CompanyId == companyId.Value && !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync();
        if (entity == null)
            return NotFound(ApiResponse<AutomationRuleDto>.FailureResponse("Rule not found", 404));
        if (!request.Action.HasValue)
            return BadRequest(ApiResponse<AutomationRuleDto>.FailureResponse("Action is required for update"));

        entity.Update(
            request.Name?.Trim() ?? entity.Name,
            request.Condition.HasValue ? request.Condition.Value.GetRawText() : null,
            request.Action.Value.GetRawText(),
            request.IsActive,
            _currentUser.UserId);

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<AutomationRuleDto>.SuccessResponse(new AutomationRuleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = request.Description,
            IsActive = entity.IsActive,
            Trigger = entity.Trigger,
            Condition = request.Condition,
            Action = request.Action,
            Priority = request.Priority,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        }));
    }

    [HttpPost(ApiRoutes.Automation.RuleToggle)]
    public async Task<IActionResult> ToggleRule(Guid id, [FromQuery] bool isActive)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Unauthorized(ApiResponse.FailureResponse("Company context is required", 401));

        var entity = await _context.AutomationRules
            .Where(x => x.CompanyId == companyId.Value && !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync();
        if (entity == null)
            return NotFound(ApiResponse.FailureResponse("Rule not found", 404));

        entity.SetActive(isActive, _currentUser.UserId);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.SuccessResponse("Rule status updated"));
    }

    [HttpDelete(ApiRoutes.Automation.RuleById)]
    public async Task<IActionResult> DeleteRule(Guid id)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Unauthorized(ApiResponse.FailureResponse("Company context is required", 401));

        var entity = await _context.AutomationRules
            .Where(x => x.CompanyId == companyId.Value && !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync();
        if (entity == null)
            return NotFound(ApiResponse.FailureResponse("Rule not found", 404));

        entity.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.SuccessResponse("Rule deleted"));
    }

    [HttpGet(ApiRoutes.Automation.Executions)]
    public async Task<IActionResult> GetExecutions([FromQuery] Guid? ruleId = null, [FromQuery] int top = 50)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Unauthorized(ApiResponse<List<AutomationExecutionDto>>.FailureResponse("Company context is required", 401));

        var query = _context.AutomationRuleHistories
            .Include(x => x.AutomationRule)
            .Where(x => x.CompanyId == companyId.Value && !x.IsDeleted);

        if (ruleId.HasValue)
            query = query.Where(x => x.AutomationRuleId == ruleId.Value);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Take(Math.Clamp(top, 1, 200))
            .Select(x => new AutomationExecutionDto
            {
                Id = x.Id,
                RuleId = x.AutomationRuleId,
                RuleName = x.AutomationRule != null ? x.AutomationRule.Name : "Unknown Rule",
                TriggeredAt = x.CreatedAt,
                Status = x.Success ? "Success" : "Failed",
                Message = x.ErrorMessage ?? x.ActionExecuted
            })
            .ToListAsync();

        return Ok(ApiResponse<List<AutomationExecutionDto>>.SuccessResponse(items));
    }

    private static JsonElement? TryParseJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
        catch
        {
            return null;
        }
    }

    public class SaveAutomationRuleRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Trigger { get; set; }
        public JsonElement? Condition { get; set; }
        public JsonElement? Action { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class AutomationRuleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string Trigger { get; set; } = string.Empty;
        public JsonElement? Condition { get; set; }
        public JsonElement? Action { get; set; }
        public int Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AutomationExecutionDto
    {
        public Guid Id { get; set; }
        public Guid RuleId { get; set; }
        public string RuleName { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; }
        public string Status { get; set; } = "Success";
        public string? Message { get; set; }
    }
}
