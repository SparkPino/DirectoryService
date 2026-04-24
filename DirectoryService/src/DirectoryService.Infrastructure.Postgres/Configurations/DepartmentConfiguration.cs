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

        builder.HasIndex(d => new { d.IsActive, d.Name })
            .HasDatabaseName("ix_department_name");

        builder.Property(d => d.Id)
            .IsRequired()
            .HasColumnName("id");


        builder
            .HasMany(d => d.ChildDepartments)
            .WithOne()
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("name")
            .HasConversion(dn => dn.Name, dn => DepartmentName.FromDb(dn));

        builder.Property(d => d.Identifier)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("identifier")
            .HasConversion(di => di.Identifier, s => DepartmentIndentifier.FromDb(s));

        builder.Property(d => d.Path)
            .IsRequired()
            .HasColumnName("path")
            .HasConversion(dp => dp.Path, s => DepartmentPath.FromDb(s));

        builder.Property(d => d.Depth)
            .IsRequired()
            .HasColumnName("depth");

        builder.Property(d => d.ParentId)
            .HasColumnName("parent_id")
            .IsRequired(false);

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

        builder.HasQueryFilter(d => d.IsActive);
    }
}