using MAS_MP1.Database;
using MAS_MP1.Enums;
using MAS_MP1.Person;

namespace MAS_MP1.Order;

public class Question
{
    // to zrobic najpierw

    public int ID_Question;
    
    public string Title;
    public Language Language;
    public DateTime Date;
    // zrobic z tego tablice, w bazie sklada sie z ID_Pytania i tresci
    public string Description;
    public Status QuestionStatus;

    public string ClientLogin;
    public EmployeeSupportSpecialist EmployeeSupportSpecialist;

    public List<String> Answers;
    
    
    public Question(string login, string title, Language language, string description)
    {
        ClientLogin = login;
        Title = title;
        Language = language;
        Date = DateTime.Now;
        Description = description;
        QuestionStatus = Status.WaitingForSupport; // po otworzeniu pytania domyslnie pytanie ma status "waiting"

        ID_Question = CheckQuestion(title, description);
        if (ID_Question == 0)
        {
            AddQuestion();
        }
        
        AddToAnswersList(ID_Question);
    }

    public void AddToAnswersList(int id)
    {
        var reader = Connection.Select($"SELECT * FROM Question_Answers_List WHERE Question_ID_Question = {id}");
        while (reader.Read())
        {
            var ans = Convert.ToString(reader["Answer"]);
            Answers.Add(ans);
        }
    }
    private void AddQuestion()
    {
        // sprawdzamy czy taki klient znajduje się w DB
        if (Client.CheckLogin(ClientLogin) != 0)
        {
            ID_Question = Connection.Insert($"INSERT INTO Question (Client_Login, Title, Language, Date, Status) VALUES ('{ClientLogin}','{Title}', '{Language}', '{Date}', '{QuestionStatus}')");
            AnswerQuestion(ID_Question, Description);
        }
    }
    
    //abysmy mogli odpowiadac na zadane pytania (tj jedno pytanie sklada sie z wielu tresci)
    public static void AnswerQuestion(int id, string answer)
    {
        // dodanie do tabeli drugiej ->
        Connection.Insert($"INSERT INTO Question_Answers_List (Question_ID_Question, Answer) VALUES ({id}, '{answer}')");
    }

    private int CheckQuestion(string title, string description)
    {
        // aby wygodniej sie testowalo zadanie, nie pozwalam by mogly istniec dwa pytania o takim samym tytule
        var reader = Connection.Select($"SELECT ID_Question FROM Question WHERE Title = '{title}'");
        int x = 0;
        while (reader.Read())
        {
            x = Convert.ToInt32(reader["ID_Question"]);
        }
        return x;
    }
    
    public static void AddEmployeeToTheQuestion(Question q, Employee? e)
    {
        // szukamy id question 
        var id_q = q.CheckQuestion(q.Title, q.Description); // niby tez mozna q.ID_Question

        // szukamy pracownika ktorego język odpowiada jezykowi w zapytaniu
        var questionLanguage = q.Language;
        var id_emp = Employee.GetEmployeeIDByPesel(e.Pesel);
        var reader = Connection.Select($"SELECT Employee_Support_ID_Employee_Support FROM Employee_Support_Languages esl INNER JOIN Employee_Support ES on esl.Employee_Support_ID_Employee_Support = ES.ID_Employee_Support INNER JOIN Employee E on E.ID_Employee = ES.Employee_ID_Employee WHERE E.ID_Employee = {id_emp} AND Language = '{questionLanguage}'");
        // przypisujemy pracownika do zapytania
        var id_emp_supp = 0;
        while (reader.Read())
        {
            id_emp_supp = Convert.ToInt32(reader["Employee_Support_ID_Employee_Support"]);
        }
        if (id_emp_supp != 0)
            Connection.Edit($"UPDATE Question SET Employee_Support_Languages_ID_ESL = {id_emp_supp} WHERE ID_Question = {id_q}");
        else
            Console.WriteLine("Sorry this employee cant be added to this question - he dont know " + q.Language + " language");
    }

    
    
}