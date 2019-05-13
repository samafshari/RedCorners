using System;
using System.Threading.Tasks;
using RedCorners;
using RedCorners.Components;

namespace RedCorners.Demo.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkAsync().GetAwaiter().GetResult();
        }

        static async Task WorkAsync()
        {
            Benchmark benchmark = new Benchmark();
            await Task.Delay(1234);
            Console.WriteLine(benchmark.ToString());
            await Task.Delay(4321);
            Console.WriteLine(benchmark.StopToString());
        }
    }
}
