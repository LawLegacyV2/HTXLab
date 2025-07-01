using Microsoft.AspNetCore.Mvc;
using SphereApi.Models;
using SphereApi.Services;

namespace SphereApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SphereController : ControllerBase
{
    private readonly SphereService _sphereService;

    public SphereController(SphereService sphereService)
    {
        _sphereService = sphereService;
    }

    [HttpPost("check")]
    public IActionResult CheckPointInSphere([FromBody] SphereRequest request)
    {
        if (request.Radius < 0)
            return BadRequest("Radius must be non-negative.");

        var result = _sphereService.GetPointPosition(request);
        return Ok(new { position = result });
    }
}
