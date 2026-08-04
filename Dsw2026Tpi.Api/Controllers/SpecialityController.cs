using Dsw2026Tpi.Application.Intefaces;
using Dsw2026Tpi.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers
{
    [Route("specialities")]
    public class SpecialityController : AppController
    {
        private readonly ISpecialityService _service;
        public SpecialityController(ISpecialityService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecialityByName([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, [FromQuery] string? name = null)
        {
            return Ok(await _service.FilterSpecialityByName(pageSize, pageIndex, name));
        }

        [HttpPost]
        public async Task<IActionResult> AddSpeciality(SpecialityModel.Request request)
        {
            await _service.AddSpeciality(request);
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpeciality(Guid id, SpecialityModel.Request request)
        {
            await _service.UpdateSpeciality(id, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpeciality([FromRoute] Guid id)
        {
            await _service.DeleteSpeciality(id);
            return Ok();
        }

        [HttpPut("activate")]
        public async Task<IActionResult> ReactivateSpeciality([FromQuery] Guid id)
        {
            await _service.ReactivateSpeciality(id);
            return Ok();
        }

    }
}
