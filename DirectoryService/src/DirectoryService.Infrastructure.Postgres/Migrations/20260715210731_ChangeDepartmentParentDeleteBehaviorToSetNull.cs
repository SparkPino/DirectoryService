using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDepartmentParentDeleteBehaviorToSetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departments_departments_parent_id",
                table: "departments");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "positions",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "locations",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "departments",
                newName: "deleted_at");

            migrationBuilder.AddForeignKey(
                name: "FK_departments_departments_parent_id",
                table: "departments",
                column: "parent_id",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departments_departments_parent_id",
                table: "departments");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "positions",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "locations",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "departments",
                newName: "DeletedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_departments_departments_parent_id",
                table: "departments",
                column: "parent_id",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
