using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Suppliers.Commands.DeleteSupplier;

public class DeleteSupplierCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

