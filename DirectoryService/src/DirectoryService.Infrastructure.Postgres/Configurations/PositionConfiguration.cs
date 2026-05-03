using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(p => p.Id)
            .HasName("pk_positions");

        builder.HasIndex(p => new { p.IsActive, p.Name })
            .HasDatabaseName("ix_position_name");

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .IsRequired()
            .HasConversion(
                id => id.Id,
                guid => new PositionId(guid));


        builder.Property(n => n.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(
                name => name.Name,
                value => PositionName.FromDb(value));


        builder.Property(p => p.Description)
            .HasColumnName("description")
            .IsRequired(false);

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.HasQueryFilter(d => d.IsActive);
    }
}