using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Intefaces;
using Dsw2026Tpi.CrossCutting.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[Route("api/specialties")]
[Authorize(Policy = Policies.AdminPolicy)]
public class SpecialtyController : AppController
{
    private readonly ISpecialtyService _service;
    public SpecialtyController(ISpecialtyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetSpecialtyByName([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, [FromQuery] string? name = null)
    {
        return Ok(await _service.FilterSpecialtyByName(pageSize, pageIndex, name));
    }

    [HttpPost]
    public async Task<IActionResult> AddSpecialty(SpecialtyModel.Request request)
    {
        return Ok(await _service.AddSpecialty(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpecialty(Guid id, SpecialtyModel.Request request)
    {
        return Ok(await _service.UpdateSpecialty(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpecialty([FromRoute] Guid id)
    {
        await _service.DeleteSpecialty(id);
        return Ok("Ok");
    }

    [HttpPut("activate")]
    public async Task<IActionResult> ReactivateSpecialty([FromQuery] Guid id)
    {
        await _service.ReactivateSpecialty(id);
        return Ok();
    }

}
