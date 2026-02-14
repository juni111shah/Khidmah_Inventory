using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Models;

public class AccountDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public Guid? ParentAccountId { get; set; }
    public bool IsActive { get; set; }
    public List<AccountDto> Children { get; set; } = new();
}
