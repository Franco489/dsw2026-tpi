using Dsw2026Tpi.Application.Intefaces;
using Dsw2026Tpi.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers
{
    [Route("api/specialities")]
    public class SpecialityController : AppController
    {
        private readonly ISpecialityService _service;
        public SpecialityController(ISpecialityService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecialityByName([FromQuery] int pageSize, [FromQuery] int pageIndex, [FromQuery] string? name = null)
        {
            return Ok(await _service.FilterSpecialityByName(pageSize, pageIndex, name));
        }

        [HttpPost]
        public async Task<IActionResult> AddSpeciality(SpecialityModel.CreateRequest request)
        {
            await _service.AddSpeciality(request);
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpeciality(Guid id, SpecialityModel.UpdateRequest request)
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

    }
}
