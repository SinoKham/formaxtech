using Formaxtech.WebAPI.DTOs;
using Formaxtech.WebAPI.Models;
using Formaxtech.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Formaxtech.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly MapService _mapService;

        public DriverController(MapService mapService)
        {
            _mapService = mapService;
        }

        [HttpPost("set")]
        public IActionResult SetDriverPosition([FromBody] DriverRequest request)
        {
            var existingDriver = _mapService.GetDriver(request.Id);

            if (existingDriver == null)
            {
                // Новый водитель
                var driver = new Driver { Id = request.Id, X = request.X, Y = request.Y };
                if (_mapService.TryAddDriver(driver, out var error))
                {
                    return Ok("Координаты успешно добавлены");
                }
                return BadRequest(error);
            }
            else
            {
                // Обновление существующего
                if (_mapService.TryUpdateDriverPosition(request.Id, request.X, request.Y, out var error))
                {
                    return Ok("Координаты успешно изменены");
                }
                return BadRequest(error);
            }
        }
    }
}