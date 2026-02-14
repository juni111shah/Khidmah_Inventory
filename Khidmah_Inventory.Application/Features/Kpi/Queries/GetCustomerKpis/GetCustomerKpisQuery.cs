using MediatR;
using Khidmah_Inventory.Application.Common.Calculations.Dto;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Kpi.Queries.GetCustomerKpis;

public class GetCustomerKpisQuery : IRequest<Result<CustomerKpisDto>>
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
