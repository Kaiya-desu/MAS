using System.Diagnostics;
using MAS_MP1.Database;
using MAS_MP1.Enums;

namespace MAS_MP1.Person;

public enum Sex
{
    Male,
    Female
}

public abstract class Employee : Person 
{
    public static List<Employee> _employeesList = new List<Employee>();

    public abstract int Pesel { get; set; } 
    public abstract DateOnly EmploymentDate { get; set; } 
    
    public abstract Sex Sex { get; set; } 
    public abstract String? MaidenName { get; set; }
    // mezczyzna ma moneybonus
    
    public abstract float HourlyWage  { get; set; } 
    public abstract float PartTime { get; set; } 
    public abstract float Payment { get; set; }

    public static float SaleBonus = 0.10f;
    public abstract float CountPayment();

    public abstract void AddEmployee();

    public static List<Employee> EmployeesList()
    {
        // rozdzielam Employee na podstawie Emp ID które są w tablicy Emp_supp i Emp_Warehouse, na koncu łącze w jedno
        // EmployeeSupport:
        var reader_es = Connection.Select("SELECT * FROM Employee E inner join Employee_Support ES on E.ID_Employee = ES.Employee_ID_Employee inner join Person P on P.ID_Person = E.Person_ID_Person");
        while (reader_es.Read())
        {
            var name = Convert.ToString(reader_es["Name"]);
            var surname = Convert.ToString(reader_es["Surname"]);
            var birthdate_raw = Convert.ToString(reader_es["Birthdate"]);
            var birthdate = DateOnly.FromDateTime(Convert.ToDateTime(birthdate_raw));
            var phoneNumber = Convert.ToInt32(Convert.ToString(reader_es["PhoneNumber"]));
            
            var pesel = Convert.ToInt32(Convert.ToString(reader_es["Pesel"]));
            var sex_raw = Convert.ToString(reader_es["Sex"]);
            var sex = (Sex) Enum.Parse(typeof(Sex), sex_raw);
            var maidenName = Convert.ToString(reader_es["MaidenName"]);
            var employmentDate_raw = Convert.ToString(reader_es["EmploymentDate"]);
            var employmentDate = DateOnly.FromDateTime(Convert.ToDateTime(employmentDate_raw));
            var hourlyWage = Convert.ToSingle(Convert.ToString(reader_es["HourlyWage"]));
            var partTime = Convert.ToSingle(Convert.ToString(reader_es["PartTime"]));
            
            var ID_supp = Convert.ToInt32(Convert.ToString(reader_es["ID_Employee_Support"]));
            var languages = GetLanguagesForSupport(ID_supp);

            if (GetEmployeeIDByPesel(pesel) != 0)
            {
                _employeesList.Add(new EmployeeSupportSpecialist(name, surname, birthdate, phoneNumber,
                    sex, maidenName, pesel, employmentDate, hourlyWage, partTime, languages));
            }
        }
        
        // EmployeeWarehouse:
        var reader_w = Connection.Select("SELECT * FROM Employee E inner join Employee_Warehouse W on E.ID_Employee = W.Employee_ID_Employee inner join Person P on P.ID_Person = E.Person_ID_Person");
        while (reader_w.Read())
        {
            var name = Convert.ToString(reader_w["Name"]);
            var surname = Convert.ToString(reader_w["Surname"]);
            var birthdate_raw = Convert.ToString(reader_w["Birthdate"]);
            var birthdate = DateOnly.FromDateTime(Convert.ToDateTime(birthdate_raw));
            var phoneNumber = Convert.ToInt32(Convert.ToString(reader_w["PhoneNumber"]));
            
            var pesel = Convert.ToInt32(Convert.ToString(reader_w["Pesel"]));
            var sex_raw = Convert.ToString(reader_w["Sex"]);
            var sex = (Sex) Enum.Parse(typeof(Sex), sex_raw);
            var maidenName = Convert.ToString(reader_w["MaidenName"]);
            var employmentDate_raw = Convert.ToString(reader_w["EmploymentDate"]);
            var employmentDate = DateOnly.FromDateTime(Convert.ToDateTime(employmentDate_raw));
            var hourlyWage = Convert.ToSingle(Convert.ToString(reader_w["HourlyWage"]));
            var partTime = Convert.ToSingle(Convert.ToString(reader_w["PartTime"]));

            var ID_Warehouse = Convert.ToInt32(Convert.ToString(reader_w["ID_Employee_Warehouse"]));
            var licence = GetForkliftLicence(ID_Warehouse);

            if (GetEmployeeIDByPesel(pesel) != 0)
            {
                _employeesList.Add(new EmployeeWarehouse(name, surname, birthdate, phoneNumber,
                    sex, maidenName, pesel, employmentDate, hourlyWage, partTime, licence));
            }
        }
        
        return _employeesList;
    }
    
