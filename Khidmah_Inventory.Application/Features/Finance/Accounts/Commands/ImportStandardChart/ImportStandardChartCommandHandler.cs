using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Commands.ImportStandardChart;

public class ImportStandardChartCommandHandler : IRequestHandler<ImportStandardChartCommand, Result<ImportStandardChartResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    private static readonly (string Code, string Name, AccountType Type)[] StandardAccounts =
    {
        ("CASH", "Cash", AccountType.Asset),
        ("AR", "Accounts Receivable", AccountType.Asset),
        ("INVENTORY", "Inventory", AccountType.Asset),
        ("AP", "Accounts Payable", AccountType.Liability),
        ("TAX", "Tax Payable", AccountType.Liability),
        ("REVENUE", "Revenue", AccountType.Revenue),
        ("EXPENSE", "Expenses", AccountType.Expense),
        ("EQUITY", "Equity", AccountType.Equity)
    };

    public ImportStandardChartCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ImportStandardChartResult>> Handle(ImportStandardChartCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<ImportStandardChartResult>.Failure("Company context is required.");

        var existingCodes = await _context.Accounts
            .Where(a => a.CompanyId == companyId && !a.IsDeleted)
            .Select(a => a.Code)
            .ToListAsync(cancellationToken);
        var existingSet = existingCodes.ToHashSet();

        var result = new ImportStandardChartResult();
        foreach (var (code, name, type) in StandardAccounts)
        {
            if (existingSet.Contains(code))
            {
                result.Skipped++;
                continue;
            }
            var account = new Account(companyId.Value, code, name, type, null, _currentUser.UserId);
            _context.Accounts.Add(account);
            result.Created++;
            existingSet.Add(code);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var all = await _context.Accounts
            .AsNoTracking()
            .Where(a => a.CompanyId == companyId && !a.IsDeleted)
            .OrderBy(a => a.Code)
            .ToListAsync(cancellationToken);
        result.Accounts = all.Select(a => new AccountDto
        {
            Id = a.Id,
            CompanyId = a.CompanyId,
            Code = a.Code,
            Name = a.Name,
            Type = a.Type,
            TypeName = a.Type.ToString(),
            ParentAccountId = a.ParentAccountId,
            IsActive = a.IsActive
        }).ToList();

        return Result<ImportStandardChartResult>.Success(result);
    }
}
