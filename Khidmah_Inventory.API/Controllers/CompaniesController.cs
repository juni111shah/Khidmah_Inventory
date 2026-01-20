using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Controllers;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompaniesController : BaseApiController
{
    // Example structure - will be implemented with CQRS commands/queries
    // GET api/companies
    // GET api/companies/{id}
    // POST api/companies
    // PUT api/companies/{id}
}

