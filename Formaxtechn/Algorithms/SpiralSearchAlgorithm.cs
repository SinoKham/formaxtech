using Formaxtech.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Formaxtech.Algorithms
{
    public static class SpiralSearchAlgorithm
    {
        public static List<Driver> FindNearestDrivers(Map map, int orderX, int orderY, int count)
        {
            if (!IsValidOrderPosition(map, orderX, orderY))
                throw new ArgumentException("Координаты заказа вне карты");

            var found = new List<Driver>();
            if (count <= 0) return found;

            // Проверяем точку заказа
            var driverAtOrder = map.Drivers.FirstOrDefault(d => d.X == orderX && d.Y == orderY);
            if (driverAtOrder != null)
                found.Add(driverAtOrder);

            int radius = 1;
            while (found.Count < count)
            {
                // Проходим по периметру квадрата со стороной 2*radius
                for (int dx = -radius; dx <= radius && found.Count < count; dx++)
                    AddDriverAt(map, orderX + dx, orderY - radius, found);

                for (int dy = -radius + 1; dy <= radius && found.Count < count; dy++)
                    AddDriverAt(map, orderX + radius, orderY + dy, found);

                for (int dx = radius - 1; dx >= -radius && found.Count < count; dx--)
                    AddDriverAt(map, orderX + dx, orderY + radius, found);

                for (int dy = radius - 1; dy >= -radius + 1 && found.Count < count; dy--)
                    AddDriverAt(map, orderX - radius, orderY + dy, found);

                if (radius > Math.Max(map.Width, map.Height))
                {
                    var remaining = map.Drivers
                        .Except(found)
                        .OrderBy(d => GetDistance(d.X, d.Y, orderX, orderY))
                        .Take(count - found.Count);
                    found.AddRange(remaining);
                    break;
                }

                radius++;
            }

            return found.Take(count).ToList();
        }

        private static void AddDriverAt(Map map, int x, int y, List<Driver> found)
        {
            if (x < 0 || x >= map.Width || y < 0 || y >= map.Height) return;
            var driver = map.Drivers.FirstOrDefault(d => d.X == x && d.Y == y);
            if (driver != null && !found.Any(d => d.Id == driver.Id))
                found.Add(driver);
        }

        private static double GetDistance(int x1, int y1, int x2, int y2)
            => Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

        private static bool IsValidOrderPosition(Map map, int x, int y)
            => x >= 0 && x < map.Width && y >= 0 && y < map.Height;
    }
}