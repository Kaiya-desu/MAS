using System.Data.SQLite;

namespace MAS_MP1.Database;

public class Connection
{
    public static SQLiteConnection _SQLiteConnection { get; set; }

    public static SQLiteDataReader Select(string query)
    {
        var sqLiteCommand = _SQLiteConnection.CreateCommand();
        sqLiteCommand.CommandText = query;
        return sqLiteCommand.ExecuteReader();
    }
    
    public static int Insert(string query)
    {
        var sqLiteCommand = _SQLiteConnection.CreateCommand();
        sqLiteCommand.CommandText = query;
        sqLiteCommand.ExecuteNonQuery();
        
        // jakis komunikat?
        string? val = "";
        var reader = Select($"SELECT last_insert_rowid()");
        while (reader.Read())
        {
            val = Convert.ToString(reader["last_insert_rowid()"]);
        }
        Console.WriteLine("New record for query " + query + "\nID = " + val);
        
        return Convert.ToInt32(val);
    }
    
    public static void Edit(string query)
    {
        var sqLiteCommand = _SQLiteConnection.CreateCommand();
        sqLiteCommand.CommandText = query;
        sqLiteCommand.ExecuteNonQuery();
        // jakis komunikat?
        Console.WriteLine("Data edited! " + query);
    }
    
    public static void Delete(string query)
    {
        var sqLiteCommand = _SQLiteConnection.CreateCommand();
        sqLiteCommand.CommandText = query;
        sqLiteCommand.ExecuteNonQuery();
        // jakis komunikat?
        Console.WriteLine("Data deleted! " + query);
    }
    
    
}