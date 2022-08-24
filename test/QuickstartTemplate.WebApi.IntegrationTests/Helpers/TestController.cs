using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace QuickstartTemplate.WebApi.IntegrationTests.Helpers;

[ApiController]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ApiVersion("1.0")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class TestController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public TestController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult UserDetail()
    {
        return Ok(User.Identity.Name);
    }
}
