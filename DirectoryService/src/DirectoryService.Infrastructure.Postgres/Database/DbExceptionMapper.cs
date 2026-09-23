using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Infrastructure.Postgres.Database;

internal static class DbExceptionMapper
{
    private static Dictionary<string, string> UniqueConstraintFields = new()
    {
        ["IX_locations_name"] = "LocationName",
    };

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
                    null,
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
                string? field = null;
                if (exception.ConstraintName is { } constraint)
                    UniqueConstraintFields.TryGetValue(constraint, out field);

                return Error.Conflict(
                    "database.unique_violation",
                    field,
                    "Запись с такими данными уже существует");

            case PostgresErrorCodes.ForeignKeyViolation:
                logger.LogWarning(
                    exception,
                    "Нарушение внешнего ключа при сохранении. Constraint: {Constraint}",
                    exception.ConstraintName);
                return Error.Conflict(
                    "database.foreign_key_violation",
                    null,
                    "Операция невозможна из-за связанных записей");

            case PostgresErrorCodes.RestrictViolation:
                logger.LogWarning(
                    exception,
                    "Нарушение RESTRICT-ограничения при удалении. Constraint: {Constraint}",
                    exception.ConstraintName);
                return Error.Conflict(
                    "database.restrict_violation",
                    null,
                    "Нельзя удалить объект, пока на него кто-то ссылается");

            case PostgresErrorCodes.DeadlockDetected:
                logger.LogWarning("Зафиксирована мертвая блокировка при выполнении транзакции");
                return Error.Conflict(
                    "database.deadlock_detected",
                    null,
                    "Попробуйте повторить операцию");

            default:
                logger.LogError(
                    exception,
                    "Ошибка базы данных. SqlState: {SqlState}",
                    exception.SqlState);
                return Error.Failure("database.failure", "Произошла ошибка при сохранении данных");
        }
    }
}