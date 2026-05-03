using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id)
            .HasName("pk_location");

        builder.HasIndex(l => new { l.IsActive, l.Name })
            .HasDatabaseName("ix_location_name_is_active");

        builder.HasIndex(l => l.Name)
            .IsUnique();

        builder.Property(l => l.Id)
            .HasColumnName("id")
            .IsRequired()
            .HasConversion(
                id => id.Id,
                guid => new LocationId(guid));

        builder.Property(l => l.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasConversion(
                name => name.Name,
                value => LocationName.FromDb(value));


        builder.OwnsOne(l => l.Address, lb =>
        {
            lb.ToJson("addresses");

            lb.Property(a => a.Country)
                .HasColumnName("country")
                .IsRequired();

            lb.Property(a => a.City)
                .HasColumnName("city")
                .IsRequired();

            lb.Property(a => a.Street)
                .HasColumnName("street")
                .IsRequired();

            lb.Property(a => a.PostalCode)
                .HasColumnName("postal_code")
                .IsRequired();

            lb.Property(a => a.BuildingNumber)
                .HasColumnName("building_number")
                .IsRequired();

            lb.Property(a => a.Apartment)
                .HasColumnName("apartment")
                .IsRequired(false);
        });

        builder.ComplexProperty(l => l.TimeZone, tzb =>
        {
            tzb.Property(tz => tz.TimeZone)
                .HasColumnName("timezone")
                .IsRequired();
        });

        builder.Property(l => l.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.HasQueryFilter(d => d.IsActive);
    }
}