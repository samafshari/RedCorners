using RedCorners.Models;

using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RedCorners.Components;

namespace RedCorners.Services
{
    public class BenchmarkService
    {
        public static BenchmarkService Instance { get; private set; } = new BenchmarkService();
        public static BenchmarkService SetInstance(BenchmarkService instance) => Instance = instance;

        public bool IsDebug;
        public bool IsEnabled;
        public bool ShouldPrintAfterStops = true;
        public bool Echo = true;
        public uint AutoPrintInterval = 2000;

        public readonly ConcurrentDictionary<Benchmark, string> Messages = new ConcurrentDictionary<Benchmark, string>();
        public readonly ConcurrentBag<(DateTime Date, TimeSpan Duration, string Message)> FinishedTasks = new ConcurrentBag<(DateTime Date, TimeSpan Duration, string Message)>();
        public bool IsPrintTimerActive => isPrintTimerActive;

        volatile bool isPrintTimerActive = false;

        public event EventHandler<ReportedException> OnExceptionReport;

        public IDisposable StartBenchmark([CallerMemberName] string message = null)
        {
            if (!IsEnabled)
                return null;

            var benchmark = new Benchmark();
            Messages[benchmark] = message;
            benchmark.Stopped += Benchmark_Stopped;
            if (Echo)
            {
                Console.WriteLine($"[Benchmark Start | {Messages.Count} active] {message}");
                StartPrintTimer();
            }
            return benchmark;
        }

        private void Benchmark_Stopped(object sender, TimeSpan e)
        {
            var b = sender as Benchmark;
            b.Stopped -= Benchmark_Stopped;
            StopBenchmark(b);
        }

        public void StopBenchmark(IDisposable handle, string message = null)
        {
            if (handle == null)
                return;

            var benchmark = handle as Benchmark;
            if (Messages.TryRemove(benchmark, out var startMessage))
            {
                FinishedTasks.Add((DateTime.Now, benchmark.Elapsed, $"{startMessage} {message}"));
                if (Echo)
                {
                    Console.WriteLine($"[Benchmark End | {Messages.Count} active] Elapsed: {benchmark.StopToString()} {startMessage} {message}");
                    if (ShouldPrintAfterStops)
                        PrintStatus();
                }
            }
        }

        private void StartPrintTimer()
        {
            Task.Run(async () =>
            {
                if (Messages.IsEmpty || isPrintTimerActive)
                    return;

                isPrintTimerActive = true;
                while (isPrintTimerActive && Messages.Count > 0)
                {
                    await Task.Delay((int)AutoPrintInterval);
                    PrintStatus();
                }
                isPrintTimerActive = false;
            });
        }

        public void PrintStatus()
        {
            if (!IsEnabled)
                return;

            Console.WriteLine(" ");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("ACTIVE BENCHMARKS");
            foreach (var message in Messages)
                Console.WriteLine($"{message.Key}\t{message.Value}");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("RECENT BENCHMARKS");
            foreach (var message in FinishedTasks.OrderByDescending(x => x.Date).Take(5))
                Console.WriteLine(message);
            Console.WriteLine("------------------------------------");
            Console.WriteLine("FINISHED BENCHMARKS");
            var summaries = FinishedTasks.GroupBy(x => x.Message).Select(x => new
            {
                Duration = TimeSpan.FromSeconds(x.Sum(y => y.Duration.TotalSeconds)),
                Message = x.Key,
                Count = x.Count()
            }).OrderByDescending(x => x.Duration);
            foreach (var item in summaries)
                Console.WriteLine($"{item.Duration}\tx{item.Count,-5}\t\t{item.Duration.TotalSeconds / item.Count:N3} s/t\t\t{item.Message}");
            Console.WriteLine(" ");
        }

        public void Throw(Exception ex, [CallerMemberName] string caller = null)
        {
            Report(ex, caller);
            throw ex;
        }

        public void ThrowIfDebug(Exception ex, [CallerMemberName] string caller = null)
        {
            Report(ex, caller);
            if (IsDebug) throw ex;
        }

        public void Report(Exception ex, [CallerMemberName] string caller = null)
        {
            if (Echo)
                Console.WriteLine($"[Exception Thrown by {caller}] {ex}");
            OnExceptionReport?.Invoke(this, new ReportedException
            {
                Exception = ex,
                Caller = caller
            });
        }

        public static void ThrowDefault(Exception ex, [CallerMemberName] string caller = null)
        {
            Instance.Throw(ex, caller);
        }
    }
}
