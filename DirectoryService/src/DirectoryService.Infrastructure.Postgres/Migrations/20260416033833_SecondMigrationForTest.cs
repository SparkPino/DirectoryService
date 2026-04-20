using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigrationForTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "departments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_departments_LocationId",
                table: "departments",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_departments_locations_LocationId",
                table: "departments",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departments_locations_LocationId",
                table: "departments");

            migrationBuilder.DropIndex(
                name: "IX_departments_LocationId",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "departments");
        }
    }
}
