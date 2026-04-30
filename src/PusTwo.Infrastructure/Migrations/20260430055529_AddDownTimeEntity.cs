using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PusTwo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDownTimeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DownTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Machine = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WorkCentre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    JobNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StockCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DowntimeMinutes = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DownTimes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DownTimes_EntryDate",
                table: "DownTimes",
                column: "EntryDate");

            migrationBuilder.CreateIndex(
                name: "IX_DownTimes_JobNumber",
                table: "DownTimes",
                column: "JobNumber");

            migrationBuilder.CreateIndex(
                name: "IX_DownTimes_Machine",
                table: "DownTimes",
                column: "Machine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DownTimes");
        }
    }
}
