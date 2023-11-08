using System.Data;
using MAS_MP1.Database;
using MAS_MP1.Enums;
using MAS_MP1.Product;

namespace MAS_MP1.Person;

public class Client : Person
{
    public override int ID { get; set; }
    public override string Name { get; set; }
    public override string Surname { get; set; }
    public override DateOnly DateOfBirth { get; set; }
    public override int? PhoneNumber { get; set; }

    public string Login  { get; set; }
    public string Password  { get; set; }
    
    public bool BirthdayBonus = false;
    
    // lista dodanych ocen przez klienta
    private HashSet<UserScoreData> parts = new HashSet<UserScoreData>();
    
    public Client(string name, string surname, DateOnly birthDate, int phoneNumber, string login, string password) 
        : base(name, surname, birthDate, phoneNumber)
    {
        Login = login;
        Password = password;
        // sprawdzam czy dany Login jest juz w bazie
        if (CheckLogin(login) == 0)
        {
            // wiem ze moga byc dwie osoby o takim samym imieniu, nazwisku, dacie urodzenia, ale telefon jest raczej unique wiec
            // ustalam ze gdy te cztery rzeczy są takie same to wtedy nie dodajemy do bazy (uznaje za duplikat)
            AddClient();
        }
        else
        {
            ClientScores();
        }
    }

    public static int CheckLogin(string login)
    {
        var reader = Connection.Select($"SELECT Person_ID_Person FROM Client WHERE Login = '{login}'");
        int x = 0;
        while (reader.Read()) 
        {
            x = Convert.ToInt32(reader["Person_ID_Person"]);
        }
        return x;
    }
    
    // znajdujemy osobe po loginie i hasle i tworzymy obiekt Client
    public static Client LoginIn(string login, string password)
    {
        var reader1 = Connection.Select($"SELECT Login FROM Client WHERE Login = '{login}' AND Password = '{password}'");
        var logged  = "";
        while (reader1.Read())
        {
            logged = Convert.ToString(reader1["Login"]);
        }
        if (logged != "")
        {
            var reader = Connection.Select($"SELECT * FROM Client C INNER JOIN Person P on P.ID_Person = C.Person_ID_Person");
            while (reader.Read())
            {
                var name = Convert.ToString(reader["Name"]);
                var surname = Convert.ToString(reader["Surname"]);
                var birthdate_raw = Convert.ToString(reader["Birthdate"]);
                var birthdate = DateOnly.FromDateTime(Convert.ToDateTime(birthdate_raw));
                var phoneNumber = Convert.ToInt32(Convert.ToString(reader["PhoneNumber"]));
                
                Console.WriteLine("Logged in!");
                return new Client(name, surname, birthdate, phoneNumber, login, password);
            }
        }
        Console.WriteLine("Wrong data!");
        return null;
    }
    
    

    private void AddClient()
    {
        //Console.WriteLine(Name + " " + Surname + " " + DateOfBirth + " " + PhoneNumber);
        // bo nowy CLIENT mógłby być wczesniej w DB jako istniejący PERSON (np poprzez bycie pracownikiem)
        var id = GetPersonIDByName(Name, Surname, DateOfBirth, PhoneNumber);
        if (id == 0)
        {
            Connection.Insert(
                $"INSERT INTO Person (Name, Surname, Birthdate, PhoneNumber) VALUES ('{Name}','{Surname}','{DateOfBirth}', '{PhoneNumber}')");
        }
        
        if (id != 0)
        {
            Connection.Insert(
                $"INSERT INTO Client (Person_ID_Person, Login, Password) VALUES ({id},'{Login}','{Password}')"
            );
        }
    }
    
    // usuwanie ze wszystkich tabeli na raz bo mamy kompozycje
    public static void DeleteClient(string login)
    {
        Connection.Delete($"DELETE From UserScore WHERE Login = '{login}'");
        var qID = Connection.Select($"SELECT Question FROM Client WHERE Login = '{login}'");
        Connection.Delete($"DELETE FROM Question_Answer WHERE Question_ID_Question = {qID}");
        Connection.Delete($"DELETE FROM Question WHERE Login = '{login}'");
        Connection.Delete($"DELETE FROM Client WHERE Login = '{login}'");
    }

    // nabija sie wtedy znizka na zamówienie 10%
    public bool CheckBirthdayBonus()
    {
        var todaysDate = DateTime.Today;
        if (DateOfBirth.Month == todaysDate.Month && DateOfBirth.Day == todaysDate.Day)
        {
            BirthdayBonus = true;
        }
        else
        {
            BirthdayBonus = false;
        }
        return BirthdayBonus;
    }
    
    public override string ToString()
    {
        string info = "User: " + Login + "\n";
        foreach (UserScoreData part in parts)
        {
            info +=  part.GameName + " " + part.Score + " " + part.Comment + "\n";
        }
        return info;
    }

    
    public void ClientScores()
    {
        var reader = Connection.Select($"SELECT * FROM UserScore WHERE Client_Login = '{Login}'");
        var part = new UserScoreData();
        while (reader.Read())
        {
            var login = Convert.ToString(reader["Client_Login"]);
            var id_game = Convert.ToInt32(reader["Game_ID_Game"]);
            var score_raw = Convert.ToInt32(reader["Score"]);
            var score = (Score)score_raw;
            var comment = Convert.ToString(reader["Comment"]);
            var comment_date_raw = Convert.ToString(reader["Comment_Date"]);
            var comment_date = Convert.ToDateTime(comment_date_raw);

            var game = Game.GetGameByID(id_game);
            // bo nie utworze przy wczytaniu ponownie obiektu UserScore (jest prywatny)
            // aby uniknac powtórek (przy ponownym wczytywaniu danych), sprawdzam name
            if(!parts.Any(x => x.GameName.Contains(game.Name)))
                parts.Add(new UserScoreData(login, game.Name, score, comment, comment_date));
        }
    }


}