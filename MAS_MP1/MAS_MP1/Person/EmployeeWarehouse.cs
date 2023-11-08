using MAS_MP1.Database;
using MAS_MP1.Enums;

namespace MAS_MP1.Person;

public class EmployeeWarehouse : Employee
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
    public override float HourlyWage  { get; set; }
    public override float PartTime  { get; set; }
    public override float Payment { get; set; }
    
    public bool ForkliftDriveLicence { get; set; }
    
    public EmployeeWarehouse(string name, string surname, DateOnly birthDate, int? phoneNumber, 
        Sex sex, string? maidenName, int pesel, DateOnly employmentDate, float hourlyWage, float partTime, bool forkliftDriveLicence) 
        : base(name, surname, birthDate, phoneNumber,sex, maidenName, pesel, employmentDate, hourlyWage, partTime)
    {
        ForkliftDriveLicence = forkliftDriveLicence;
        Payment = CountPayment();
        
        if (GetEmployeeIDByPesel(pesel) == 0)
        {
            AddEmployee();
        }
        ID = GetEmployeeIDByPesel(pesel);
    }

    public EmployeeWarehouse(){}

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
            Connection.Insert($"INSERT INTO Employee_Warehouse (Employee_ID_Employee, Forklift_Licence) VALUES ({idE}, '{ForkliftDriveLicence}')");
        }
        
    }
    
    // edycja employee
    public static void DeleteEmployeeWarehouse(int idEmp)
    {
        Connection.Edit($"UPDATE New_Order SET Employee_Warehouse_ID_Employee_Warehouse = null  WHERE Employee_Warehouse_ID_Employee_Warehouse = {idEmp}");
        Connection.Delete($"DELETE From Employee_Warehouse WHERE ID_Employee_Support = {idEmp}");
     }

    public override float CountPayment()
    {
        return MathF.Round(HourlyWage * (PartTime * 160) + (ForkliftDriveLicence ? 500 : 0), 2); 
        // bonusik za wózki widłowe
    }

    public override string ToString()
    {
        return base.ToString() + (ForkliftDriveLicence ? "| Have forklift a licence |" : "| Do not have a forklift licence |");
    }
}