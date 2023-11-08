using MAS_MP1.Enums;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using MAS_MP1.Database;


namespace MAS_MP1.Product;

[Serializable]
public class Game
{
    // MP1 - ATRYBUTY
    // atrybuty złozony - DateOnly ReleaseDate 
    // atrybut opcjonalny = TrailerURL, komentarz
    // atrybut powtarzalny - np lista języków, lista extra info etc....
    // atrybut klasowy - promo - czasem jest promocja na wszystkie gry 
    // atrybut pochodny - obliczenie oceny -> pobarnie danych z DB i wyliczenie
    
    // MP2 - ASOCJACJE
    //„Zwykła”  - np lista gatunków w klasie game | (gatunek) 1..* -> 0..* (game) |
    // atrybutem -  - ~ dla tabeli wiele do wiele - > klasa UserScore pomiedzy Klientem a Grą, atrybutem jest Score i Comment
    // Kwalifikowana - np hashset platform w klasie game (po Nazwie platformy mozna znalezc liste gier i na odwrót)
    // Kompozycja - Person i Addres, Paymen i Order, Client i ClientScore, np -> ClientScore nie moze istniec bez Clienta
    
    // MP3 - DZIEDZICZENIE 
    // wieloaspektowe - plec, roznica na podstawie plci w klasie Employee
    // dynamiczne -               <brak>
    // overlapping - w klasie Person, klient moze byc jednoczesnie pracownikiem i na odwrot
    // wielodziedziczenie -       <raczej odpada, duzo przerabiania>
    
    // MP4
    // unique - np nazwa w Genre / Platform - > nie moze byc powtórek
    // subset -                 <brak>
    // ordered - np posegregowanie po nazwie gry do  listy 
    // bag / history -           <raczej duplikaty mi nigdzie nie pasują w DB>
    // xor - cos albo co nie moze byc A jak jest B i na odwrót (pracownik Support lub Warehouse!) - pesel jest unikalny wiec jak juz jest pracownik w supporcie to nie mozna go dodac do Warehouse
    
    private static List<Game> _gameList = new List<Game>();
    public int IDGame { get; set; }
    public string Name { get; set; }
    private double Price { get; set; }
    public static double Promo { get; set; } = 0; // czasem moze byc procentowa znizka na wszystkie gry, 
    private string Studio { get; set; }
    private DateOnly ReleaseDate { get; set; }
    private string Description { get; set; }
    private double AverageScore { get; set; }

    public Dictionary<String, String> Comments { get; set; } = new Dictionary<String, String>();
    private string? TrailerURL { get; set; } // can be null
    private Availability Availability { get; set; }
    
    // wiele-do-wiele:
    private List<Genre> GenreList { get; set; }
    private List<Platform> PlatformList { get; set; }
    private List<Language> Languages { get; set; }
    private List<ExtraInfo> ExtraInfos { get; set; }
    
    // mp2 
    private Dictionary<String, Platform > PlatformQualif = new Dictionary<String, Platform>();
    
    public Game(List<Genre> genres, List<Platform> platforms, string name, double price, string studio, DateOnly releaseDate, List<Language> languages, 
                string description, List<ExtraInfo> extraInfo, string? trailerUrl, Availability availability)
    {
        Name = name;
        GenreList = genres; // dodanie do tabeli Game_Genre
        PlatformList = platforms; // dodanie do tabeli Game_Platforms
        PlatformListMethod(PlatformList); // dodanie do listy kwalifikowanej
        CheckPrice(price); // sprawdza czy cena OK
        Studio = studio;
        ReleaseDate = releaseDate;
        Languages = languages; // Dodanie do tabeli Game_Language
        Description = description;
        ExtraInfos = extraInfo;
        TrailerURL = trailerUrl;
        Availability = availability;
        
        if(GetGameByName(name) == 0)
        {
            AddGameToDB();
        }

    }
    private void AddGameToDB() {
        IDGame = Connection.Insert($"INSERT INTO Game (Name, Price, Studio, ReleaseDate, Description, Trailer, Availability) VALUES ('{Name}','{Price}','{Studio}', '{ReleaseDate}', '{Description}', '{TrailerURL}', '{Availability}')");

        // dodanie do tabeli game platform
        foreach(Platform p in PlatformList)
        {
            var ID_P = Platform.GetPlatformByName(p.Name);
            Connection.Insert($"Insert INTO Game_Platform (Game_ID_Game, Platform_ID_Platform) VALUES ({IDGame}, {ID_P})");
        }
        // dodanie do tabeli game genre
        foreach(Genre g in GenreList)
        {
            var ID_G = Genre.GetGenreByName(g.Name);
            Connection.Insert($"Insert INTO Game_Genre (Game_ID_Game, Genre_ID_Genre) VALUES ({IDGame}, {ID_G})");
        }
        // dodanie do tabeli game langauges
        foreach(Language l in Languages)
        {
            Connection.Insert($"Insert INTO Game_Languages (Game_ID_Game, Language) VALUES ({IDGame}, '{l}')");
        }
        // dodanie do tabeli game extra info 
        foreach(ExtraInfo e in ExtraInfos)
        {
            Connection.Insert($"Insert INTO Game_ExtraInfos (Game_ID_Game, ExtraInfo) VALUES ({IDGame}, '{e}')");
        }
        
    }
    
