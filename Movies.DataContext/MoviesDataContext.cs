﻿using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Movies.EntityModels;

public partial class MoviesDataContext : DbContext
{
    public MoviesDataContext() { }

    public MoviesDataContext(DbContextOptions<MoviesDataContext> options) : base(options)
    {
    }
    
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Comment> Comments { get; set; }

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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasData(
            new Movie
            {
                MovieId = 1,
                Title = "Inception",
                Director = "Christopher Nolan",
                Description = "A thriller about dreams",
                ReleaseDate = new DateOnly(2010, 7, 16),
                Seen = true,
                ImagePath = "inception.jpg"
            },
            new Movie
            {   
                MovieId = 2,
                Title = "The Matrix",
                Director = "The Wachowskis",
                Description = "You know what it is",
                ReleaseDate = new DateOnly(1999,3,31),
                Seen = true,
                ImagePath = "matrix.jpg"
            }
        );
        base.OnModelCreating(modelBuilder);
    }

}   
