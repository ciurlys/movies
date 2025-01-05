

using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Movies.Mvc.Data;

public static class ApplicationDbContextExtension
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, string? connectionString = null)
    {
        if (connectionString is null)
        {
            NpgsqlConnectionStringBuilder builder = new();
            
            builder.Host = "localhost";
            builder.Port = 5432;
            builder.Database = "Users";
            builder.Username = Environment.GetEnvironmentVariable("SQL_USR");
            builder.Password = Environment.GetEnvironmentVariable("SQL_PWD");

            connectionString = builder.ConnectionString;
        }
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        }, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);

        return services;
    }
}