    public static HashSet<Language> GetLanguagesForSupport(int employee_ID)
    {
        var reader = Connection.Select($"SELECT * FROM Employee_Support_Languages WHERE Employee_Support_ID_Employee_Support = {employee_ID}");
        var tmp_list = new HashSet<Language>();
        while (reader.Read())
        {
            var language_raw = Convert.ToString(reader["Language"]);
            var language = (Language) Enum.Parse(typeof(Language), language_raw);
            tmp_list.Add(language);
        }
        return tmp_list;
    }

    public static bool GetForkliftLicence(int employee_ID)
    {
        var licence = false;
        var reader = Connection.Select($"SELECT * FROM Employee_Warehouse WHERE ID_Employee_Warehouse = {employee_ID}");
        while (reader.Read())
        {
            licence = Convert.ToBoolean(Convert.ToString(reader["Forklift_Licence"]));
        }
        return licence;
    }
   protected Employee(string name, string surname, DateOnly birthDate, int? phoneNumber, 
       Sex sex, string? maidenName, int pesel, DateOnly employmentDate, float hourlyWage, float partTime) 
       : base(name, surname, birthDate, phoneNumber)
   {
       Sex = sex;
       Pesel = pesel;
       EmploymentDate = employmentDate;
       HourlyWage = hourlyWage;
       PartTime = partTime;
       
       // z wieloaspektowego
       HaveMaidenName(maidenName);
       HaveMoneyBonus();
   }
   
   public Employee(){}
   
   public static int GetEmployeeIDByPesel(int pesel)
   {
       var reader = Connection.Select($"SELECT ID_Employee FROM Employee WHERE Pesel = {pesel}");
       int id = 0;
       while (reader.Read()) 
       {
           id = Convert.ToInt32(reader["ID_Employee"]);
       }
       return id;
   }
   // tylko dla kobiet
   private void HaveMaidenName(string? maidenName)
   {
       var a = Sex == Sex.Female ? MaidenName = maidenName : MaidenName = null;
   }

   // tylko dla męzczyzn
   private void HaveMoneyBonus()
   {
       var maleBonus = 1.23f;
       var b = Sex == Sex.Male ? HourlyWage *= maleBonus : 0f;
   }
   public override string ToString()
   {
       return "ID: " + ID + " Name " + Name + " " + Surname + " " + MaidenName +  " payment: " + Payment;
   }

   public static void DeleteEmployee(int idEmp)
   {
       // dla emp Warehouse
       var reader =
           Connection.Select($"SELECT Employee_ID_Employee FROM Employee_Warehouse WHERE Employee_ID_Employee = {idEmp}");
       int idW = 0;
       while (reader.Read())
       {
           idW = Convert.ToInt32(reader["Employee_ID_Employee"]);
       }

       if (idW != 0)
       {
           EmployeeWarehouse.DeleteEmployeeWarehouse(idW);
       }
       
       // dla emp Support
       reader =
           Connection.Select($"SELECT Employee_ID_Employee FROM Employee_Support WHERE IEmployee_ID_Employee = {idEmp}");
       int idS = 0;
       while (reader.Read())
       {
           idS = Convert.ToInt32(reader["Employee_ID_Employee"]);
       }

       if (idS != 0)
       {
           EmployeeSupportSpecialist.DeleteEmployeeSupport(idS);
       }
   }

   
}