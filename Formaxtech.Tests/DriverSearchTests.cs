using Formaxtech.Algorithms;
using Formaxtech.Models;
using NUnit.Framework;
using System;
using System.Linq;

namespace Formaxtech.Tests
{
    [TestFixture]
    public class DriverSearchTests
    {
        private Map _map;

        [SetUp]
        public void Setup()
        {
            _map = new Map(10, 10);
            _map.AddDriver(new Driver { Id = 1, X = 0, Y = 0 });
            _map.AddDriver(new Driver { Id = 2, X = 1, Y = 1 });
            _map.AddDriver(new Driver { Id = 3, X = 5, Y = 5 });
            _map.AddDriver(new Driver { Id = 4, X = 2, Y = 2 });
            _map.AddDriver(new Driver { Id = 5, X = 3, Y = 3 });
            _map.AddDriver(new Driver { Id = 6, X = 4, Y = 4 });
            _map.AddDriver(new Driver { Id = 7, X = 9, Y = 9 });
            _map.AddDriver(new Driver { Id = 8, X = 8, Y = 8 });
        }

        [Test]
        public void FullScan_ReturnsCorrectCount()
        {
            var result = FullScanAlgorithm.FindNearestDrivers(_map, 0, 0, 3);
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void FullScan_ReturnsCorrectOrder()
        {
            var result = FullScanAlgorithm.FindNearestDrivers(_map, 0, 0, 5);
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[2].Id, Is.EqualTo(4));
        }

        [Test]
        public void FullScan_ThrowsOnInvalidOrder()
        {
            Assert.Throws<ArgumentException>(() =>
                FullScanAlgorithm.FindNearestDrivers(_map, -1, 0, 5));
        }

        [Test]
        public void SpiralSearch_ReturnsCorrectCount()
        {
            var result = SpiralSearchAlgorithm.FindNearestDrivers(_map, 0, 0, 3);
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void SpiralSearch_ReturnsNearestFirst()
        {
            var result = SpiralSearchAlgorithm.FindNearestDrivers(_map, 0, 0, 5);
            Assert.That(result[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void GridSearch_ReturnsCorrectCount()
        {
            var result = GridSearchAlgorithm.FindNearestDrivers(_map, 0, 0, 3);
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void AllAlgorithms_ReturnSameResults()
        {
            var expected = FullScanAlgorithm.FindNearestDrivers(_map, 5, 5, 8)
                                           .Select(d => d.Id)
                                           .OrderBy(id => id)
                                           .ToList();
            var spiral = SpiralSearchAlgorithm.FindNearestDrivers(_map, 5, 5, 8)
                                             .Select(d => d.Id)
                                             .OrderBy(id => id)
                                             .ToList();
            var grid = GridSearchAlgorithm.FindNearestDrivers(_map, 5, 5, 8)
                                         .Select(d => d.Id)
                                         .OrderBy(id => id)
                                         .ToList();

            CollectionAssert.AreEqual(expected, spiral);
            CollectionAssert.AreEqual(expected, grid);
        }
    }
}