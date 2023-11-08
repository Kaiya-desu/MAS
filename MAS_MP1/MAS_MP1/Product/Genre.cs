using MAS_MP1.Database;

namespace MAS_MP1.Product;

[Serializable]
public class Genre
{ 
    public string Name { get; set; }
    public string Description { get; set; }

    private static List<Genre> _genreList = new List<Genre>();

    public static List<Genre> GetAllGenresFromDB()
    {
        var reader = Connection.Select("SELECT * FROM Genre");
        
        while (reader.Read())
        {
            var name = Convert.ToString(reader["Name"]);
            var description = Convert.ToString(reader["Description"]);
            if (GetGenreByName(name) != 0)
            {
                _genreList.Add(new Genre(name, description));
            }
        }
        _genreList.Sort((x, y) => x.Name.CompareTo(y.Name));
        return _genreList;
    }
    public Genre(string name, string description)
    {
        Name = name;
        Description = CheckDescription(description);

        if (GetGenreByName(name) == 0)
        {
            AddGenreToDB();
        }
    }
    
    private static string CheckDescription(string description)
    {
        return description.Length > 300 ? description.Substring(0, 299) : description;
    }
    
    // dodanie w czy poza konstruktorem?
    private void AddGenreToDB() 
    {
        var id = Connection.Insert(
                $"INSERT INTO Genre (Name, Description) VALUES ('{Name}','{Description}')");
            //Console.WriteLine("Genre " + Name + " Added, ID: = " + id);
    }

    public static int GetGenreByName(string name)
    {
        var reader = Connection.Select($"SELECT ID_Genre FROM Genre WHERE Name = '{name}'");
        string x = "";
        while (reader.Read()) 
        {
            x = Convert.ToString(reader["ID_Genre"]);
        }
        if (x != "")
        {
            return Convert.ToInt32(x);
        }

        return 0;
    }
   
    public static List<Genre> GetGenresForGame(int game_ID)
    {
        var reader = Connection.Select($"SELECT * FROM Genre g INNER JOIN Game_Genre gg ON g.ID_Genre = gg.Genre_ID_Genre WHERE gg.Game_ID_GAME = {game_ID}");
        var tmp_list = new List<Genre>();
        while (reader.Read())
        {
            var name = Convert.ToString(reader["Name"]);
            var description = Convert.ToString(reader["Description"]);
            tmp_list.Add(new Genre(name, description));
        }
        return tmp_list;
    }
    public override string ToString()
    {
        return Name;
    }

    public static void EditGenre(int id, string name, string description)
    {
        description  = CheckDescription(description);
        Connection.Edit($"UPDATE Genre SET Name = '{name}' Description = '{description}' WHERE ID_Genre = {id}");
    }
    
    // ograniczenia

}