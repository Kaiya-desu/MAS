using MAS_MP1.Database;
using MAS_MP1.Enums;
using MAS_MP1.Person;

namespace MAS_MP1.Product;

[Serializable]
public class UserScore
{
    // do tego potem bedzie GUI:
    /*
     * LOGOWANIE SIE CLIENTA
     * MOZLIWOSC DODANIA KOMENTARZA / OCENY -> WIDOCZNY PRZYCISK PRZY DANYCH GRY
     * WYSLANIE FORMULARZA
     *      INFORMACJA O DODANIU, DANE POJAWIAJĄ SIE W BAZIE
     *      GRACZ JUZ DODAŁ TA OCENE -> KOMUNIKAT O BŁĘDZIE
     */
    // musi byc cycus glancus
    
    
    // dodac do bazy:
    public DateTime CommentDate;
    
    public Client Client;
    public Game Game;
    public int IDGame;
    public Score Score { get; set; }
    public string Comment { get; set; }
    
   
    // Kompozycja - User i UserScore, Game i User Score, User score nie moze istniec bez Usera i Gry
    // uzytkownik client dodaje score i comment dla danej gry
    // dany klient moze tylko raz ocenić daną gre, 
    // do powstania UserScore niezbędna jest gra i klient
    
    private UserScore(Client client, Game game, Score score, string comment)
    {
        CommentDate = DateTime.Now; // data tworzenia obiektu 
        Client = client;
        Game = game;
        Score = score;
        CheckComment(comment); //Comment = comment;

        AddScoreToDB();
    }
    
    private void CheckComment(string comment)
    {
        Comment = comment.Length > 100 ? comment.Substring(0, 49) : comment;
    }
    
    public override string ToString()
    {
        var c = Client is null ? "No User" : Client.Login;
        var g = Game is null ? "No Game" : Game.Name;
        return "Client " + c + " Game " + g + " Comment: " + Comment + " Score: " + Score;
    }
    
    public void AddScoreToDB()
    {
        Object value = Convert.ChangeType(Score, Score.GetTypeCode());
        IDGame = Game.GetGameByName(Game.Name);
        Console.WriteLine("**** " + IDGame);
        Connection.Insert($"INSERT INTO UserScore (Game_ID_Game, Client_Login, Score, Comment, Comment_Date) VALUES ({IDGame}, '{Client.Login}', '{value}', '{Comment}', '{CommentDate}')");
    }
    
    // dodanie do DB 
    public static UserScore AddNewScore(Client client, Game game, Score score, string comment)
    {
        // całością jest Klient i Gra - bez tego nie powstanie ocena z komentarzem
        if(Client.CheckLogin(client.Login) == 0 || Game.GetGameByName(game.Name) == 0)
        {
            Console.WriteLine("Wrong User or Game");
            // throw new Exception("Nie ma takiego uzytkownika!");
        }

        if (CheckIfUserScoredGame(client, game) == 0)
        {
            // tworzenie nowej czesci, dodanie do db
            UserScore part = new UserScore(client, game, score, comment);
    
            // dodanie do calosci
            client.ClientScores();
            game.CalculateScore();
            game.GameComments();
 
            return part;
        }
        Console.WriteLine("This user already added score to this game!");
        return null;
    }
    
    // kasowanie oceny z db
    public static void DeleteScore(UserScore us)
    {
        Connection.Delete($"DELETE From UserScore WHERE Login = '{us.Client.Login}' AND Game_ID_Game = {us.IDGame}");
        us.Client = null;
        us.Comment = null;
        us.Score = Score.NoScore;
    }

    // jeden uzytkownik NIE MOZE dwa razy wystawic oceny tej samej grze -> dlatego sprawzamy kombinacje login i id gry
    public static int CheckIfUserScoredGame(Client c, Game g)
    {
        var IDGame = Game.GetGameByName(g.Name);
        var reader = Connection.Select($"SELECT ID_Score FROM UserScore WHERE Client_Login = '{c.Login}' AND Game_ID_Game = '{IDGame}'");
        var idScore = 0;
        while (reader.Read())
        {
            idScore = Convert.ToInt32(reader["ID_Score"]);
        }
        return idScore;
    }
    
}

// klasa pomocnicza (jako że konstruktor UserScore jest prywatny, to w celu stworzenia obiektów tej klasy
// aby wygodnie przetrzymywać dane z DB w liscie w programie C# zrobiłam tą klase do wyświetlania info
public class UserScoreData
{
    
    public string Login;
    public string GameName;
    public Score Score { get; set; }
    public string Comment { get; set; }
    public DateTime CommentDate;

    public UserScoreData(string login, string game, Score score, string comment, DateTime commentDate)
    {
        Login = login;
        GameName = game;
        Score = score;
        Comment = comment;
        CommentDate = commentDate;
    }

    public UserScoreData()
    {
    }
    
    public override string ToString()
    {
        return "User " + Login + " for a Game :" + "\n + Comment date: " + CommentDate + " Comment: " + Comment + " Score: " + Score ;
    }
}