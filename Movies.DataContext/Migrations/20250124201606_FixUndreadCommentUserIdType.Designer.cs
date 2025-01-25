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
    [Migration("20250124201606_FixUndreadCommentUserIdType")]
    partial class FixUndreadCommentUserIdType
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

            modelBuilder.Entity("Movies.EntityModels.UserCommentRead", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<int>("CommentId")
                        .HasColumnType("integer");

                    b.Property<bool>("Seen")
                        .HasColumnType("boolean");

                    b.HasKey("UserId", "CommentId");

                    b.ToTable("UserCommentReads");
                });

            modelBuilder.Entity("Movies.EntityModels.UserVoteDate", b =>
                {
                    b.Property<int>("UserVoteDateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserVoteDateId"));

                    b.Property<int>("DateId")
                        .HasColumnType("integer");

                    b.Property<bool>("HasVoted")
                        .HasColumnType("boolean");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("VoteDateId")
                        .HasColumnType("integer");

                    b.HasKey("UserVoteDateId");

                    b.HasIndex("VoteDateId");

                    b.HasIndex("UserId", "DateId")
                        .IsUnique();

                    b.ToTable("UserVotesDate");
                });

            modelBuilder.Entity("Movies.EntityModels.UserVoteMovie", b =>
                {
                    b.Property<int>("UserVoteMovieId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserVoteMovieId"));

                    b.Property<bool>("HasVoted")
                        .HasColumnType("boolean");

                    b.Property<int>("MovieId")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserVoteMovieId");

                    b.HasIndex("UserId", "MovieId")
                        .IsUnique();

                    b.ToTable("UserVotesMovie");
                });

            modelBuilder.Entity("Movies.EntityModels.VoteDate", b =>
                {
                    b.Property<int>("VoteDateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("VoteDateId"));

                    b.Property<DateTime>("ProposedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Votes")
                        .HasColumnType("integer");

                    b.HasKey("VoteDateId");

                    b.ToTable("Dates");
                });

            modelBuilder.Entity("Movies.EntityModels.Comment", b =>
                {
                    b.HasOne("Movies.EntityModels.Movie", null)
                        .WithMany("Comments")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Movies.EntityModels.UserVoteDate", b =>
                {
                    b.HasOne("Movies.EntityModels.VoteDate", null)
                        .WithMany("UserVotes")
                        .HasForeignKey("VoteDateId");
                });

            modelBuilder.Entity("Movies.EntityModels.Movie", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("Movies.EntityModels.VoteDate", b =>
                {
                    b.Navigation("UserVotes");
                });
#pragma warning restore 612, 618
        }
    }
}
