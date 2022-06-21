﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Orders.Infra.Database;

#nullable disable

namespace Orders.Infra.Migrations
{
    [DbContext(typeof(OrdersDbContext))]
    [Migration("20220620185037_migrationDb1")]
    partial class migrationDb1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Orders.Domain.Entities.OrderAggregateRoot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Items")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("created_by")
                        .HasColumnType("text");

                    b.Property<DateTime?>("created_datetime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("modified_by")
                        .HasColumnType("text");

                    b.Property<DateTime?>("modified_datetime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ruid")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("orders", "public");
                });

            modelBuilder.Entity("Orders.Infra.EntityConfigurations.IntegrationEventLogEntry", b =>
                {
                    b.Property<Guid>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EventTypeName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<int>("TimesSent")
                        .HasColumnType("integer");

                    b.Property<string>("TransactionId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("EventId");

                    b.ToTable("IntegrationEventLog", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
