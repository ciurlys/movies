using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Movies.Mvc.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            NpgsqlConnectionStringBuilder builder = new();
            builder.Host = "localhost";
            builder.Port = 5432;
            builder.Database = "Users";
            builder.Username = Environment.GetEnvironmentVariable("SQL_USR");
            builder.Password = Environment.GetEnvironmentVariable("SQL_PWD");
            optionsBuilder.UseNpgsql(builder.ConnectionString);
        }
    }
}
