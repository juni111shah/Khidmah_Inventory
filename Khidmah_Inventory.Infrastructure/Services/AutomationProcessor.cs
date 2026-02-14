using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Services;

public class AutomationProcessor : IAutomationExecutor
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IOperationsBroadcast? _broadcast;

    public AutomationProcessor(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IOperationsBroadcast? broadcast = null)
    {
        _context = context;
        _currentUser = currentUser;
        _broadcast = broadcast;
    }

    public async Task ExecuteStockBelowThresholdAsync(
        Guid companyId,
        Guid productId,
        Guid? warehouseId,
        decimal currentQuantity,
        decimal minStockLevel,
        CancellationToken cancellationToken = default)
    {
        var rules = await LoadActiveRules(companyId, "StockBelowThreshold", cancellationToken);
        if (rules.Count == 0) return;

        var product = await _context.Products
            .Where(p => p.CompanyId == companyId && p.Id == productId && !p.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
        if (product == null) return;

        foreach (var rule in rules)
        {
            var contextObj = new
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                CurrentQuantity = currentQuantity,
                MinStockLevel = minStockLevel
            };

            if (!EvaluateCondition(rule, contextObj))
            {
                await AddHistory(rule, contextObj, "SkippedByCondition", true, null, cancellationToken);
                continue;
            }

            try
            {
                var actionType = GetActionType(rule);
                if (actionType == "CreatePO")
                {
                    var supplier = await _context.Suppliers
                        .Where(s => s.CompanyId == companyId && s.IsActive && !s.IsDeleted)
                        .OrderBy(s => s.Name)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (supplier == null)
                    {
                        await AddHistory(rule, contextObj, "CreatePO", false, "No active supplier found", cancellationToken);
                        continue;
                    }

                    var qtyToOrder = GetDecimal(rule, "targetQuantity", Math.Max(minStockLevel * 2, minStockLevel + 1));
                    var poNumber = $"AUTO-PO-{DateTime.UtcNow:yyyyMMddHHmmss}";
                    var po = new PurchaseOrder(companyId, poNumber, supplier.Id, DateTime.UtcNow, _currentUser.UserId);
                    po.UpdateStatus("Draft", _currentUser.UserId);

                    var item = new PurchaseOrderItem(companyId, po.Id, productId, qtyToOrder, product.PurchasePrice, _currentUser.UserId);
                    item.Update(qtyToOrder, product.PurchasePrice, 0, 0, "Auto-created by automation", _currentUser.UserId);
                    po.AddItem(item);
                    po.CalculateTotals();

                    _context.PurchaseOrders.Add(po);
                    await _context.SaveChangesAsync(cancellationToken);

                    await AddHistory(rule, contextObj, "CreatePO", true, null, cancellationToken);
                }
                else
                {
                    var title = $"Automation: {rule.Name}";
                    var message = $"Rule executed for low stock on {product.Name} ({product.SKU}).";
                    _context.Notifications.Add(new Notification(companyId, title, message, "Warning", null, "Product", productId, _currentUser.UserId));
                    await _context.SaveChangesAsync(cancellationToken);
                    await AddHistory(rule, contextObj, "Notify", true, null, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                await AddHistory(rule, contextObj, GetActionType(rule), false, ex.Message, cancellationToken);
            }
        }

        if (_broadcast != null)
        {
            await _broadcast.BroadcastAsync(
                Application.Common.Constants.OperationsEventNames.LowStockDetected,
                companyId,
                productId,
                "Product",
                new { WarehouseId = warehouseId, CurrentQuantity = currentQuantity, MinStockLevel = minStockLevel },
                cancellationToken);
        }
    }

    public async Task ExecutePOApprovedAsync(Guid companyId, Guid purchaseOrderId, CancellationToken cancellationToken = default)
    {
        var rules = await LoadActiveRules(companyId, "POApproved", cancellationToken);
        if (rules.Count == 0) return;

        foreach (var rule in rules)
        {
            var contextObj = new { PurchaseOrderId = purchaseOrderId };
            if (!EvaluateCondition(rule, contextObj))
            {
                await AddHistory(rule, contextObj, "SkippedByCondition", true, null, cancellationToken);
                continue;
            }

            _context.Notifications.Add(new Notification(
                companyId,
                $"Automation: {rule.Name}",
                $"PO {purchaseOrderId} approved. Rule executed.",
                "Info",
                null,
                "PurchaseOrder",
                purchaseOrderId,
                _currentUser.UserId));

            await _context.SaveChangesAsync(cancellationToken);
            await AddHistory(rule, contextObj, "Notify", true, null, cancellationToken);
        }
    }

    public async Task ExecuteSaleCreatedAsync(Guid companyId, Guid salesOrderId, CancellationToken cancellationToken = default)
    {
        var rules = await LoadActiveRules(companyId, "SaleCreated", cancellationToken);
        if (rules.Count == 0) return;

        var salesOrder = await _context.SalesOrders
            .Where(s => s.CompanyId == companyId && s.Id == salesOrderId && !s.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
        if (salesOrder == null) return;

        foreach (var rule in rules)
        {
            var contextObj = new { SalesOrderId = salesOrderId, TotalAmount = salesOrder.TotalAmount };
            if (!EvaluateCondition(rule, contextObj))
            {
                await AddHistory(rule, contextObj, "SkippedByCondition", true, null, cancellationToken);
                continue;
            }

            var title = $"Automation: {rule.Name}";
            var message = $"Sales order {salesOrder.OrderNumber} triggered automation.";
            _context.Notifications.Add(new Notification(companyId, title, message, "Info", null, "SalesOrder", salesOrderId, _currentUser.UserId));
            await _context.SaveChangesAsync(cancellationToken);
            await AddHistory(rule, contextObj, "Notify", true, null, cancellationToken);
        }
    }

    private async Task<List<AutomationRule>> LoadActiveRules(Guid companyId, string trigger, CancellationToken cancellationToken)
    {
        return await _context.AutomationRules
            .Where(r => r.CompanyId == companyId && r.IsActive && !r.IsDeleted && r.Trigger == trigger)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    private bool EvaluateCondition(AutomationRule rule, object contextObj)
    {
        if (string.IsNullOrWhiteSpace(rule.ConditionJson))
            return true;

        try
        {
            using var doc = JsonDocument.Parse(rule.ConditionJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("params", out var parameters))
                return true;

            // Supports simple threshold validation used by frontend builder.
            if (parameters.TryGetProperty("threshold", out var thresholdElement) &&
                contextObj is not null &&
                thresholdElement.TryGetDecimal(out var threshold))
            {
                var json = JsonSerializer.Serialize(contextObj);
                using var ctxDoc = JsonDocument.Parse(json);
                if (ctxDoc.RootElement.TryGetProperty("CurrentQuantity", out var qty) && qty.TryGetDecimal(out var current))
                {
                    return current <= threshold;
                }
            }

            if (parameters.TryGetProperty("limit", out var limitElement) &&
                limitElement.TryGetDecimal(out var limit))
            {
                var json = JsonSerializer.Serialize(contextObj);
                using var ctxDoc = JsonDocument.Parse(json);
                if (ctxDoc.RootElement.TryGetProperty("TotalAmount", out var total) && total.TryGetDecimal(out var amount))
                {
                    return amount >= limit;
                }
            }
        }
        catch
        {
            // Fail-open for malformed condition JSON to avoid blocking transactions.
            return true;
        }

        return true;
    }

    private string GetActionType(AutomationRule rule)
    {
        try
        {
            using var doc = JsonDocument.Parse(rule.ActionJson);
            if (doc.RootElement.TryGetProperty("type", out var type))
            {
                return type.GetString() ?? "Notify";
            }
        }
        catch
        {
            // fallback
        }

        return "Notify";
    }

    private decimal GetDecimal(AutomationRule rule, string key, decimal fallback)
    {
        try
        {
            using var doc = JsonDocument.Parse(rule.ActionJson);
            if (doc.RootElement.TryGetProperty("params", out var parameters) &&
                parameters.TryGetProperty(key, out var value) &&
                value.TryGetDecimal(out var parsed))
            {
                return parsed;
            }
        }
        catch
        {
            // fallback
        }

        return fallback;
    }

    private async Task AddHistory(
        AutomationRule rule,
        object triggerContext,
        string actionExecuted,
        bool success,
        string? errorMessage,
        CancellationToken cancellationToken)
    {
        var history = new AutomationRuleHistory(
            rule.CompanyId,
            rule.Id,
            rule.Trigger,
            JsonSerializer.Serialize(triggerContext),
            actionExecuted,
            success,
            errorMessage,
            _currentUser.UserId);

        _context.AutomationRuleHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
