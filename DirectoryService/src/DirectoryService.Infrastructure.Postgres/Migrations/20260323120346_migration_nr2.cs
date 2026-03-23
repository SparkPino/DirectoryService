using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class migration_nr2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_department_positions_positions_PositionId1",
                table: "department_positions");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_location_locations_LocationId1",
                table: "departments_location");

            migrationBuilder.DropPrimaryKey(
                name: "pk_departments_location",
                table: "departments_location");

            migrationBuilder.DropIndex(
                name: "IX_departments_location_LocationId1",
                table: "departments_location");

            migrationBuilder.DropPrimaryKey(
                name: "pk_position",
                table: "department_positions");

            migrationBuilder.DropIndex(
                name: "IX_department_positions_PositionId1",
                table: "department_positions");

            migrationBuilder.DropColumn(
                name: "LocationId1",
                table: "departments_location");

            migrationBuilder.DropColumn(
                name: "PositionId1",
                table: "department_positions");

            migrationBuilder.AddPrimaryKey(
                name: "pk_department_locations",
                table: "departments_location",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_department_positions",
                table: "department_positions",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_department_locations",
                table: "departments_location");

            migrationBuilder.DropPrimaryKey(
                name: "pk_department_positions",
                table: "department_positions");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId1",
                table: "departments_location",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PositionId1",
                table: "department_positions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_departments_location",
                table: "departments_location",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_position",
                table: "department_positions",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_departments_location_LocationId1",
                table: "departments_location",
                column: "LocationId1");

            migrationBuilder.CreateIndex(
                name: "IX_department_positions_PositionId1",
                table: "department_positions",
                column: "PositionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_department_positions_positions_PositionId1",
                table: "department_positions",
                column: "PositionId1",
                principalTable: "positions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_departments_location_locations_LocationId1",
                table: "departments_location",
                column: "LocationId1",
                principalTable: "locations",
                principalColumn: "id");
        }
    }
}
