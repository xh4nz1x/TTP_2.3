using Npgsql;

public class DBService
{
    private static NpgsqlConnection? _connection;
    
    private static string GetConnectionString()
    {
        return @"Host=66.151.40.179;Port=5432;Database=gr624_cheaan;Username=gr624_cheaan;Password=password";
    }
    
    public static NpgsqlConnection GetSqlConnection()
    {
        if (_connection is null)
        {
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();
        }
        
        return _connection;
    }
}
