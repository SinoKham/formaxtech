using BenchmarkDotNet.Attributes;
using Formaxtech.Algorithms;
using Formaxtech.Models;
using System;
using System.Collections.Generic;

namespace Formaxtech.Benchmarks
{
    [MemoryDiagnoser]
    public class DriverSearchBenchmark
    {
        private Map _map;
        private const int MapSize = 100;
        private const int DriversCount = 1000;
        private const int Seed = 42;

        [Params(1, 5, 10)]
        public int NearestCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _map = new Map(MapSize, MapSize);
            var random = new Random(Seed);
            for (int i = 0; i < DriversCount; i++)
            {
                _map.AddDriver(new Driver
                {
                    Id = i + 1,
                    X = random.Next(0, MapSize),
                    Y = random.Next(0, MapSize)
                });
            }
        }

        [Benchmark]
        public List<Driver> FullScan()
        {
            return FullScanAlgorithm.FindNearestDrivers(_map, 50, 50, NearestCount);
        }

        [Benchmark]
        public List<Driver> SpiralSearch()
        {
            return SpiralSearchAlgorithm.FindNearestDrivers(_map, 50, 50, NearestCount);
        }

        [Benchmark]
        public List<Driver> GridSearch()
        {
            return GridSearchAlgorithm.FindNearestDrivers(_map, 50, 50, NearestCount);
        }
    }
}