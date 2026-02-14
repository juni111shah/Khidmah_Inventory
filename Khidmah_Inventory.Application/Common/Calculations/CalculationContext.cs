namespace Khidmah_Inventory.Application.Common.Calculations;

/// <summary>
/// Shared context for calculation services: date range, filters, tenant.
/// </summary>
public class CalculationContext
{
    public Guid CompanyId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? CategoryId { get; set; }
    public int? DeadStockDays { get; set; } = 90;
}
