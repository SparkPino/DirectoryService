using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class FinalFantasy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Реальное текущее состояние БД (проверено через pg_indexes):
            // "ix_department_name_trgm" - GIN+trgm, без фильтра - уже правильный, не трогаем.
            // "ix_department_name" - btree(is_active, name), без фильтра - устаревшая форма,
            // пересоздаём как btree(name) WHERE is_active = true.
            migrationBuilder.DropIndex(
                name: "ix_department_name",
                table: "departments");

            migrationBuilder.CreateIndex(
                name: "ix_department_name",
                table: "departments",
                column: "name",
                filter: "is_active = true");
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
                columns: new[] { "is_active", "name" });
        }
    }
}
