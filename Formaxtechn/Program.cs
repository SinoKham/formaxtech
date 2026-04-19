using BenchmarkDotNet.Running;
using Formaxtech.Benchmarks;

namespace Formaxtech
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DriverSearchBenchmark>();
        }
    }
}