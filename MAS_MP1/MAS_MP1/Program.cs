using System.Data.Entity;
using System.Data.SQLite;
using MAS_MP1.Database;
using MAS_MP1.Enums;
using MAS_MP1.Order;
using MAS_MP1.Person;
using MAS_MP1.Product;

    // MP5  ( i projekt )

    var dir = "/Users/karolinastruzek/RiderProjects/MAS_MP1/MAS_MP1/Database/gameshop";
    SQLiteConnection sqLiteConnection = new SQLiteConnection("Data source = " + dir);
    try
    {
        sqLiteConnection.Open();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
    Connection._SQLiteConnection = sqLiteConnection;

    /*
    // kilka genre
    Genre genAction = new Genre("Action", "An action game is a video game genre that emphasizes physical challenges, including hand–eye coordination and reaction-time.");
    Genre genParty = new Genre("Party Game", "A multiplayer game, usually consisting of a series of short minigames, that can be easily played in a social setting.");
    Genre genRacing = new Genre("Racing Game", "Cars goes BRRRRR");

    // kilka platform
    Platform platSwitch = new Platform("Nintendo Switch", Brand.Nintendo, "good party console");
    Platform platPS5 = new Platform("PlayStation 5", Brand.Sony, "Newest sony console with amazing performance");
    Platform platXSX = new Platform("XBOX Series X", Brand.Microsoft, "Newest Microsoft console, looks like a fridge");
    */

      // listy:
    /*
       var g = Genre.GetAllGenresFromDB();
       var p = Platform.GetAllPlatformsFromDB();
  
      foreach (Genre gen in g) Console.WriteLine(gen.ToString());
      foreach (Platform pe in p) Console.WriteLine(pe.ToString());
      
    // kilka gier
    Game gameMarioKart = new Game(g.Where(x=>x.Name.ToLower().Contains("racing")||x.Name.ToLower().Contains("party")).ToList(),
        p.Where(x=>x.Name.ToLower().Contains("nintendo")).ToList(),
        "Mariokart 8", 199.00,"Nintendo", new DateOnly(2014, 5, 29), 
        new List<Language>() {Language.日本語, Language.Polski}, "Racing game", 
        new List<ExtraInfo>() {ExtraInfo.Kids, ExtraInfo.Multiplayer}, null, Availability.LowStock );
    
    Game gameEldenRing = new Game(g.Where(x=>x.Name.ToLower().Contains("action")).ToList(),
        p.Where(x=>x.Name.ToLower().Contains("playstation 5")).ToList(),
        "Elden Ring", 259.00,"FromSoftware", new DateOnly(2022, 2, 25), 
        new List<Language>() {Language.English}, "Dark souls game", 
        new List<ExtraInfo>() {ExtraInfo.Adult, ExtraInfo.Singleplayer}, null, Availability.InStock );

    Game gameITT = new Game(g.Where(x=>x.Name.ToLower().Contains("action")).ToList(),
        p.Where(x=>x.Name.ToLower().Contains("xbox series x")).ToList(),
        "It takes two", 159.00,"Hazelight", new DateOnly(2022, 2, 25), 
        new List<Language>() {Language.English}, "Dark souls game", 
        new List<ExtraInfo>() {ExtraInfo.Teen, ExtraInfo.Coop}, null, Availability.InStock );

    // foreach (Game game in games) Console.WriteLine(game.ToString());

    //Game.RemoveGame(gameMarioKart);

    // asocjacja kwalifikowana test:
    //gameMarioKart.FindPlatformQualif("Nintendo");
  */  
    // var games = Game.GetAllGamesFromDB();
    // foreach (Game game in games) Console.WriteLine(game.ToString());

    // dodanie klienta, dodanie adresu, edycja adresu.

    // dziedziczenie, tworzenie nowego klienta
    /*
    Client client = new Client("Michau", "Michajo",new DateOnly(2014, 5, 29), new HashSet<PersonType>{}, 123456778, "Michcio", "michaaa");
    Client client1 = new Client("Ania", "Zielona",new DateOnly(2010, 5, 29), new HashSet<PersonType>{}, 123456790, "Aniaaa", "zaq12wsx");

    Client client2 = new Client("Testowy", "Testo", new DateOnly(2010, 1, 29), new HashSet<PersonType>{}, 123456790, 
        "TestoPolak", "testo123");

    Person.AddAddressToDB(client, "Polska", "Warszawa", "Papieska 21", "37", "2137-420");   
    */
    
/*
    EmployeeSupportSpecialist employe4 = new EmployeeSupportSpecialist("Danuta", "Danio", 
    new DateOnly(2010, 1, 29), 123456111, new HashSet<PersonType>{}, Sex.Female, "Daniowska", 123456792, new DateOnly(2012, 1, 29), 
10.5f, 2/3f, new HashSet<Language>(){Language.Polski, Language.Français});

    EmployeeSupportSpecialist employe5 = new EmployeeSupportSpecialist("Danon", "Danio", 
new DateOnly(2010, 1, 29), 123457222,new HashSet<PersonType>{}, Sex.Male, null, 123456793, new DateOnly(2013, 1, 29), 
10.5f, 2/3f, new HashSet<Language>(){Language.Polski, Language.Français});
    
    EmployeeWarehouse employe6 = new EmployeeWarehouse("Pawel", "Jumper", 
        new DateOnly(2010, 1, 29), 123457230,new HashSet<PersonType>{}, Sex.Male, null, 1234523793, new DateOnly(2013, 1, 29), 
        10.5f, 2/3f, true);
*/
/*
    var x = Employee.EmployeesList();

    foreach (Employee e in x)
    {
        Console.WriteLine(e.ToString());
    }
    */

    /*
    Question q = new Question("TestoPolak", "Test pytan", Language.Français, "Testuje pytania");
    Question q2 = new Question("Michcio", "Test pytan 2", Language.日本語, "Testuje pytanie w inny jezyku");

    Question.AddEmployeeToTheQuestion(q2, x[0]);
    */

    // foreach (Game game in games) Console.WriteLine(game.ToString());
    //UserScore.createPart(client1, games[0], Score.Super, "moja ulubiona gra");
    //UserScore.createPart(client, games[1], Score.OK, "moze byc");
    /*
    client.ClientScores();
    Console.WriteLine(client.ToString());
*/
    
    //Console.WriteLine(games[0].ToString());
    
   // Client client = new Client("Michau", "Michajo",new DateOnly(2014, 5, 29), new HashSet<PersonType>{}, 123456778, "Michcio", "michaaa");

    //Order.CreateNewOrder(client, games, Status.PickUpAtStore, Status.Cash);
    
    //Order.AssignEmployee(1, 1);
    /*

    var orders = Order.Orders();
    foreach (Order o in orders)
    {
        Console.WriteLine(o.ToString());
    }
    
    // test funkcjonalnosci do GUI
    // logowanie chłopka 
    var testClient = Client.GetClientByLoginIn("TestoPolak", "testo123");

    // -> wybieramy gre na stronie, na bieżąco parametr ID 
    var testGame = Game.GetGameByID(2);
    
    // -> po logowaniu mozemy dodac komentarz klikając buttona:
    // testClient i testGame mamy automatycznie (jak jestesmy zalogowani i wybierzemy gre)
    // comment i score dajemy z formularza
    UserScore.createPart(testClient, testGame, Score.Bad, "plakalem jak przechodzilem");

    if (testClient is not null)
    {
        Console.WriteLine(testClient.ToString());
    }
    Console.Write(testGame.ToString());
  
    
   var x =  Employee.EmployeesList();

    foreach (Employee e in x)
    {
        Console.WriteLine(e);
    }
     */
    var games = Game.GetAllGamesFromDB();
    
    //foreach (Game game in games) Console.WriteLine(game.ToString());

    Client client = new Client("Michau", "Michajo",new DateOnly(2014, 5, 29), 123456778, "Michcio", "michaaa");

    Order.CreateNewOrder(client, games, Status.PickUpAtStore, Status.Cash);
    
    Order.AssignEmployee(1, 1);
    Order.SendOrder(1);
    
    var orders = Order.Orders();
    foreach (Order o in orders)
    {
        Console.WriteLine(o.ToString());
    }


