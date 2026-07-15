using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[Route("specialities")]
public class SpecialityController : AppController
{
    private readonly ISpecialityService _service;
    public SpecialityController(ISpecialityService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSpecialityByName([FromQuery] int pageSize=10, [FromQuery] int pageIndex=1, [FromQuery] string? name = null)
    {
        var specialities = await _service.FilterSpecialityByName(pageSize, pageIndex, name);
        return Ok(specialities);
    }

    [HttpPost]
    public async Task<IActionResult> AddSpeciality(SpecialityModel.CreateRequest request)
    {
        await _service.AddSpeciality(request);
        return Created();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpeciality([FromRoute] Guid id, [FromBody] SpecialityModel.UpdateRequest request)
    {
        await _service.UpdateSpeciality(id, request);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveSpeciality([FromRoute] Guid id)
    {
        await _service.RemoveSpeciality(id);
        return Ok();
    }


}