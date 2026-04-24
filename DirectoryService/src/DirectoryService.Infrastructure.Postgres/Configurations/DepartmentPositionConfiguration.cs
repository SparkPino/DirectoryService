using DirectoryService.Domain;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_positions");

        builder.HasKey(d => d.DepartmentPositionId)
            .HasName("pk_department_positions");

        builder.HasIndex(d => new { d.DepartmentId, d.PositionId })
            .HasDatabaseName("ix_department_positions_department_id_position_id")
            .IsUnique();

        builder.Property(d => d.DepartmentPositionId)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(d => d.DepartmentId)
            .IsRequired()
            .HasColumnName("department_id");

        builder.Property(d => d.PositionId)
            .IsRequired()
            .HasColumnName("position_id");

        builder.HasOne<Position>(dp => dp.Position)
            .WithMany(p => p.DepartmentsPositions)
            .IsRequired()
            .HasForeignKey(d => d.PositionId);


        builder.HasOne<Department>(dp => dp.Department)
            .WithMany(d => d.DepartmentPositions)
            .IsRequired()
            .HasForeignKey(d => d.DepartmentId);
    }
}