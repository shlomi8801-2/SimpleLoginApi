namespace BackendApi.Controllers;
using Microsoft.Data.Sqlite;

public static class DB
{
    public static string connectionString = "Data Source=localdatabase.db";
    public static SqliteConnection ConnectToDB(){
    SqliteConnection connection = new SqliteConnection(connectionString);
    connection.Open();
    return connection;
    }

        public static HashSet<string> GetColumns(string tableName, SqliteConnection? connection = null)
    {
        var result = new HashSet<string>();
        if (connection == null){connection = ConnectToDB();}
        
        var command = connection.CreateCommand();
        // command.CommandText = $"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{tableName}';";
        command.CommandText = $"PRAGMA table_info('{tableName}');";
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                result.Add(reader.GetString(1));
            }
        }
        connection.Close();
        return result;
    }

    public static HashSet<Dictionary<string, string?>> RunCMD(string CMD){
        var result = new HashSet<Dictionary<string, string?>>();
        var connection = ConnectToDB();
        var command = connection.CreateCommand();
        command.CommandText = CMD;

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var row = new Dictionary<string, string?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                        var columnValue = reader[i]?.ToString();
                        row[columnName] = columnValue;
                }
                
                    // result[reader[0].ToString()] = row; why the possible null refrence exception?
            var key = reader[0]?.ToString();
            if (key != null && row != null)
            {
                result.Add(row);
            }
            }
        }
        connection.Close();
        
        return result;
    }
        public static HashSet<Dictionary<string, string?>> GetTable(string tableName){
            
            return RunCMD($"SELECT * FROM {tableName};");
        }
    
    public static void RunCMDNoOut(string cmd)
        {
            var connection = ConnectToDB();
            var command = connection.CreateCommand();
            command.CommandText = cmd;
            command.ExecuteNonQuery();
            connection.Close();
        }
    
    public static void PrintTable(Dictionary<string, Dictionary<string, string>> table){
        //                        Dictionary<row, Dictionary<column, value>>
            var columns = table.First().Value.Keys;
        
        Console.WriteLine(string.Join(", ", columns));
        foreach (var row in table.Keys)
        {
            
            foreach (var column in table[row])
            {
                Console.Write(column.Value + " ");
            }
            Console.WriteLine();
        }
    }




} // class
