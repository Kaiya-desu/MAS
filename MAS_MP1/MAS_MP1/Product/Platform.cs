using MAS_MP1.Database;
using MAS_MP1.Enums;

namespace MAS_MP1.Product;

[Serializable]
public class Platform
{
    public string Name { get; set; }
    private Brand Brand { get; set; }
    private string Description { get; set; }

    private static List<Platform> _platformList = new List<Platform>();
    
    private static List<Game> _gamesList = new List<Game>();

    public Platform(string name, Brand brand, string description)
    {
        Name = name;
        Brand = brand;
        Description = CheckDescription(description);;

        if (GetPlatformByName(name) == 0)
        {
            AddPlatformToDB();
        }
    }
    
    public static List<Platform> GetAllPlatformsFromDB()
    {
        var reader = Connection.Select("SELECT * FROM Platform");
        
        while (reader.Read())
        {
            var name = Convert.ToString(reader["Name"]);
            var brand_raw = Convert.ToString(reader["Brand"]);
            var brand = (Brand) Enum.Parse(typeof(Brand), brand_raw);
            var description = Convert.ToString(reader["Description"]);
            if (GetPlatformByName(name) != 0)
            {
                _platformList.Add(new Platform(name, brand, description));
            }
        }   
        _platformList.Sort((x, y) => x.Brand.CompareTo(y.Brand));
        return _platformList;
    }
    
    public static List<Platform> GetPlatformsForGame(int game_ID)
    {
        var reader = Connection.Select($"SELECT * FROM Platform p INNER JOIN Game_Platform gp ON p.ID_Platform = gp.Platform_ID_Platform WHERE gp.Game_ID_GAME = {game_ID}");
        var tmp_list = new List<Platform>();
        while (reader.Read())
        {
            var name = Convert.ToString(reader["Name"]);
            var brand_raw = Convert.ToString(reader["Brand"]);
            var brand = (Brand) Enum.Parse(typeof(Brand), brand_raw);
            var description = Convert.ToString(reader["Description"]);
            tmp_list.Add(new Platform(name, brand, description));
        }
        return tmp_list;
    }
    
    private static string CheckDescription(string description)
    {
        return description.Length > 300 ? description.Substring(0, 299) : description;
    }
    
    private void AddPlatformToDB() {
        var id = Connection.Insert($"INSERT INTO Platform (Name, Brand, Description) VALUES ('{Name}','{Brand}','{Description}')");
    }
    
    public static int GetPlatformByName(string name)
    {
        var reader = Connection.Select($"SELECT ID_Platform FROM Platform WHERE Name = '{name}'");
        string x = "";
        while (reader.Read()) 
        {
            x = Convert.ToString(reader["ID_Platform"]);
        }
        if (x != "")
        {
            return Convert.ToInt32(x);
        }

        return 0;
    }

    public static void EditPlatform(int id, string name, Brand brand, string description)
    {
        description = CheckDescription(description);
        Connection.Edit($"UPDATE Platform SET Name = '{name}' Brand = '{brand}' Description = '{description}' WHERE ID_Platform = {id}");
    }
    
    public override string ToString()
    {
        return Name;
    }
    
    // MP2
    public void GamesList(Game game) 
    {
        if(!_gamesList.Contains(game)) {
            _gamesList.Add(game);
            game.AddPlatformQualif(this);
        }
        else
        {
            throw new Exception("Ta gra zostala juz dodana");
        }
    }
    

}