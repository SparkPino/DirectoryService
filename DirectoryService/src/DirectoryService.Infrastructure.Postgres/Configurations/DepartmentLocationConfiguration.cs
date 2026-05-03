using DirectoryService.Domain;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentLocations.ValueObjects;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("departments_location");

        builder.HasKey(d => d.DepartmentsLocationId)
            .HasName("pk_department_locations");

        builder.HasIndex(d => new { d.DepartmentId, d.LocationId })
            .HasDatabaseName("ix_department_locations_department_id_location_id")
            .IsUnique();

        builder.Property(d => d.DepartmentsLocationId)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                id => id.Id,
                guid => new DepartmentLocationId(guid));

        builder.Property(d => d.LocationId)
            .IsRequired()
            .HasColumnName("location_id")
            .HasConversion(
                id => id.Id,
                guid => new LocationId(guid));

        builder.Property(d => d.DepartmentId)
            .IsRequired()
            .HasColumnName("department_id")
            .HasConversion(
                id => id.Id,
                guid => new DepartmentId(guid));

        builder.HasOne<Location>()
            .WithMany(a => a.DepartmentsLocations)
            .HasForeignKey(d => d.LocationId);

        builder.HasOne<Department>()
            .WithMany(a => a.DepartmentsLocations)
            .IsRequired()
            .HasForeignKey(d => d.DepartmentId);
    }
}