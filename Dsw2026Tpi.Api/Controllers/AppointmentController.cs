using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;

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
   public async Task<IActionResult> createAppointment([FromBody] AppointmentModel.Request request)
    {
        var response = await _service.CreateAppointment(request);
        return Ok(response);
    }
    //ver los turnos del paciente
    [HttpGet("patient")]
    public async Task<IActionResult> GetPatientAppointments([FromQuery] int dni)
    {
        if (dni <= 0 || dni.ToString().Length < 7 || dni.ToString().Length > 10)
        {
            return BadRequest(new
            {
                errorCode = "INVALID_DNI",
                message = "El DNI es obligatorio y debe tener entre 7 y 10 dígitos."
            }); //TODO: ESTOI NO VA A ACAAAAAAAAAAAAAAAAAA
        }

        var result = await _service.GetPatientAppointmentsAsync(dni);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelAppointment([FromRoute] Guid id)
    {
      await _service.CancelAppointmentAsync(id);
      return Ok("ok");
    }

    // busqueda de turnos
    [HttpGet("search")]
    public async Task<IActionResult> SearchAppointments(
        [FromQuery] Guid? specialtyId,
        [FromQuery] Guid? doctorId,
        [FromQuery] string? dni,
        [FromQuery] DateTime? date)
    {
        var result = await _service.SearchAppointmentsAsync(specialtyId, doctorId, dni, date);
        return Ok(result);
    }
}
