using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[Route("api/availabilities")]
public class AvailabilityController : AppController
{
    private readonly IAvailabilityService _service;

    public AvailabilityController(IAvailabilityService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAvailabilities([FromBody] AvailabilityModel.Request request)
    {
        await _service.CreateAvailabilitiesAsync(request);
        return Ok(request);
    }


    [HttpPut]
    public async Task<IActionResult> UpdateAvailabilities([FromBody] AvailabilityModel.Request request)
    {
        await _service.UpdateAvailabilitiesAsync(request);
        return Ok(request);
    }

}
