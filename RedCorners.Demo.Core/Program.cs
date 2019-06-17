using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedCorners;
using RedCorners.Components;

namespace RedCorners.Demo.Core
{
    class Program
    {
        public class Settings
        {
            public int Count { get; set; }
        }

        static void Main(string[] args)
        {
            var settings = new ObjectStorage<Settings>();
            Console.WriteLine($"Default Count: {settings.Data.Count}");

            settings.Data.Count++;
            settings.Save();

            var configuration = new Dictionary<string, object>
            {
                { "Count", 1000 }
            };

            configuration.Inject(settings.Data);
            Console.WriteLine($"New Count: {settings.Data.Count}");
        }
    }
}
