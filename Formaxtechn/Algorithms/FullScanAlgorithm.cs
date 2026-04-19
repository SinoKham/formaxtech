using Formaxtech.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Formaxtech.Algorithms
{
    public static class FullScanAlgorithm
    {
        public static List<Driver> FindNearestDrivers(Map map, int orderX, int orderY, int count)
        {
            if (!IsValidOrderPosition(map, orderX, orderY))
                throw new ArgumentException("Координаты заказа вне карты");

            return map.Drivers
                .OrderBy(d => GetDistance(d.X, d.Y, orderX, orderY))
                .Take(count)
                .ToList();
        }

        private static double GetDistance(int x1, int y1, int x2, int y2)
            => Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

        private static bool IsValidOrderPosition(Map map, int x, int y)
            => x >= 0 && x < map.Width && y >= 0 && y < map.Height;
    }
}