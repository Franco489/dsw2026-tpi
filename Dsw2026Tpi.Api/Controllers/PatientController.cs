using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;

namespace Dsw2026Tpi.Api.Controllers
{
    [Route("api/patient")]
    public class PatientController : AppController
    {
        private readonly IPatientService _service;
        public PatientController(IPatientService service)
        {
            _service = service;
        }

        [HttpPut("profile")]
        public async Task<IActionResult> CompleteProfile([FromRoute] string dni, [FromBody] ProfileModel.Request request)
        {
            try
            {
                await _service.CompleteProfileAsync(dni, request);
                return Ok(new { message = "El perfil del paciente se actualizó con éxito." });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
