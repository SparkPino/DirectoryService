using DirectoryService.Domain;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.DepartmentPositions.ValueObjects;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
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
            .IsRequired()
            .HasConversion(
                id => id.Id,
                guid => new DepartmentPositionId(guid));

        builder.Property(d => d.DepartmentId)
            .IsRequired()
            .HasColumnName("department_id")
            .HasConversion(
                id => id.Id,
                guid => new DepartmentId(guid));

        builder.Property(d => d.PositionId)
            .IsRequired()
            .HasColumnName("position_id")
            .HasConversion(
                id => id.Id,
                guid => new PositionId(guid));

        builder.HasOne<Position>()
            .WithMany(p => p.DepartmentsPositions)
            .IsRequired()
            .HasForeignKey(d => d.PositionId);

        builder.HasOne<Department>()
            .WithMany(d => d.Positions)
            .IsRequired()
            .HasForeignKey(d => d.DepartmentId);
    }
}