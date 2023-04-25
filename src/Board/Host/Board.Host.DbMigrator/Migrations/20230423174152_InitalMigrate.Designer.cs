﻿// <auto-generated />
using System;
using Board.Host.DbMigrator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Board.Host.DbMigrator.Migrations
{
    [DbContext(typeof(MigrationDbContext))]
    [Migration("20230423174152_InitalMigrate")]
    partial class InitalMigrate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Board.Domain.Advert", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<bool>("isActive")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Adverts", (string)null);
                });

            modelBuilder.Entity("Board.Domain.AdvertImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AdvertId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uuid");

                    b.Property<bool>("isActive")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("AdvertId");

                    b.ToTable("AdvertImages", (string)null);
                });

            modelBuilder.Entity("Board.Domain.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.Property<bool>("isActive")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Categories", (string)null);
                });

            modelBuilder.Entity("Board.Domain.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AdvertId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AdvertisementId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Rating")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<bool>("isActive")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("AdvertId");

                    b.ToTable("Comments", (string)null);
                });

            modelBuilder.Entity("Board.Domain.Advert", b =>
                {
                    b.HasOne("Board.Domain.Category", "Category")
                        .WithMany("Adverts")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Board.Domain.AdvertImage", b =>
                {
                    b.HasOne("Board.Domain.Advert", "Advert")
                        .WithMany("AdvertImages")
                        .HasForeignKey("AdvertId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Advert");
                });

            modelBuilder.Entity("Board.Domain.Category", b =>
                {
                    b.HasOne("Board.Domain.Category", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Board.Domain.Comment", b =>
                {
                    b.HasOne("Board.Domain.Advert", "Advert")
                        .WithMany()
                        .HasForeignKey("AdvertId");

                    b.Navigation("Advert");
                });

            modelBuilder.Entity("Board.Domain.Advert", b =>
                {
                    b.Navigation("AdvertImages");
                });

            modelBuilder.Entity("Board.Domain.Category", b =>
                {
                    b.Navigation("Adverts");

                    b.Navigation("Children");
                });
#pragma warning restore 612, 618
        }
    }
}
