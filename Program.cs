using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CronManager
{
    public class ScheduledJob
    {
        public string Name { get; }
        public int IntervalSeconds { get; }
        public Action Action { get; }
        public DateTime LastRun { get; private set; }

        public ScheduledJob(string name, int intervalSeconds, Action action)
        {
            Name = name;
            IntervalSeconds = intervalSeconds;
            Action = action;
            LastRun = DateTime.MinValue;
        }

        public bool IsDue() => (DateTime.UtcNow - LastRun).TotalSeconds >= IntervalSeconds;

        public void Run()
        {
            LastRun = DateTime.UtcNow;
            Action();
        }
    }

    public class CronScheduler
    {
        private readonly List<ScheduledJob> _jobs = new();
        private bool _running;

        public void Register(ScheduledJob job) => _jobs.Add(job);

        public async Task StartAsync(CancellationToken ct)
        {
            _running = true;
            Console.WriteLine("[Scheduler] Started. Polling every 1s...\n");
            while (_running && !ct.IsCancellationRequested)
            {
                foreach (var job in _jobs)
                {
                    if (job.IsDue())
                    {
                        Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Running: {job.Name}");
                        job.Run();
                    }
                }
                await Task.Delay(1000, ct);
            }
        }
    }

    class Program
    {
        static async Task Main()
        {
            var scheduler = new CronScheduler();

            scheduler.Register(new ScheduledJob("HealthCheck", 2, () =>
                Console.WriteLine("  -> Health check passed.")));

            scheduler.Register(new ScheduledJob("MetricsFlush", 4, () =>
                Console.WriteLine("  -> Metrics flushed to time-series DB.")));

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await scheduler.StartAsync(cts.Token);
            Console.WriteLine("\n[Scheduler] Stopped.");
        }
    }
}
