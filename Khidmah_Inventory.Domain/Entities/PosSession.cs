using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class PosSession : Entity
{
    public Guid UserId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public decimal OpeningBalance { get; private set; }
    public decimal? ClosingBalance { get; private set; }
    public decimal? ExpectedBalance { get; private set; }
    public string Status { get; private set; } = "Open"; // Open, Closed

    public virtual User User { get; private set; } = null!;
    public virtual ICollection<SalesOrder> SalesOrders { get; private set; } = new List<SalesOrder>();

    private PosSession() { }

    public PosSession(Guid companyId, Guid userId, decimal openingBalance, Guid? createdBy = null)
        : base(companyId, createdBy)
    {
        UserId = userId;
        StartTime = DateTime.UtcNow;
        OpeningBalance = openingBalance;
        Status = "Open";
    }

    public void Close(decimal closingBalance, decimal expectedBalance, Guid? updatedBy = null)
    {
        EndTime = DateTime.UtcNow;
        ClosingBalance = closingBalance;
        ExpectedBalance = expectedBalance;
        Status = "Closed";
        UpdateAuditInfo(updatedBy);
    }
}
