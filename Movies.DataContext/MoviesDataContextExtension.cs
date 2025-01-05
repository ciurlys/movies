using Npgsql;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Movies.EntityModels;

public static class MoviesDataContextExtension
{
   public static IServiceCollection AddMoviesDataContext(this IServiceCollection services, string? connectionString = null)
   {
        if (connectionString is null)
        {
            NpgsqlConnectionStringBuilder builder = new();
            builder.Host = "localhost";
            builder.Port = 5432;
            builder.Database = "Movies";
            builder.Username = Environment.GetEnvironmentVariable("SQL_USR");
            builder.Password = Environment.GetEnvironmentVariable("SQL_PWD");
            connectionString = builder.ConnectionString; 
        }

        services.AddDbContext<MoviesDataContext>(options =>{
            options.UseNpgsql(connectionString);
        });
        return services;
   } 
}