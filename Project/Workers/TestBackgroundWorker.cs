using Core.Services;

namespace Project.Workers
{
    public class TestBackgroundWorker : BaseBackgroundWorker
    {
        public TestBackgroundWorker(IServiceScopeFactory scopeFactory) : base(scopeFactory, "TEST_WORKER")
        {
            //every 10 minutes
            //CronExpression = "* 0/10 * * * *";

            //every 5 minutes
            //CronExpression = "* 0/5 * * * *";

            //every 1 hour
            //CronExpression = "0 0 */1 * * *";

            //every 30 seconds
            CronExpression = "0/30 * * * * *";
        }

        protected override void RunIteration()
        {
            Console.WriteLine($"{this.ServiceName} is executting - {DateTime.UtcNow}");

            using (var scope = ScopeFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<BackgroundWorkerService>();

                service.Run();
            }

            Console.WriteLine($"{ServiceName} is sleeping");
            Console.WriteLine();
        }
    }
}
