using Formaxtech.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Formaxtech.WebAPI.Algorithms
{
    public static class GridSearchAlgorithm
    {
        private const int CellSize = 10;

        public static List<Driver> FindNearestDrivers(Map map, int orderX, int orderY, int count)
        {
            if (!IsValidOrderPosition(map, orderX, orderY))
                throw new ArgumentException("Координаты заказа вне карты");

            var grid = BuildGrid(map);

            int centerCol = orderX / CellSize;
            int centerRow = orderY / CellSize;

            var candidates = new List<Driver>();
            int ring = 0;
            while (candidates.Count < count)
            {
                bool anyCellAdded = false;
                for (int dCol = -ring; dCol <= ring; dCol++)
                {
                    for (int dRow = -ring; dRow <= ring; dRow++)
                    {
                        if (Math.Abs(dCol) != ring && Math.Abs(dRow) != ring) continue;
                        int col = centerCol + dCol;
                        int row = centerRow + dRow;
                        if (col >= 0 && col < grid.GetLength(0) && row >= 0 && row < grid.GetLength(1))
                        {
                            var cell = grid[col, row];
                            if (cell != null)
                            {
                                candidates.AddRange(cell);
                                anyCellAdded = true;
                            }
                        }
                    }
                }

                if (!anyCellAdded)
                {
                    var allDrivers = map.Drivers.Except(candidates)
                        .OrderBy(d => GetDistance(d.X, d.Y, orderX, orderY))
                        .Take(count - candidates.Count);
                    candidates.AddRange(allDrivers);
                    break;
                }

                ring++;
            }

            return candidates
                .Distinct()
                .OrderBy(d => GetDistance(d.X, d.Y, orderX, orderY))
                .Take(count)
                .ToList();
        }

        private static List<Driver>[,] BuildGrid(Map map)
        {
            int cols = (int)Math.Ceiling((double)map.Width / CellSize);
            int rows = (int)Math.Ceiling((double)map.Height / CellSize);
            var grid = new List<Driver>[cols, rows];

            foreach (var driver in map.Drivers)
            {
                int col = driver.X / CellSize;
                int row = driver.Y / CellSize;
                if (grid[col, row] == null)
                    grid[col, row] = new List<Driver>();
                grid[col, row].Add(driver);
            }

            return grid;
        }

        private static double GetDistance(int x1, int y1, int x2, int y2)
            => Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

        private static bool IsValidOrderPosition(Map map, int x, int y)
            => x >= 0 && x < map.Width && y >= 0 && y < map.Height;
    }
}