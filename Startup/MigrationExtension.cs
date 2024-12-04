using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EStore.Services
{
    public class MigrationExtension
    {
        private readonly IServiceProvider _serviceProvider;

        public MigrationExtension(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IMigrationRunner ConfigureMigrations(string connectionString)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var builder = scope.ServiceProvider.GetRequiredService<IMigrationRunnerBuilder>();
                builder.WithGlobalConnectionString(connectionString);
                builder.ScanIn(AppDomain.CurrentDomain.GetAssemblies()).For.Migrations();

                return scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            }
        }

        public void RunMigrations()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                try
                {
                    // Run migrations up (to the latest)
                    runner.MigrateUp();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<MigrationExtension>>();
                    logger.LogError(ex, "An error occurred while running migrations.");
                }
            }
        }
    }
}
