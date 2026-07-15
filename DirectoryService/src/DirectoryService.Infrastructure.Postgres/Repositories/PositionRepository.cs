using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Positions.Failures;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly DirectoryServiceDbContext _context;
    private readonly ILogger<PositionRepository> _logger;

    public PositionRepository(DirectoryServiceDbContext context, ILogger<PositionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> AddAsync(Position position, CancellationToken cancellationToken)
    {
        await _context.Positions.AddAsync(position, cancellationToken);
        return position.Id.Id;
    }

    public async Task<Result<Position, Error>> GetByAsync(
        Expression<Func<Position, bool>> predicate,
        CancellationToken cancellationToken)
    {
        var position = await _context.Positions
            .FirstOrDefaultAsync(predicate, cancellationToken);
        if (position == null)
        {
            _logger.LogWarning("Должность не найдена");
            return Error.NotFound(null, "Должность не найдена");
        }

        return position;
    }

    public async Task<Result<Position, Error>> GetByIdAsync(Guid positionId, CancellationToken cancellationToken)
    {
        var correctId = new PositionId(positionId);

        var position = await _context.Positions
            .FirstOrDefaultAsync(p => p.Id == correctId, cancellationToken);

        if (position == null)
        {
            _logger.LogWarning("Должность не найдена Id:{PositionId}", positionId);
            return PositionErrors.NotFound(positionId);
        }

        return position;
    }

    public async Task<Result<Unit, Error>> DeleteByIdAsync(Guid positionId, CancellationToken cancellationToken)
    {
        var correctId = new PositionId(positionId);

        var position = await _context.Positions
            .FirstOrDefaultAsync(p => p.Id == correctId, cancellationToken);

        if (position == null)
        {
            _logger.LogWarning("Должность с Id:{PositionId} не найдена при удалении", positionId);
            return PositionErrors.NotFound(positionId);
        }

        _context.Positions.Remove(position);

        return Unit.Value;
    }
}