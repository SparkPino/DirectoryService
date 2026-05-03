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
            .HasColumnName("id")
            .HasConversion(
                a => a.Id,
                a => new DepartmentId(a));

        builder
            .HasMany(d => d.ChildDepartments)
            .WithOne()
            .IsRequired(false)
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

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

        builder.ComplexProperty(d => d.Path, pb =>
        {
            pb.Property(d => d.Path)
                .IsRequired()
                .HasColumnName("path");
        });

        builder.Property(d => d.Depth)
            .IsRequired()
            .HasColumnName("depth");

        builder.Property(d => d.ParentId)
            .HasColumnName("parent_id")
            .IsRequired(false)
            .HasConversion(d => d!.Id, d => new DepartmentId(d));

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