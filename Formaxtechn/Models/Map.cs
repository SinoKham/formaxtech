using System;
using System.Collections.Generic;
using System.Linq;

namespace Formaxtech.Models
{
    public class Map
    {
        public int Width { get; }
        public int Height { get; }
        private readonly List<Driver> _drivers = new();

        public IReadOnlyList<Driver> Drivers => _drivers;

        public Map(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Размеры карты должны быть положительными");
            Width = width;
            Height = height;
        }

        public void AddDriver(Driver driver)
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));
            if (!IsValidPosition(driver.X, driver.Y))
                throw new ArgumentOutOfRangeException($"Координаты ({driver.X}, {driver.Y}) вне границ карты");
            if (_drivers.Any(d => d.Id == driver.Id))
                throw new InvalidOperationException($"Водитель с Id {driver.Id} уже существует");

            _drivers.Add(driver);
        }

        public void RemoveDriver(int id)
        {
            var driver = _drivers.FirstOrDefault(d => d.Id == id);
            if (driver != null)
                _drivers.Remove(driver);
        }

        public void UpdateDriverPosition(int id, int x, int y)
        {
            if (!IsValidPosition(x, y))
                throw new ArgumentOutOfRangeException($"Координаты ({x}, {y}) вне границ карты");

            var driver = _drivers.FirstOrDefault(d => d.Id == id);
            if (driver == null)
                throw new InvalidOperationException($"Водитель с Id {id} не найден");

            driver.X = x;
            driver.Y = y;
        }

        private bool IsValidPosition(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
    }
}