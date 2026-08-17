using ERP.DbMigrator._2026._08;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.DbMigrator
{
    public static class DbMigratorExtensions
    {
        public static IServiceCollection AddDbMigrator(this IServiceCollection services, string connectionString)
        {
            services.AddFluentMigratorCore()
                    .ConfigureRunner(runner => 
                    {
                        runner.AddSqlServer().WithGlobalConnectionString(connectionString).ScanIn(typeof(Users_20260817001).Assembly).For.Migrations();
                    });
            return services;
        }
    }
}
