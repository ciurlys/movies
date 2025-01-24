﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Movies.EntityModels;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Movies.DataContext.Migrations
{
    [DbContext(typeof(MoviesDataContext))]
    [Migration("20250123204515_AddDateTableReally")]
    partial class AddDateTableReally
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Movies.EntityModels.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CommentId"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("MovieId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CommentId");

                    b.HasIndex("MovieId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Movies.EntityModels.Date", b =>
                {
                    b.Property<int>("DateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("DateId"));

                    b.Property<DateTime>("ProposedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Votes")
                        .HasColumnType("integer");

                    b.HasKey("DateId");

                    b.ToTable("Dates");
                });

            modelBuilder.Entity("Movies.EntityModels.Movie", b =>
                {
                    b.Property<int>("MovieId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MovieId"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Director")
                        .HasColumnType("text");

                    b.Property<string>("ImagePath")
                        .HasColumnType("text");

                    b.Property<bool>("IsParticipating")
                        .HasColumnType("boolean");

                    b.Property<DateOnly>("ReleaseDate")
                        .HasColumnType("date");

                    b.Property<bool>("Seen")
                        .HasColumnType("boolean");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("character varying(80)");

                    b.Property<int>("Votes")
                        .HasColumnType("integer");

                    b.HasKey("MovieId");

                    b.ToTable("Movies");

                    b.HasData(
                        new
                        {
                            MovieId = 1,
                            Description = "A thriller about dreams",
                            Director = "Christopher Nolan",
                            ImagePath = "inception.jpg",
                            IsParticipating = false,
                            ReleaseDate = new DateOnly(2010, 7, 16),
                            Seen = true,
                            Title = "Inception",
                            Votes = 0
                        },
                        new
                        {
                            MovieId = 2,
                            Description = "You know what it is",
                            Director = "The Wachowskis",
                            ImagePath = "matrix.jpg",
                            IsParticipating = false,
                            ReleaseDate = new DateOnly(1999, 3, 31),
                            Seen = true,
                            Title = "The Matrix",
                            Votes = 0
                        });
                });

            modelBuilder.Entity("Movies.EntityModels.Comment", b =>
                {
                    b.HasOne("Movies.EntityModels.Movie", null)
                        .WithMany("Comments")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Movies.EntityModels.Movie", b =>
                {
                    b.Navigation("Comments");
                });
#pragma warning restore 612, 618
        }
    }
}
