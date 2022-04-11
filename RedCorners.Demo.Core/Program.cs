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
            var settings = new Settings();
            Console.WriteLine($"Default Count: {settings.Count}");

            var configuration = new Dictionary<string, object>
            {
                { "Count", 1000 }
            };

            configuration.InjectDictionary(settings);
            Console.WriteLine($"New Count: {settings.Count}");

            var dic = configuration.Export(ExportMode.AllButIgnored);
            foreach (var item in dic)
                Console.WriteLine($"{item.Key}: {item.Value}");
        }
    }
}
