using Microsoft.AspNetCore.Mvc;
using Dsw2026Tpi.Application.Interfaces;

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
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> GetSpecialityByName([FromQuery] int pageSize, [FromQuery] int pageIndex, [FromQuery] string? name = null)
        {
            var specialities = await _service.FilterSpecialityByName(pageSize, pageIndex, name);
            return Ok(specialities);
        }

    }
}
