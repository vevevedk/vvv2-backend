using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Veveve.Domain.Database.Migrations
{
    public partial class AddJobQueueEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobQueue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobStatus = table.Column<int>(type: "integer", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    FeatureName = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobQueue_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobErrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<string>(type: "text", nullable: false),
                    RowDetails = table.Column<string>(type: "text", nullable: false),
                    ErrorCode = table.Column<string>(type: "text", nullable: false),
                    ExceptionDetails = table.Column<string>(type: "text", nullable: false),
                    JobQueueEntityId = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobErrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobErrors_JobQueue_JobQueueEntityId",
                        column: x => x.JobQueueEntityId,
                        principalTable: "JobQueue",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobErrors_JobQueueEntityId",
                table: "JobErrors",
                column: "JobQueueEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_JobQueue_ClientId",
                table: "JobQueue",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_JobQueue_FeatureName",
                table: "JobQueue",
                column: "FeatureName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobErrors");

            migrationBuilder.DropTable(
                name: "JobQueue");
        }
    }
}
