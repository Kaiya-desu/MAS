namespace MAS_MP1.Person;

public class Address
{
    public string Country;
    public string City;
    public string Street;
    public string HouseNumber;
    public string PostCode;

    // zrobic kompozycje jak w UserScore
    private Address(string country, string city, string street, string houseNumber, string postCode)
    {
        Country = country;
        City = city;
        Street = street;
        HouseNumber = houseNumber;
        PostCode = postCode;
    }

    public static Address AddAddress(string country, string city, string street, string houseNumber, string postCode)
    {
        return new Address(country, city, street, houseNumber, postCode);
    }

    // adresem zarządza osoba Person, klasy do modyfikacji adresu znajdują się w person.

}