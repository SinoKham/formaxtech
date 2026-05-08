using Formaxtech.WebAPI.DTOs;
using Formaxtech.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Formaxtech.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly MapService _mapService;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderController(MapService mapService, IHttpClientFactory httpClientFactory)
        {
            _mapService = mapService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("match")]
        public async Task<IActionResult> MatchOrder([FromBody] OrderRequest request)
        {
            var map = _mapService.GetMap();

            // Проверка координат заказа
            if (request.X < 0 || request.X >= map.Width || request.Y < 0 || request.Y >= map.Height)
            {
                return BadRequest("Координаты некорректны");
            }

            var drivers = map.Drivers;
            if (!drivers.Any())
            {
                return BadRequest("Свободных водителей нет");
            }

            // Находим 5 ближайших водителей
            var nearestDrivers = _mapService.FindNearestDrivers(request.X, request.Y, 5);

            // Получаем случайный индекс
            int randomIndex = await GetRandomIndexAsync(nearestDrivers.Count);

            var selectedDriver = nearestDrivers[randomIndex];

            // Строим маршрут
            var route = BuildRoute(selectedDriver.X, selectedDriver.Y, request.X, request.Y);
            var routeLength = CalculateRouteLength(route);

            var response = new OrderResponse
            {
                DriverId = selectedDriver.Id,
                DriverX = selectedDriver.X,
                DriverY = selectedDriver.Y,
                RouteLength = routeLength,
                Route = route
            };

            return Ok(response);
        }

        private async Task<int> GetRandomIndexAsync(int max)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(
                    $"http://www.randomnumberapi.com/api/v1.0/random?min=0&max={max - 1}&count=1");

                var numbers = JsonSerializer.Deserialize<int[]>(response);
                return numbers?[0] ?? Random.Shared.Next(0, max);
            }
            catch
            {
                return Random.Shared.Next(0, max);
            }
        }

        private List<RoutePoint> BuildRoute(int fromX, int fromY, int toX, int toY)
        {
            var route = new List<RoutePoint>();

            int x = fromX;
            int y = fromY;

            int dx = Math.Abs(toX - fromX);
            int dy = Math.Abs(toY - fromY);
            int sx = fromX < toX ? 1 : -1;
            int sy = fromY < toY ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                route.Add(new RoutePoint { X = x, Y = y });

                if (x == toX && y == toY)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }

            return route;
        }

        private double CalculateRouteLength(List<RoutePoint> route)
        {
            double length = 0;
            for (int i = 0; i < route.Count - 1; i++)
            {
                var p1 = route[i];
                var p2 = route[i + 1];
                length += Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            }
            return Math.Round(length, 2);
        }
    }
}