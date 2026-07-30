using Npgsql;
namespace DirectoryService.Seeder;

public static class HidePassword
{
    public static string MaskPassword(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString) { Password = "****", };
        return builder.ConnectionString;
    }
}