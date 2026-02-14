using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Budget plan per account per year. Optional monthly breakdown.
/// </summary>
public class Budget : Entity
{
    public int Year { get; private set; }
    public Guid AccountId { get; private set; }
    public decimal PlannedAmount { get; private set; }
    /// <summary>
    /// Optional JSON: { "1": 1000, "2": 1200, ... } for monthly amounts.
    /// If null, PlannedAmount is annual and distributed evenly or used as total.
    /// </summary>
    public string? MonthlyAmountsJson { get; private set; }
    public string? Notes { get; private set; }

    public virtual Account Account { get; private set; } = null!;

    private Budget() { }

    public Budget(
        Guid companyId,
        int year,
        Guid accountId,
        decimal plannedAmount,
        string? monthlyAmountsJson = null,
        string? notes = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Year = year;
        AccountId = accountId;
        PlannedAmount = plannedAmount;
        MonthlyAmountsJson = monthlyAmountsJson;
        Notes = notes;
    }

    public void Update(decimal plannedAmount, string? monthlyAmountsJson = null, string? notes = null, Guid? updatedBy = null)
    {
        PlannedAmount = plannedAmount;
        MonthlyAmountsJson = monthlyAmountsJson;
        Notes = notes;
        UpdateAuditInfo(updatedBy);
    }
}
