using DataLayer.Repositories;
using Core.Services;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using Project.Workers;

namespace Project.Settings
{
    public static class Dependencies
    {

        public static void Inject(WebApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Services.AddControllers();
            applicationBuilder.Services.AddSwaggerGen();

            applicationBuilder.Services.AddDbContext<AppDbContext>();
            applicationBuilder.Services.AddTransient<AppDbContext>();

            AddRepositories(applicationBuilder.Services);
            AddServices(applicationBuilder.Services);
            AddWorkers(applicationBuilder.Services);
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<StudentService>();
            services.AddTransient<AuthorizationService>();
            services.AddTransient<ClassService>();

            services.AddSingleton<BackgroundWorkerService>();
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddTransient<StudentsRepository>();
            services.AddTransient<ClassRepository>();
            services.AddTransient<UnitOfWork>();
        }

        public static void AddWorkers(this IServiceCollection services)
        {
            services.AddHostedService<TestBackgroundWorker>();
        }

    }
}
