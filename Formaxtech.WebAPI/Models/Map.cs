using System.Collections.Concurrent;

namespace Formaxtech.WebAPI.Models
{
    public class Map
    {
        public int Width { get; }
        public int Height { get; }

        private readonly ConcurrentDictionary<int, Driver> _drivers = new();
        private readonly ConcurrentDictionary<(int X, int Y), int> _occupiedPositions = new();

        public IReadOnlyList<Driver> Drivers => _drivers.Values.ToList().AsReadOnly();

        public Map(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Размеры карты должны быть положительными");
            Width = width;
            Height = height;
        }

        public bool TryAddDriver(Driver driver, out string? error)
        {
            error = null;

            if (driver == null)
            {
                error = "Водитель не может быть null";
                return false;
            }

            if (!IsValidPosition(driver.X, driver.Y))
            {
                error = "Координаты некорректны";
                return false;
            }

            var position = (driver.X, driver.Y);
            if (_occupiedPositions.ContainsKey(position))
            {
                error = "Здесь уже находится другой водитель";
                return false;
            }

            if (!_drivers.TryAdd(driver.Id, driver))
            {
                error = $"Водитель с Id {driver.Id} уже существует";
                return false;
            }

            _occupiedPositions[position] = driver.Id;
            return true;
        }

        public bool TryUpdateDriverPosition(int id, int x, int y, out string? error)
        {
            error = null;

            if (!_drivers.TryGetValue(id, out var driver))
            {
                error = $"Водитель с Id {id} не найден";
                return false;
            }

            if (!IsValidPosition(x, y))
            {
                error = "Координаты некорректны";
                return false;
            }

            var newPosition = (x, y);

            if (_occupiedPositions.TryGetValue(newPosition, out var existingId) && existingId != id)
            {
                error = "Здесь уже находится другой водитель";
                return false;
            }

            var oldPosition = (driver.X, driver.Y);
            _occupiedPositions.TryRemove(oldPosition, out _);

            driver.X = x;
            driver.Y = y;
            _occupiedPositions[newPosition] = id;

            return true;
        }

        public bool RemoveDriver(int id)
        {
            if (_drivers.TryRemove(id, out var driver))
            {
                _occupiedPositions.TryRemove((driver.X, driver.Y), out _);
                return true;
            }
            return false;
        }

        public Driver? GetDriver(int id)
        {
            _drivers.TryGetValue(id, out var driver);
            return driver;
        }

        private bool IsValidPosition(int x, int y)
            => x >= 0 && x < Width && y >= 0 && y < Height;
    }
}