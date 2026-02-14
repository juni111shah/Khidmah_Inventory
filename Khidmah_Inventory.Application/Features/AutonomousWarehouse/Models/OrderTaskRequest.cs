namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

public class OrderTaskRequest
{
    public List<Guid>? SalesOrderIds { get; set; }
    public List<Guid>? PurchaseOrderIds { get; set; }
}
