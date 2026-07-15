using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[Route("doctors")]
//[Authorize(Policy = Policies.AdminPolicy)]
public class DoctorController : AppController
{
    private readonly IDoctorService _service;

    public DoctorController(IDoctorService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery]int pageSize=10, [FromQuery]int pageIndex=1, [FromQuery]string? name = null)
    {
        var doctors = await _service.GetAll(pageSize, pageIndex, name);
        return Ok(doctors);
    }

    [HttpPost]
    public async Task<IActionResult> AddDoctor(DoctorModel.Request request)
    {
        await _service.AddDoctor(request);
        return Created();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDoctor([FromRoute] Guid id, [FromBody] DoctorModel.Request request)
    {
        await _service.UpdateDoctor(id, request);
        return Ok();
    }
}
