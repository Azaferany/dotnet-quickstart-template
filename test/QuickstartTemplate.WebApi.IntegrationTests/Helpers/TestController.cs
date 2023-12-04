using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using QuickstartTemplate.ApplicationCore.Resources;

namespace QuickstartTemplate.WebApi.IntegrationTests.Helpers;

[ApiController]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ApiVersion("1.0")]
[Produces("application/json")]
[Consumes("application/json")]
public class TestController : ControllerBase
{
    private readonly IStringLocalizer<SharedResource> _stringLocalizer;

    public TestController(IStringLocalizer<SharedResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    [HttpGet]
    [Authorize]
    public IActionResult UserDetail()
    {
        return Ok(User.Identity.Name);
    }
    [HttpGet]
    public IActionResult Culture()
    {
        return Ok(_stringLocalizer["Test"].ToString());
    }
}
