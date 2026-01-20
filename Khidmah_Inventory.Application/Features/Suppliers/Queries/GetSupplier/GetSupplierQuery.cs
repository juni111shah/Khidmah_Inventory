using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Suppliers.Models;

namespace Khidmah_Inventory.Application.Features.Suppliers.Queries.GetSupplier;

public class GetSupplierQuery : IRequest<Result<SupplierDto>>
{
    public Guid Id { get; set; }
}

