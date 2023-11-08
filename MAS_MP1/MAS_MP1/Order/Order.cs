using MAS_MP1.Database;
using MAS_MP1.Enums;
using MAS_MP1.Person;
using MAS_MP1.Product;

namespace MAS_MP1.Order;

public class Order
{
    public string ClientLogin;
    public List<Game> Games;
    public DateTime OrderDate;
    public DateTime? ShippingDate; // np -> gdy przedmiot zostanie wysłany i zmieni się orderstatus
    public double? FullPrice;
    public Status ShippingType;
    public Status OrderStatus;
    public EmployeeWarehouse EmployeeWarehouse;  // moze zostać przydzielony do zamówienia,

    public bool Bonus;
    public Payment Payment;
    public static List<Order> _orders = new List<Order>();
    
    // tutaj musi pobierac liste gier (tabela wiele do wiele), id_klienta,
    // obliczać cene zamowienia na podstawie tabeli wiele-do-wiele z grami (po cenach gry)
    private Order(string login, List<Game> gameList, Status shippingType, bool bonus)
    {
        //Client = client;
        ClientLogin = login; // i tak dodajemy klienta po loginie do tabeli
        Games = gameList;
        OrderDate = DateTime.Now;
        ShippingType = shippingType;
        OrderStatus = Status.Collecting;
        Bonus = bonus;
    }
    
    // aby utworzyc obiekt korzystamy z tej metody
    public static void CreateNewOrder(Client client, List<Game> gameList, Status shippingType, Status paymentForm)
    { 
        var bonus = client.BirthdayBonus; // bonus urodziny
        Order o = new Order(client.Login, gameList, shippingType, bonus);
        AddOrder(o, paymentForm); //dodanie do DB
    }

    public static List<Order> Orders()
    {
        var reader = Connection.Select($"SELECT * FROM New_Order WHERE Status IS NOT 'Done'");
        while (reader.Read())
        {
            var id_order = Convert.ToInt32(reader["ID_Order"]);
            var client_login = Convert.ToString(reader["Client_Login"]);
            var order_date_raw = Convert.ToString(reader["Date"]);
            var order_date = Convert.ToDateTime(order_date_raw);
            var shipping_raw = Convert.ToString(reader["Shipping_Type"]);
            var shipping = (Status) Enum.Parse(typeof(Status), shipping_raw);
            var status_raw = Convert.ToString(reader["Status"]);
            var status = (Status) Enum.Parse(typeof(Status), status_raw);
            var shipping_date_raw = Convert.ToString(reader["Date"]);
            var shipping_date = Convert.ToDateTime(shipping_date_raw);
            var bonus = Convert.ToBoolean(reader["Bonus"]);
            var reader2 = Connection.Select($"SELECT * FROM Game_New_Order WHERE New_Order_ID_Order = {id_order}");
            // game list
            List<Game> games = new List<Game>();
            while (reader2.Read())
            {
                var id_game = Convert.ToInt32(reader2["Game_ID_Game"]);
                games.Add(Game.GetGameByID(id_game));
            }
            
            _orders.Add(new Order(client_login, games, shipping, bonus));
            // wziac pozostale rzeczy z db - te które mozna nadać później / modyfikować etc a których nie dajemy przy tworzeniu obiektu.
            _orders[-1].OrderDate = order_date;
            _orders[-1].OrderStatus = status;
            _orders[-1].ShippingDate = shipping_date;
            _orders[-1].FullPrice = CalculateFullPrice(id_order);
        }
        return _orders;
    }
    private static void AddOrder(Order o, Status paymentForm)
    {
        var ID_Order = 0;
        ID_Order =  Connection.Insert($"INSERT INTO New_Order (Client_Login, Date, Shipping_Type, Status, Bonus) VALUES ('{o.ClientLogin}', '{o.OrderDate}', '{o.ShippingType}', '{o.OrderStatus}', '{o.Bonus}')");
        Console.WriteLine(" teest " + ID_Order);
        // dodanie listy gier do tabeli wiele do wiele
        if (ID_Order != 0)
        {
            foreach (Game g in o.Games)
            {
                Connection.Insert(
                    $"INSERT INTO Game_New_Order (New_Order_ID_Order, Game_ID_Game) VALUES ({ID_Order}, {g.IDGame})");
            }

            o.FullPrice = CalculateFullPrice(ID_Order);
            Connection.Edit($"UPDATE New_Order SET FullPrice = {o.FullPrice} WHERE ID_Order = {ID_Order}");

            Payment p = new Payment(paymentForm, ID_Order);
        }
    }

    public static void DeleteOrder(int idO)
    {
        Connection.Delete($"DELETE From Payment WHERE Order_ID_Order = {idO}");
        Connection.Delete($"DELETE From New_Order WHERE ID_Order = {idO}");
    }
    public static double CalculateFullPrice(int id)
    {
        var reader = Connection.Select($"SELECT SUM(G.Price) as FullPrice FROM Game_New_Order INNER JOIN Game G on G.ID_Game = Game_New_Order.Game_ID_Game WHERE New_Order_ID_Order = {id}");
        var price = 0.0;
        while (reader.Read())
        {
            price = Convert.ToDouble(reader["FullPrice"]);
        }
        
        price = price * 1.23 - (price * Game.Promo) ; // 23% VATu + ewentualna promocja
        
        
        return price;
    }
    
    // przypisanie chłopka do orderu -> zmieni też jego status na "Pakowanie"
    public static void AssignEmployee(int idOrder, int idEmployeeWarehouse)
    {
        var orderStatus = Status.Collecting;
        Connection.Edit($"UPDATE New_Order SET Status = '{orderStatus}', Employee_Warehouse_ID_Employee_Warehouse = {idEmployeeWarehouse} WHERE ID_Order = {idOrder}");
    }
    
    // wysyłamy zamówienie (musi byc przypisany pracownik), zmieniamy status i nadajemy date wysyłki.
    public static void SendOrder(int idOrder)
    {
        // sprawdzamy czy został już przydzielony pracownik
        var reader = Connection.Select($"SELECT Employee_Warehouse_ID_Employee_Warehouse FROM New_Order WHERE ID_Order = {idOrder}");
        int idE = 0;
        while (reader.Read())
        {
            idE = Convert.ToInt32(reader["Employee_Warehouse_ID_Employee_Warehouse"]);
        }

        if (idE != 0)
        {
            var shippingDate = DateTime.Now;
            var orderStatus = Status.Shipped;
        
            Connection.Edit($"UPDATE New_Order SET Status = '{orderStatus}', Shipping_Date = '{shippingDate}' WHERE ID_Order = {idOrder}"); 
        }
    }

    public override string ToString()
    {
        string info = "ORDER BY " + ClientLogin + "\n";
        info += "Games : ";
        foreach (Game g in Games)
        {
            info +=  g.Name + " | ";
        }

        info += "\n Status " + OrderStatus;
        return info;
    }
}