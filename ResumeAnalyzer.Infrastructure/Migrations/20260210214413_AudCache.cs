using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeAnalyzer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AudCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VacancyId = table.Column<string>(type: "TEXT", nullable: false),
                    CollectionId = table.Column<string>(type: "TEXT", nullable: false),
                    ActionId = table.Column<string>(type: "TEXT", nullable: false),
                    NegotiationIds = table.Column<string>(type: "TEXT", nullable: false),
                    SuccessCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FailCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Details = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisAudits", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisAudits");
        }
    }
}
