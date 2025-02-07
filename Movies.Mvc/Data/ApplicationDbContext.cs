using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Movies.Options;

namespace Movies.Mvc.Data;

public class ApplicationDbContext : IdentityDbContext
{
    private readonly DatabaseOptions _databaseOptions;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options, 
        DatabaseOptions databaseOptions) : base(options)
    {
        _databaseOptions = databaseOptions;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            NpgsqlConnectionStringBuilder builder = new();
            builder.Host = "localhost";
            builder.Port = 5432;
            builder.Database = "Movies";
            builder.Username = Environment.GetEnvironmentVariable("SQL_USR");
            builder.Password = Environment.GetEnvironmentVariable("SQL_PWD");
            optionsBuilder.UseNpgsql(builder.ConnectionString);
	}
    }
}
