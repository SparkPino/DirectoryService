using DirectoryService.Domain;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id)
            .HasName("pk_department");

        // Для листинга без поиска: WHERE is_active = true ORDER BY name (GetDepartmentsHandler).
        // Второй аргумент HasIndex - имя индекса на уровне МОДЕЛИ (не путать с HasDatabaseName) -
        // без него EF считает оба HasIndex(d => d.Name) одним и тем же индексом (одинаковый список свойств)
        // и просто перезаписывает конфигурацию вместо создания двух разных индексов.
        builder.HasIndex(d => d.Name, "IX_department_name_sort")
            .HasDatabaseName("ix_department_name")
            .HasFilter("is_active = true");

        // Для ILIKE '%...%' поиска по имени (GetDepartmentsHandler.Search, SearchDepartmentTreeHandler).
        builder.HasIndex(d => d.Name, "IX_department_name_trgm")
            .HasDatabaseName("ix_department_name_trgm")
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");


        builder.Property(d => d.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                a => a.Id,
                a => new DepartmentId(a));

        builder
            .HasMany(d => d.ChildDepartments)
            .WithOne()
            .IsRequired(false)
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(d => d.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(n => n.Name)
            .HasColumnName("name")
            .HasMaxLength(DepartmentName.NAME_MAX_LENGTH)
            .IsRequired()
            .HasConversion(
                name => name.Value,
                value => DepartmentName.FromDb(value));

        builder.ComplexProperty(d => d.Identifier, nb =>
        {
            nb.Property(d => d.Identifier)
                .IsRequired()
                .HasMaxLength(DepartmentIdentifier.IDENTIFIER_MAX_LENGTH)
                .HasColumnName("identifier");
        });

        builder.OwnsOne(d => d.Path, pb =>
        {
            pb.Property(d => d.Path)
                .IsRequired()
                .HasColumnName("path")
                .HasColumnType("ltree");

            pb.HasIndex(d => d.Path)
                .HasMethod("gist")
                .HasDatabaseName("ix_department_path");
        });

        builder.Property(d => d.Depth)
            .IsRequired()
            .HasColumnName("depth");

        builder.Property(d => d.ParentId)
            .HasColumnName("parent_id")
            .IsRequired(false)
            .HasConversion(
                d => d == null ? (Guid?)null : d.Id,
                d => d == null ? null : new DepartmentId(d.Value));

        builder.Property(d => d.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(d => d.RowVersion)
            .IsRowVersion()
            .HasColumnName("xmin")
            .HasColumnType("xid");

        builder.HasQueryFilter(d => d.IsActive);
    }
}