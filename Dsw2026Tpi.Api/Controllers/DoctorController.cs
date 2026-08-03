using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[Route("api/doctors")]
//[Authorize(Policy = Policies.AdminPolicy)]
public class DoctorController : AppController
{
    private readonly IDoctorService _service;

    public DoctorController(IDoctorService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)] //TODO: Esto no hace falta, verda?
    public async Task<IActionResult> GetAll([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0, [FromQuery]string? name = null)
    {
        var doctors = await _service.GetAll(pageSize, pageIndex, name);
        return Ok(doctors);
    }

    [HttpGet("{id}/availabilities")]
    [AllowAnonymous]

    public async Task<IActionResult> GetAvailabilities([FromRoute] Guid id, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
    {
        var availabilities = await _service.GetAvailabilities(id, pageSize, pageIndex);
        return Ok(availabilities);
    }


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateDoctor(DoctorModel.Request request)
    {
        var doctor = await _service.CreateDoctor(request);
        return Ok(doctor);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDoctor(Guid id, DoctorModel.Request request)
    {
        var doctor = await _service.UpdateDoctor(id, request);
        return Ok(doctor);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor([FromRoute] Guid id)
    {
        await _service.DeleteDoctor(id);
        return Ok("ok");
    }

    [HttpPut("activate")]
    public async Task<IActionResult> ReactivateDoctor([FromQuery] Guid id)
    {
        await _service.ReactivateDoctor(id);
        return Ok("Doctor reactivado correctamente");
    }
}
