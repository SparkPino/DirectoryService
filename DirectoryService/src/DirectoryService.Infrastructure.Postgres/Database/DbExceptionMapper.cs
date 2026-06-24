using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Database;

internal static class DbExceptionMapper
{
    public static Error Map(Exception exception, ILogger logger)
    {
        switch (exception)
        {
            case DbUpdateConcurrencyException concurrencyException:
                logger.LogWarning(
                    concurrencyException,
                    "Конфликт параллельного изменения записи");
                return Error.Conflict(
                    "database.concurrency_conflict",
                    "Запись была изменена или удалена другим пользователем, повторите попытку");

            case DbUpdateException { InnerException: PostgresException postgresException }:
                return MapPostgresException(postgresException, logger);

            case PostgresException postgresException:
                return MapPostgresException(postgresException, logger);

            default:
                logger.LogError(exception, "Непредвиденная ошибка при обращении к базе данных");
                return Error.Failure("database.failure", "Произошла ошибка при сохранении данных");
        }
    }

    private static Error MapPostgresException(PostgresException exception, ILogger logger)
    {
        switch (exception.SqlState)
        {
            case PostgresErrorCodes.UniqueViolation:
                logger.LogWarning(
                    exception,
                    "Нарушение уникальности при сохранении. Constraint: {Constraint}",
                    exception.ConstraintName);
                return Error.Conflict(
                    "database.unique_violation",
                    "Запись с такими данными уже существует");

            case PostgresErrorCodes.ForeignKeyViolation:
                logger.LogWarning(
                    exception,
                    "Нарушение внешнего ключа при сохранении. Constraint: {Constraint}",
                    exception.ConstraintName);
                return Error.Conflict(
                    "database.foreign_key_violation",
                    "Операция невозможна из-за связанных записей");

            default:
                logger.LogError(
                    exception,
                    "Ошибка базы данных. SqlState: {SqlState}",
                    exception.SqlState);
                return Error.Failure("database.failure", "Произошла ошибка при сохранении данных");
        }
    }
}