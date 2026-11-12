using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;

namespace Dsw2026Tpi.Api.Controllers;

[Route("api/patient")]
public class PatientController : AppController
{
    private readonly IPatientService _service;
    public PatientController(IPatientService service)
    {
        _service = service;
    }

    [HttpPut("profile")]
    public async Task<IActionResult> CompleteProfile(string dni, [FromBody] ProfileModel.Request request)
    {
        await _service.CompleteProfileAsync(dni, request);
        return Ok("Perfil actualizado con éxito.");
    }
}
