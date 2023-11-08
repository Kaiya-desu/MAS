using MAS_MP1.Database;

namespace MAS_MP1.Person;
    
public abstract class Person
{

    public abstract int ID  { get; set; }
    public abstract string Name { get; set; }
    public abstract string Surname { get; set; }
    public abstract DateOnly DateOfBirth { get; set; }
    public abstract int? PhoneNumber { get; set; }
    //----- 

    protected Person(String name, String surname, DateOnly birthDate, int? phoneNumber)
    {
        Name = name;
        Surname = surname;
        CheckNumber(phoneNumber); // musi miec 9 cyfr
        DateOfBirth = birthDate;
    }

    private void CheckNumber(int? phoneNumber)
    {
        if (phoneNumber >= 100000000)
        {
            PhoneNumber = phoneNumber;
        }
    }
    protected Person() {}
    
    public static int GetPersonIDByName(string name, string surname, DateOnly birthDate, int? phoneNumber)
    {
        var reader = Connection.Select($"SELECT ID_Person FROM Person WHERE Name = '{name}' AND Surname = '{surname}' AND Birthdate = '{birthDate}' AND PhoneNumber = {phoneNumber}");
        string x = "";
        while (reader.Read()) 
        {
            x = Convert.ToString(reader["ID_Person"]);
        }
        if (x != "")
        {
            return Convert.ToInt32(x);
        }
        return 0;
    }

    
    public static void AddAddressToDB(Person p, string country, string city, string street, string houseNumber, string postCode)
    {
        var id = GetPersonIDByName(p.Name, p.Surname, p.DateOfBirth, p.PhoneNumber);
        var a = Address.AddAddress(country, city, street, houseNumber, postCode);
        int boolID = 0;
        var reader = Connection.Select($"SELECT Person_ID_Person FROM Address WHERE Person_ID_Person = {id}");
        while (reader.Read()) 
        {
            boolID = Convert.ToInt32(Convert.ToString(reader["Person_ID_Person"]));
        }   
        // sprawdzamy czy osoba ma juz dodany adres -> przyjmujemy ze kazdy moze miec tylko jeden adres
        if (boolID == 0)
        {
            Connection.Insert($"INSERT INTO Address (Person_ID_Person, Country, City, Street, HouseNumber, PostCode) VALUES ({id},'{a.Country}', '{a.City}', '{a.Street}', '{a.HouseNumber}', '{a.PostCode}')");
        }
        else
        {
            Console.WriteLine("You already added an addres.. Cant insert new address! Will edit old one instead :)");
            EditAddress(p, a);
        }
    }

    public static void EditAddress(Person p, Address a)
    {
        var id = GetPersonIDByName(p.Name, p.Surname, p.DateOfBirth, p.PhoneNumber);
        Connection.Edit($"UPDATE Address SET Country = '{a.Country}', City = '{a.City}', Street = '{a.Street}', HouseNumber = '{a.HouseNumber}', PostCode = '{a.PostCode}' WHERE Person_ID_Person = {id}");
    }

    public static void DeletePerson(int idP)
    {
        // bo Person moze istniec bez klienta
        var readerC = Connection.Select($"SELECT Login FROM Client WHERE Person_ID_Person = {idP}");
        string login = "";
        while (readerC.Read())
        { 
            login = Convert.ToString(readerC["Login"]);
        }
        
        // kasujmey jezeli jest userem
        if (login != "")
        { 
            Client.DeleteClient(login);
        }
        
        // kasujemy jezeli jest pracownikiem
        var readerE = Connection.Select($"SELECT Employee_ID_Employee FROM Employee WHERE Person_ID_Person = {idP}");
        int idE = 0;
        while (readerE.Read())
        { 
            idE = Convert.ToInt32(readerE["Employee_ID_Employee"]);
        }

        if (idE != 0)
        {
            Employee.DeleteEmployee(idE);
        }
        
       
    }
}