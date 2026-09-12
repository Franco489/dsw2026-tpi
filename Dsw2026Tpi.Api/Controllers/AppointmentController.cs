using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Services;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Dsw2026Tpi.Api.Controllers;

[Route("api/appointments")]
public class AppointmentController : AppController
{
    private readonly IAppointmentService _service;

    public AppointmentController(IAppointmentService service)
    {
        _service = service;
    }

   [HttpPost]
   [EnableRateLimiting("BookingPolicy")]
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentModel.Request request)
    {
        var response = await _service.CreateAppointment(request);
        return Ok(response);
    }


    [HttpGet("patient")]
    public async Task<IActionResult> GetPatientAppointments([FromQuery] string dni)
    {
        var result = await _service.GetPatientAppointmentsAsync(dni);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelAppointment([FromRoute] Guid id)
    {
      await _service.CancelAppointmentAsync(id);
      return Ok("ok");
    }


    [HttpGet]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<IActionResult> GetAppointmentsByDate([FromQuery] DateOnly date)
    {
        var result = await _service.GetAppointmentsByDate(date);
        return Ok(result);
    }


    [HttpGet("search")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<IActionResult> SearchAppointments([FromQuery] Guid? specialtyId,[FromQuery] Guid? doctorId,[FromQuery] string? dni, [FromQuery] DateOnly? date, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
    {
        var result = await _service.CombinedSearch(pageSize,pageIndex,specialtyId, doctorId, dni, date);
        return Ok(result);
    }
}
