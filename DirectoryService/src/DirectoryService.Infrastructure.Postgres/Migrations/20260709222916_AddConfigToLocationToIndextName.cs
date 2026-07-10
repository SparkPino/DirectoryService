using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigToLocationToIndextName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_locations_name",
                table: "locations");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "departments",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_locations_name",
                table: "locations",
                column: "name",
                unique: true,
                filter: "is_active = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_locations_name",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "departments");

            migrationBuilder.CreateIndex(
                name: "IX_locations_name",
                table: "locations",
                column: "name",
                unique: true);
        }
    }
}
