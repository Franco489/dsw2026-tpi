using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Dsw2026Tpi.CrossCutting.Exceptions;

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
   public async Task<IActionResult> CreateAppointment([FromBody] AppointmentModel.Request request)
    {
        var response = await _service.CreateAppointment(request);
        return Ok(response);
    }
    //ver los turnos del paciente
    [HttpGet("patient")]
    public async Task<IActionResult> GetPatientAppointments([FromQuery] int dni)
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

    [HttpGet("search")]
    public async Task<IActionResult> SearchAppointments([FromQuery] Guid? specialtyId,[FromQuery] Guid? doctorId,[FromQuery] string? dni, [FromQuery] DateOnly? date, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
    {
        var result = await _service.CombinedSearch(pageSize,pageIndex,specialtyId, doctorId, dni, date);
        return Ok(result);
    }
}
