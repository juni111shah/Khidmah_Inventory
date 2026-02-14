namespace Khidmah_Inventory.Application.Features.Intelligence.Models;

public class WarehouseMetricsDto
{
    public List<WarehouseMetricItemDto> Warehouses { get; set; } = new();
}

public class WarehouseMetricItemDto
{
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public decimal UtilizationPercent { get; set; }
    public decimal StockValue { get; set; }
    public int TransferInCount { get; set; }
    public int TransferOutCount { get; set; }
    public int Rank { get; set; }
    public int TotalWarehouses { get; set; }
}
