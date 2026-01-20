using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Suppliers.Models;

namespace Khidmah_Inventory.Application.Features.Suppliers.Commands.ActivateSupplier;

public class ActivateSupplierCommand : IRequest<Result<SupplierDto>>
{
    public Guid Id { get; set; }
}

