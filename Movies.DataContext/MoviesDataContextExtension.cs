using Npgsql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace Movies.EntityModels;

public static class MoviesDataContextExtension
{
    public static IServiceCollection AddMoviesDataContext(
    this IServiceCollection services,
    string? connectionString = null)
    {
        services.AddDbContext<MoviesDataContext>((serviceProvider, options) =>
        {

            if (connectionString is null)
            {
                NpgsqlConnectionStringBuilder builder = new()
                {
                    Host = "localhost",
                    Port = 5432,
                    Database = "Movies",
                    Username = Environment.GetEnvironmentVariable("SQL_USR"),
                    Password = Environment.GetEnvironmentVariable("SQL_PWD")
                };

                connectionString = builder.ConnectionString;
            }

            options.UseNpgsql(connectionString);

            options.LogTo(MoviesDataContextLogger.WriteLine, new[]
               { Microsoft.EntityFrameworkCore
           .Diagnostics.RelationalEventId.CommandExecuting});
        });
        return services;
    }
}
