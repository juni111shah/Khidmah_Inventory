namespace Khidmah_Inventory.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

