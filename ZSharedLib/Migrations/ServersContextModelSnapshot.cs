﻿// <auto-generated />
using System;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ZSharedLib.DBDrivers.PostgreSQLDBDriver;

namespace ZSharedLib.Migrations
{
    [DbContext(typeof(ServersContext))]
    partial class ServersContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ZSharedLib.DBDrivers.PostgreSQLDBDriver.DBFlag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ServerId");

                    b.Property<byte>("Type");

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("DBFlag");
                });

            modelBuilder.Entity("ZSharedLib.DBDrivers.PostgreSQLDBDriver.DBPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsBot");

                    b.Property<bool>("IsSpectator");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<short>("Ping");

                    b.Property<short>("PlayTime");

                    b.Property<short>("Score");

                    b.Property<long>("ServerId");

                    b.Property<short>("Team");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("DBPlayer");
                });

            modelBuilder.Entity("ZSharedLib.DBDrivers.PostgreSQLDBDriver.DBPWad", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<long>("ServerId");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("DBPWad");
                });

            modelBuilder.Entity("ZSharedLib.DBDrivers.PostgreSQLDBDriver.DBServer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<IPAddress>("Address")
                        .IsRequired();

                    b.Property<bool>("ForcePassword");

                    b.Property<byte>("GameType");

                    b.Property<string>("Iwad")
                        .IsRequired();

                    b.Property<DateTime>("LogTime");

                    b.Property<string>("Map")
                        .IsRequired();

                    b.Property<byte>("MaxClients");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<byte>("NumPlayers");

                    b.Property<short>("Ping");

                    b.Property<int>("Port");

                    b.Property<byte>("Skill");

                    b.Property<string>("Version")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ServerLogs");
                });

            modelBuilder.Entity("ZSharedLib.DBDrivers.PostgreSQLDBDriver.DBFlag", b =>
                {
                    b.HasOne("ZSharedLib.DBDrivers.PostgreSQLDBDriver.DBServer", "Server")
                        .WithMany("Flags")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ZSharedLib.DBDrivers.PostgreSQLDBDriver.DBPlayer", b =>
                {
                    b.HasOne("ZSharedLib.DBDrivers.PostgreSQLDBDriver.DBServer", "Server")
                        .WithMany("Players")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ZSharedLib.DBDrivers.PostgreSQLDBDriver.DBPWad", b =>
                {
                    b.HasOne("ZSharedLib.DBDrivers.PostgreSQLDBDriver.DBServer", "Server")
                        .WithMany("PWads")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
