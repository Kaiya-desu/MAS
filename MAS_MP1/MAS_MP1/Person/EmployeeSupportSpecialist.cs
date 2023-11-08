using MAS_MP1.Database;
using MAS_MP1.Enums;

namespace MAS_MP1.Person;

public class EmployeeSupportSpecialist : Employee
{
    public override int ID { get; set; }
    public override string Name { get; set; }
    public override string Surname { get; set; }
    public override DateOnly DateOfBirth { get; set; }
    public override int? PhoneNumber { get; set; }
    public override int Pesel { get; set; }
    
    public override DateOnly EmploymentDate { get; set; }
    public override Sex Sex { get; set; }
    public override string? MaidenName { get; set; }
    public override float HourlyWage { get; set; }
    public override float PartTime  { get; set; }
    public override float Payment { get; set; }

    public HashSet<Language> Languages { get; set; } 

    public EmployeeSupportSpecialist(string name, string surname, DateOnly birthDate, int? phoneNumber, 
        Sex sex, string? maidenName, int pesel, DateOnly employmentDate, float hourlyWage, float partTime, HashSet<Language> languages) 
        : base(name, surname, birthDate, phoneNumber, sex, maidenName, pesel, employmentDate, hourlyWage, partTime)
    {
        Payment = CountPayment();
        Languages = languages;
        
        // moze byc tylko jeden Employee o danym peselu -> więc nie mozna byc jednocześnie EmpSupport i EmpWarehouse 
        // czyli XOR 
        if (GetEmployeeIDByPesel(pesel) == 0)
        {
            AddEmployee();
        }
        ID = GetEmployeeIDByPesel(pesel);
    }
    
    public override void AddEmployee()
    {
        // bo nowy Employee moze byc juz wczesniej jako "Person" w bazie -> np jako Client
        var idP = GetPersonIDByName(Name, Surname, DateOfBirth, PhoneNumber);
        if (idP == 0)
        {
             idP = Connection.Insert(
                $"INSERT INTO Person (Name, Surname, Birthdate, PhoneNumber) VALUES ('{Name}','{Surname}','{DateOfBirth}', '{PhoneNumber}')");
        }
        
        var idE = Connection.Insert($"INSERT INTO Employee (Person_ID_Person, Sex, Pesel, MaidenName, EmploymentDate, HourlyWage, PartTime) VALUES ({idP},'{Sex}', {Pesel}, '{MaidenName}','{EmploymentDate}', {HourlyWage}, {PartTime})");

        if (idE != 0)
        {
            var idS = Connection.Insert($"INSERT INTO Employee_Support (Employee_ID_Employee) VALUES ({idE})");

            if (idS != 0)
            {
                    foreach (Language l in Languages)
                    {
                        Connection.Insert(
                            $"INSERT INTO Employee_Support_Languages(Employee_Support_ID_Employee_Support, Language) VALUES ({idE}, '{l}')");
                    }
            }
        }
    }
    public override float CountPayment()
    {
        return MathF.Round(HourlyWage * (PartTime * 160) + (HourlyWage * (PartTime * 160) * SaleBonus), 2);
        // bonus sprzedazowy z klasy Employee 
    }

    public override string ToString()
    {
        var str = "";
        foreach (var VARIABLE in Languages)
        {
            str += VARIABLE.ToString() + " ";
        }
        return base.ToString() + " Languages: [" + str + "]";
    }

    // usuwanie z kilku tabeli na raz bo mamy dziedziczenie 
    public static void DeleteEmployeeSupport(int idEmp)
    {
        var reader =
            Connection.Select($"SELECT Employee_ID_Employee FROM Employee_Support WHERE ID_Employee_Warehouse = {idEmp}");
        int idE = 0;
        while (reader.Read())
        {
            idE = Convert.ToInt32(reader["Employee_ID_Employee"]);
        }

        if (idE > 0)
        {
            Connection.Delete($"DELETE From Employee_Support_Languages_ID_ESL WHERE ID_Employee_Support = {idEmp}");
            Connection.Edit($"UPDATE Question SET Employee_Support_Languages_ID_ESL = null WHERE ID_Employee_Support = {idEmp}");
            Connection.Delete($"DELETE FROM Question_Answer WHERE Question_ID_Employee = {idEmp}");
            Connection.Delete($"DELETE From Employee WHERE ID_Employee= {idE}");
        }
    }
}