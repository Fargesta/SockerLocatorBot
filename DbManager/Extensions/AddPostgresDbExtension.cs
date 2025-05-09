using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbManager.Extensions
{
    public static class AddPostgresDbExtension
    {
        public static IServiceCollection AddPostgresDb(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services.AddDbContextPool<PgContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("Postgres");
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("Connection string 'Postgres' is not configured.");

                options.UseNpgsql(connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.UseNetTopologySuite();
                        npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        npgsqlOptions.UseRelationalNulls();
                        npgsqlOptions.MigrationsAssembly(typeof(PgContext).Assembly.GetName().Name);
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorCodesToAdd: null);

                    }).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); ;
            });
            
            return services;
        }
    }
}
