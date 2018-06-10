using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ZSharedLib.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServerLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    LogTime = table.Column<DateTime>(nullable: false),
                    Address = table.Column<IPAddress>(nullable: false),
                    Port = table.Column<int>(nullable: false),
                    Ping = table.Column<short>(nullable: false),
                    Version = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Map = table.Column<string>(nullable: false),
                    MaxClients = table.Column<byte>(nullable: false),
                    ForcePassword = table.Column<bool>(nullable: false),
                    NumPlayers = table.Column<byte>(nullable: false),
                    Iwad = table.Column<string>(nullable: false),
                    Skill = table.Column<byte>(nullable: false),
                    GameType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DBFlag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Value = table.Column<int>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    ServerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBFlag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DBFlag_ServerLogs_ServerId",
                        column: x => x.ServerId,
                        principalTable: "ServerLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DBPlayer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Score = table.Column<short>(nullable: false),
                    Ping = table.Column<short>(nullable: false),
                    IsSpectator = table.Column<bool>(nullable: false),
                    IsBot = table.Column<bool>(nullable: false),
                    Team = table.Column<short>(nullable: false),
                    PlayTime = table.Column<short>(nullable: false),
                    ServerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBPlayer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DBPlayer_ServerLogs_ServerId",
                        column: x => x.ServerId,
                        principalTable: "ServerLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DBPWad",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    ServerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBPWad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DBPWad_ServerLogs_ServerId",
                        column: x => x.ServerId,
                        principalTable: "ServerLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DBFlag_ServerId",
                table: "DBFlag",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_DBPlayer_ServerId",
                table: "DBPlayer",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_DBPWad_ServerId",
                table: "DBPWad",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DBFlag");

            migrationBuilder.DropTable(
                name: "DBPlayer");

            migrationBuilder.DropTable(
                name: "DBPWad");

            migrationBuilder.DropTable(
                name: "ServerLogs");
        }
    }
}