    public static void RemoveGame(string name) {
        // usuwanie ZE wszystkich tabeli asocjacyjnych:
        var ID =  GetGameByName(name);
        // usuwanie z do tabeli game platform
        Connection.Delete($"DELETE FROM Game_Platform WHERE Game_ID_Game = {ID}");

        // usuwanie z do tabeli game genre
        Connection.Delete($"DELETE FROM Game_Genre WHERE Game_ID_Game = {ID}");

        // v do tabeli game langauges
        Connection.Delete($"DELETE FROM Game_Languages WHERE Game_ID_Game = {ID}");

        // usuwanie z tabeli game extra info 
        Connection.Delete($"DELETE FROM Game_ExtraInfos WHERE Game_ID_Game = {ID}");

        // usuwanie z tabeli game score
        Connection.Delete($"DELETE FROM UserScore WHERE Game_ID_Game = {ID}");

        // i na koncu z Game....
        Connection.Delete($"DELETE FROM Game WHERE ID_Game = {ID}");

        GetAllGamesFromDB(); // wywołuje aby c# widział zmiany od razu
    }
    
    public static List<Game> GetAllGamesFromDB()
    {
        var reader = Connection.Select("SELECT * FROM Game");
        
        while (reader.Read())
        {
            var ID = Convert.ToInt32(Convert.ToString(reader["ID_Game"]));
            var name = Convert.ToString(reader["Name"]);
            var price = Convert.ToDouble(Convert.ToString(reader["Price"]));
            var studio = Convert.ToString(reader["Studio"]);
            var releaseDate_raw = Convert.ToString(reader["ReleaseDate"]); //Convert.ToDateTime(reader["Premiere"]);
            var releaseDate = DateOnly.FromDateTime(Convert.ToDateTime(releaseDate_raw));
            var description = Convert.ToString(reader["Description"]);
            var trailerUrl = Convert.ToString(reader["Trailer"]);
            var availability_raw = Convert.ToString(reader["Availability"]);
            var availability  = (Availability) Enum.Parse(typeof(Availability), availability_raw);

            // listy:
            // game platform
            var platformList = Platform.GetPlatformsForGame(ID);
            // game genre
            var genreList = Genre.GetGenresForGame(ID);
            // game langage
            var languages = GetLanguagesForGame(ID);
            // game extra info
            var extraInfo = GetExtraInfosForGame(ID);
            // game score
            // na razie bez tego
            
            _gameList.Add(new Game(genreList, platformList, name, price, studio, releaseDate, languages, description, extraInfo, trailerUrl, availability));
            _gameList[_gameList.Count - 1].IDGame = ID;
            _gameList[_gameList.Count - 1].CalculateScore(); // liczymy wynik 
            _gameList[_gameList.Count - 1].GameComments(); // dodajemy komentarze
        }

        _gameList = _gameList.OrderBy(x => x.Name).ToList();
        return _gameList;
    }

    public static List<Language> GetLanguagesForGame(int game_ID)
    {
        var reader = Connection.Select($"SELECT * FROM Game_Languages WHERE Game_ID_GAME = {game_ID}");
        var tmp_list = new List<Language>();
        while (reader.Read())
        {
            var language_raw = Convert.ToString(reader["Language"]);
            var language = (Language) Enum.Parse(typeof(Language), language_raw);
            tmp_list.Add(language);
        }
        return tmp_list;
    }
    
