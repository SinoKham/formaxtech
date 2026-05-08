using Formaxtech.WebAPI.Models;
using Formaxtech.WebAPI.Algorithms;

namespace Formaxtech.WebAPI.Services
{
    public class MapService
    {
        private readonly Map _map;

        public MapService(IConfiguration config)
        {
            var width = config.GetValue<int>("MapSettings:Width");
            var height = config.GetValue<int>("MapSettings:Height");
            _map = new Map(width, height);
        }

        public Map GetMap() => _map;

        public bool TryAddDriver(Driver driver, out string? error)
            => _map.TryAddDriver(driver, out error);

        public bool TryUpdateDriverPosition(int id, int x, int y, out string? error)
            => _map.TryUpdateDriverPosition(id, x, y, out error);

        public bool RemoveDriver(int id)
            => _map.RemoveDriver(id);

        public Driver? GetDriver(int id)
            => _map.GetDriver(id);

        public List<Driver> FindNearestDrivers(int orderX, int orderY, int count)
            => GridSearchAlgorithm.FindNearestDrivers(_map, orderX, orderY, count);
    }
}