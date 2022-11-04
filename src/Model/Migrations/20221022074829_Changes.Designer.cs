﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Model;

#nullable disable

namespace Model.Migrations
{
    [DbContext(typeof(DispatcherContext))]
    [Migration("20221022074829_Changes")]
    partial class Changes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.10");

            modelBuilder.Entity("Model.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Constraints")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("HomeRegion")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ResultsUri")
                        .HasColumnType("TEXT");

                    b.Property<int?>("RoutingDecision")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("JobId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("Model.JobStateChange", b =>
                {
                    b.Property<int>("JobStateChangeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("JobId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StateChangedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("WorkerName")
                        .HasColumnType("TEXT");

                    b.HasKey("JobStateChangeId");

                    b.HasIndex("JobId");

                    b.HasIndex("WorkerName");

                    b.ToTable("JobStateChangeHistory");
                });

            modelBuilder.Entity("Model.Worker", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Region")
                        .HasColumnType("INTEGER");

                    b.HasKey("Name");

                    b.ToTable("Workers");
                });

            modelBuilder.Entity("Model.JobStateChange", b =>
                {
                    b.HasOne("Model.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Worker", "Worker")
                        .WithMany()
                        .HasForeignKey("WorkerName");

                    b.Navigation("Job");

                    b.Navigation("Worker");
                });
#pragma warning restore 612, 618
        }
    }
}