    public static List<ExtraInfo> GetExtraInfosForGame(int game_ID)
    {
        var reader = Connection.Select($"SELECT * FROM Game_ExtraInfos WHERE Game_ID_GAME = {game_ID}");
        var tmp_list = new List<ExtraInfo>();
        while (reader.Read())
        {
            var extrainfo_raw = Convert.ToString(reader["ExtraInfo"]);
            var extrainfo = (ExtraInfo) Enum.Parse(typeof(ExtraInfo), extrainfo_raw);
            tmp_list.Add(extrainfo);
        }
        return tmp_list;
    }
    
    public static int GetGameByName(string name)
    {
        var reader = Connection.Select($"SELECT ID_Game FROM Game WHERE Name = '{name}'");
        var x = 0;
        while (reader.Read()) 
        {
            x = Convert.ToInt32(reader["ID_Game"]);
        }

        return x;
    }
    
    public static Game GetGameByID(int id_game)
    {
        foreach (Game g in _gameList)
        {
            if (g.IDGame == id_game)
            {
                return g;
            }
        }
        return null;
    }


    public void CalculateScore()
    {
        // pobiera wartosci dla danego Game_Score i wylicza srednią
        var reader = Connection.Select($"SELECT IFNULL(AVG(Score), 0) AS AVGScore FROM UserScore WHERE Game_ID_Game = {IDGame}");
        while (reader.Read())
        {
            AverageScore = Convert.ToDouble(reader["AVGScore"]);
        }
        //Console.WriteLine(AverageScore);
    }
    
    public void GameComments()
    {
        var reader = Connection.Select($"SELECT Client_Login, Comment FROM UserScore WHERE Game_ID_Game = {IDGame}");
        while (reader.Read())
        {
            var login = Convert.ToString(reader["Client_Login"]);
            var comment = Convert.ToString(reader["Comment"]);

            if (!Comments.ContainsKey(login))
            {
                Comments.Add(login, comment);
            }
        }
        
    }

    public override string ToString()
    {
        var genresString = "";
        foreach(var genre in GenreList.Select((value,i)=>new{i,value}))
        {
            genresString += (genre.i != 0 ? "; " : "") + genre.value;
        }
        
        var platformsString = "";
        foreach(var platform in PlatformList.Select((value,i)=>new{i,value}))
        {
            platformsString += (platform.i != 0 ? "; " : "") + platform.value;
        }
        
        return "Name: " + Name + " Price: " + Price + " PLN," + " ReleaseDate " + ReleaseDate + " by: " + Studio +
               "\n" + "Genre: [" + genresString + "], Platforms: [" + platformsString + "] " +
               "\n" + "Languages: [" + String.Join("; ", Languages.ToArray()) + "], Extra info: [" +
               String.Join("; ", ExtraInfos.ToArray()) + "]" +
               "\n" + "Average user score: " + AverageScore + ", comments: " + String.Join(" ", Comments.ToArray()) +
               "\n" + " trailer: <" + TrailerURL + ">" +
               " Availability: " + Availability;
    }
    
    // mp2 

    // asocjacja kwalifikowana :
    public void PlatformListMethod(List<Platform> platf)
    {
        for (var i = 0 ; i < platf.Count; i++)
        {
            AddPlatformQualif(platf[i]);
        }
    }
    public void AddPlatformQualif(Platform platf) {
        // Check if we already have the info
        if (PlatformQualif == null)
        {
            PlatformQualif = new Dictionary<String, Platform>();
        }
        
        if(!PlatformQualif.ContainsKey(platf.Name)) {
            PlatformQualif.Add(platf.Name, platf);
 
            // Add the reverse connection
            platf.GamesList(this);
        }
    }
    public Platform FindPlatformQualif(String name) {
        // Check if we have the info
        if(!PlatformQualif.ContainsKey(name)) {
            //Console.WriteLine("Ta gra nie jest dostepna na platformie " + name);
            throw new Exception("Ta gra nie jest dostepna na platformie " + name);
            return null;
        }

        Console.WriteLine("Gra dostepna na platforme " + name);
        return PlatformQualif[name];
    }
    
    // ograniczenia
    public void CheckPrice(double price)
    {
        if (price <= 0)
        {
            throw new Exception("Cena nie moze byc mniejsza niz 0!");
        }
        else
        {
            Price = price;
        }
    }
}