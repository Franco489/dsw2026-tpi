using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[Route("api/doctors")]
public class DoctorController : AppController
{
    private readonly IDoctorService _service;

    public DoctorController(IDoctorService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0, [FromQuery]string? name = null)
    {
        var doctors = await _service.GetAll(pageSize, pageIndex, name);
        return Ok(doctors);
    }

    [HttpGet("{id}/availabilities")]
    public async Task<IActionResult> GetAvailabilities([FromRoute] Guid id, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
    {
        var availabilities = await _service.GetAvailabilities(id, pageSize, pageIndex);
        return Ok(availabilities);
    }

<<<<<<< HEAD

    [HttpPost]
    [Authorize(Policy = Policies.AdminPolicy)]
=======
    // si el profesor me pide que solo el admin cree o modique doctores debo agregar la politica [Authorize(Policy = Policies.AdminPolicy)] y eliminar los allowanonymus 
    [HttpPost] //permite que cualquiera incluso sin loguearse cree un medico 
    [AllowAnonymous]
>>>>>>> a7e87a9 (cambios DNI a long y mapeo a string para validacion y persistencia)
    public async Task<IActionResult> CreateDoctor(DoctorModel.Request request)
    {
        var doctor = await _service.CreateDoctor(request);
        return Ok(doctor); //si quiero que devuelva 201 return CreatedAtAction(nameof(GetAll), new { id = doctor.Id }, doctor);
    }

    //aca tmb debo poner la politica porque este endpoint modifica 
    [HttpPut("{id}")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<IActionResult> UpdateDoctor(Guid id, DoctorModel.Request request)
    {
        var doctor = await _service.UpdateDoctor(id, request);
        return Ok(doctor);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<IActionResult> DeleteDoctor([FromRoute] Guid id)
    {
        await _service.DeleteDoctor(id);
        return Ok("ok"); //tenia return Ok ("ok"); es 200 ok - NotContent es 204 
    }

    [HttpPut("activate")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<IActionResult> ReactivateDoctor([FromQuery] Guid id)
    {
        await _service.ReactivateDoctor(id);
        return Ok("Doctor reactivado correctamente");
    }
}
