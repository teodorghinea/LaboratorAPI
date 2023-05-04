using NCrontab;

namespace Project.Workers
{
    public abstract class BaseBackgroundWorker : BackgroundService
    {
        protected string ServiceName { get; set; }
        protected string CronExpression { get { return cronExpression; } set { cronExpression = value; UpdateCronExpression(); } }
        private string cronExpression;

        protected IServiceScopeFactory ScopeFactory { get; set; }
        protected CrontabSchedule scheduler { get; set; }
        protected DateTime nextRun { get; set; }

        public BaseBackgroundWorker(IServiceScopeFactory scopeFactory, string serviceName)
        {
            ScopeFactory = scopeFactory;
            ServiceName = serviceName;
        }

        public BaseBackgroundWorker(IServiceScopeFactory scopeFactory, string serviceName, string cronExpression)
        {
            this.cronExpression = cronExpression;
            scheduler = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            nextRun = scheduler.GetNextOccurrence(DateTime.UtcNow);

            ScopeFactory = scopeFactory;
            ServiceName = serviceName;
        }

        protected abstract void RunIteration();

        protected virtual void Warmup()
        {

        }

        protected virtual void HandleIterationError()
        {

        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{ServiceName} is running");

            return Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (DateTime.UtcNow > nextRun)
                    {
                        Warmup();

                        try
                        {
                            RunIteration();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"{ServiceName} unexpected exception");
                            HandleIterationError();
                        }

                        nextRun = scheduler.GetNextOccurrence(DateTime.UtcNow);
                    }

                    Thread.Sleep(1000);
                }

            }, cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{ServiceName} is stopping");

            return Task.Run(() => { });
        }

        private void UpdateCronExpression()
        {
            scheduler = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            nextRun = scheduler.GetNextOccurrence(DateTime.UtcNow);
        }
    }
}
