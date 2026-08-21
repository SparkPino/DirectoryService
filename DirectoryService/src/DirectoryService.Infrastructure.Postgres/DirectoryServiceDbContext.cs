using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres;

public class DirectoryServiceDbContext : DbContext, IReadDbContext
{
    public DirectoryServiceDbContext(DbContextOptions<DirectoryServiceDbContext> options)
        : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("ltree");
        modelBuilder.HasPostgresExtension("pg_trgm");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DirectoryServiceDbContext).Assembly);
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<DepartmentLocation> DepartmentLocations => Set<DepartmentLocation>();

    public DbSet<DepartmentPosition> DepartmentPositions => Set<DepartmentPosition>();

    public IQueryable<Location> ReadLocations => Set<Location>().AsQueryable().AsNoTracking();

    public IQueryable<Department> ReadDepartments => Set<Department>().AsQueryable().AsNoTracking();

    /*public IQueryable<Location> SearchLocationsByName(string search) => Set<Location>()
        .FromSqlInterpolated(
            // Алиасы нужны через баг EF Core: при композиции LINQ поверх FromSql
            // колонки complex/owned-свойств ищуться по именам по умолчанию,
            // а не по HasColumnName
            $"""
             SELECT *, timezone AS "TimeZone_TimeZone", addresses AS "Address"
             FROM locations
             WHERE name ILIKE {"%" + search + "%"}
             """)
        .AsNoTracking();*/
}