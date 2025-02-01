using Npgsql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Movies.Options;


namespace Movies.EntityModels;

public static class MoviesDataContextExtension
{
    public static IServiceCollection AddMoviesDataContext(
	this IServiceCollection services,
	string? connectionString = null)
   {
       services.AddDbContext<MoviesDataContext>((serviceProvider, options) =>
       {
	   var databaseOptions = serviceProvider
	       .GetRequiredService<IOptions<DatabaseOptions>>().Value;
       
	   if (connectionString is null)
	   {
	       NpgsqlConnectionStringBuilder builder = new()
	       {
		   Host = "localhost",
		   Port = 5432,
		   Database = "Movies",
		   Username = databaseOptions.Username,
		   Password = databaseOptions.Password
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
