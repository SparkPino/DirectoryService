using DirectoryService.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <summary>
    /// До MakeDeleteAtnullable колонка deleted_at была NOT NULL, и у живых записей там остался '-infinity'.
    /// Теперь "удалён" определяется через deleted_at IS NULL, поэтому приводим такие значения к NULL.
    /// </summary>
    [DbContext(typeof(DirectoryServiceDbContext))]
    [Migration("20261021120000_NormalizeDeletedAtAndDepartmentNameIndex")]
    public partial class NormalizeDeletedAtAndDepartmentNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE departments SET deleted_at = NULL WHERE deleted_at = '-infinity';");
            migrationBuilder.Sql("UPDATE locations SET deleted_at = NULL WHERE deleted_at = '-infinity';");
            migrationBuilder.Sql("UPDATE positions SET deleted_at = NULL WHERE deleted_at = '-infinity';");

            migrationBuilder.DropIndex(
                name: "ix_department_name",
                table: "departments");

            migrationBuilder.CreateIndex(
                name: "ix_department_name",
                table: "departments",
                column: "name",
                filter: "deleted_at IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_department_name",
                table: "departments");

            migrationBuilder.CreateIndex(
                name: "ix_department_name",
                table: "departments",
                column: "name",
                filter: "is_active = true");
        }
    }
}
