using System;
using System.Collections;

// Перечисление TimeFrame


interface INameAndCopy
{
    string Name { get; set; }
    object DeepCopy();
}


enum TimeFrame
{
    Year,
    TwoYears,
    Long
}

// Класс Person
class Person : INameAndCopy
{
    public string Name { get; set; }
    //    public string LastName { get; set; }

    public string LastName { get; set; }

    public Person(string name)

    {
        Name = name;
    }

    public object DeepCopy()
    {
        return new Person(Name);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Person other = (Person)obj;
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public static bool operator ==(Person person1, Person person2)
    {
        if (ReferenceEquals(person1, person2))
            return true;
        if (ReferenceEquals(person1, null) || ReferenceEquals(person2, null))
            return false;
        return person1.Equals(person2);
    }

    public static bool operator !=(Person person1, Person person2)
    {
        return !(person1 == person2);
    }

    // Переопределение метода ToString
    public override string ToString()
    {
        return $"Person: {Name}";
    }
}

// Класс Paper
class Paper : INameAndCopy
{
    public string Name { get; set; }
    public Person Author { get; set; }
    public DateTime PublicationDate { get; set; }

    public Paper(string name, Person author, DateTime publicationDate)
    {
        Name = name;
        Author = author;
        PublicationDate = publicationDate;
    }

    public Paper()
    {
        Name = "";
        Author = new Person("Default_name") { LastName = "DefaultAuthorSurName"};
        PublicationDate = DateTime.Now;
    }

    public object DeepCopy()
    {
        return new Paper(Name, (Person)Author.DeepCopy(), PublicationDate);
    }

    public override string ToString()
    {
        return $"Title: {Name}, Author: {Author.Name} {Author.LastName}, Publication Date: {PublicationDate}";
    }
}

class Team : INameAndCopy
{
    protected string organization;
    protected int registrationNumber;

    // Свойство для доступа к полю с названием организации
    public string Organization
    {
        get { return organization; }
        set { organization = value; }
    }

    // Свойство для доступа к полю с номером регистрации
    public int RegistrationNumber
    {
        get { return registrationNumber; }
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Registration number must be greater than 0.");
            }
            registrationNumber = value;
        }
    }

    // Конструктор с параметрами
    public Team(string org, int regNumber)
    {
        Organization = org;
        RegistrationNumber = regNumber;
    }

    // Конструктор без параметров
    public Team()
    {
        Organization = "";
        RegistrationNumber = 0;
    }

    // Реализация метода DeepCopy
    public virtual object DeepCopy()
    {
        return new Team(Organization, RegistrationNumber);
    }

    // Реализация интерфейса INameAndCopy
    string INameAndCopy.Name
    {
        get { return Organization; }
        set { Organization = value; }
    }

    // Переопределение метода Equals, GetHashCode и операторов == и !=
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Team other = (Team)obj;
        return Organization == other.Organization && RegistrationNumber == other.RegistrationNumber;
    }

    public override int GetHashCode()
    {
        return (Organization.GetHashCode() * 397) ^ RegistrationNumber;
    }

    public static bool operator ==(Team team1, Team team2)
    {
        if (ReferenceEquals(team1, team2))
            return true;
        if (ReferenceEquals(team1, null) || ReferenceEquals(team2, null))
            return false;
        return team1.Equals(team2);
    }

    public static bool operator !=(Team team1, Team team2)
    {
        return !(team1 == team2);
    }

    // Переопределение метода ToString
    public override string ToString()
    {
        return $"Team: Organization: {Organization}, Registration Number: {RegistrationNumber}";
    }
}



// Класс ResearchTeam
class ResearchTeam : Team, INameAndCopy
{
    private string researchTopic;
    private TimeFrame timeFrame;
    private ArrayList members;
    private ArrayList publications;

    public string ResearchTopic
    {
        get { return researchTopic; }
        set { researchTopic = value; }
    }

    public ArrayList Members
    {
        get { return members; }
    }

    public ArrayList Publications
    {
        get { return publications; }
    }

    public ResearchTeam(string researchTopic,string org, int regNumber, TimeFrame timeFrame)
        : base(org, regNumber)
    {
        this.researchTopic = researchTopic;
        this.timeFrame = timeFrame;
        members = new ArrayList();
        publications = new ArrayList();
    }

    public ResearchTeam()
         : base() // Вызываем конструктор базового класса Team
    {
        timeFrame = TimeFrame.Year;
        members = new ArrayList();
        publications = new ArrayList();
    }

    public TimeFrame TimeFrame
    {
        get { return timeFrame; }
        set { timeFrame = value; }
    }

    public Paper LatestPublication
    {
        get
        {
            if (publications.Count == 0)
                return null;

            Paper latest = (Paper)publications[0];
            foreach (Paper paper in publications)
            {
                if (paper.PublicationDate > latest.PublicationDate)
                    latest = paper;
            }
            return latest;
        }
    }

    public bool this[TimeFrame tf]
    {
        get { return timeFrame == tf; }
    }


    public override object DeepCopy()
    {
        ResearchTeam copy = new ResearchTeam(ResearchTopic, Organization, RegistrationNumber, TimeFrame);
        foreach (Person member in members)
        {
            copy.AddMembers((Person)member.DeepCopy());
        }
        foreach (Paper publication in publications)
        {
            copy.AddPapers((Paper)publication.DeepCopy());
        }
        return copy;
    }

    // Реализация интерфейса INameAndCopy
    string INameAndCopy.Name
    {
        get { return ResearchTopic; }
        set { ResearchTopic = value; }
    }

    // Добавление участников проекта
    public void AddMembers(params Person[] newMembers)
    {
        foreach (Person member in newMembers)
        {
            members.Add(member);
        }
    }

    // Добавление публикаций
    public void AddPapers(params Paper[] newPapers)
    {
        foreach (Paper publication in newPapers)
        {
            publications.Add(publication);
        }
    }

    // Перегруженный метод ToString
    public override string ToString()
    {
        string memberList = string.Join(", ", members.Cast<Person>());
        string publicationList = string.Join("\n", publications.Cast<Paper>());
        return $"ResearchTeam: Organization: {Organization}, Registration Number: {RegistrationNumber}, " +
               $"Research Topic: {ResearchTopic}, TimeFrame: {TimeFrame}\n" +
               $"Members: {memberList}\n" +
               $"Publications:\n{publicationList}";
    }

    // Метод формирования короткой строки без списка публикаций и участников
    public string ToShortString()
    {
        return $"ResearchTeam: Organization: {Organization}, Registration Number: {RegistrationNumber}, " +
               $"Research Topic: {ResearchTopic}, TimeFrame: {TimeFrame}";
    }

    // Реализация интерфейса IEnumerable для перебора участников проекта, у которых есть публикации
    public IEnumerator GetEnumerator()
    {
        foreach (Person member in members)
        {
            if (publications.Cast<Paper>().Any(paper => paper.Author == member))
            {
                yield return member;
            }
        }
    }

    // Итератор для перебора участников проекта, имеющих более одной публикации
    public IEnumerable<Person> MembersWithMultiplePublications()
    {
        var publicationCounts = new Dictionary<Person, int>();

        foreach (Paper publication in publications)
        {
            if (!publicationCounts.ContainsKey(publication.Author))
            {
                publicationCounts[publication.Author] = 0;
            }
            publicationCounts[publication.Author]++;
        }

        foreach (var kvp in publicationCounts)
        {
            if (kvp.Value > 1)
            {
                yield return kvp.Key;
            }
        }
    }

    // Итератор для перебора публикаций, вышедших за последний год
    public IEnumerable<Paper> RecentPublications(int years)
    {
        DateTime currentDate = DateTime.Now;
        foreach (Paper publication in publications)
        {
            if (currentDate.Year - publication.PublicationDate.Year <= years)
            {
                yield return publication;
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Создание двух объектов Team
        Team team1 = new Team("Org1", 1);
        Team team2 = new Team("Org1", 1);

        // Проверка на неравенство ссылок и равенство объектов
        Console.WriteLine($"References are not equal: {team1 != team2}");
        Console.WriteLine($"Objects are equal: {team1.Equals(team2)}");

        // Вывод хэш-кодов объектов
        Console.WriteLine($"HashCode for team1: {team1.GetHashCode()}");
        Console.WriteLine($"HashCode for team2: {team2.GetHashCode()}");

        // Создание объекта ResearchTeam
        ResearchTeam researchTeam = new ResearchTeam("ResearchTopic", "Org2", 2, TimeFrame.TwoYears);

        // Добавление участников проекта и публикаций
        researchTeam.AddMembers(new Person("Person1"), new Person("Person2"));
        researchTeam.AddPapers(new Paper("Paper1", new Person("Author1"), DateTime.Now),
                                new Paper("Paper2", new Person("Author2"), DateTime.Now));

        // Вывод данных объекта ResearchTeam
        Console.WriteLine(researchTeam);

        // Вывод значения свойства Team
        Console.WriteLine(researchTeam);

        // Создание копии объекта ResearchTeam
        ResearchTeam copy = (ResearchTeam)researchTeam.DeepCopy();

        // Изменение данных в исходном объекте
        researchTeam.Organization = "UpdatedOrg";
        researchTeam.AddMembers(new Person("Person3"));
        researchTeam.AddPapers(new Paper("Paper3", new Person("Author3"), DateTime.Now.AddYears(-1)));

        try
        {
            team1.RegistrationNumber =  -10;
        }
        catch  (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }


        // Вывод копии и исходного объекта
        Console.WriteLine("Original ResearchTeam:");
        Console.WriteLine(researchTeam);

        Console.WriteLine("Copy of ResearchTeam:");
        Console.WriteLine(copy);

        // Использование итераторов
        Console.WriteLine("Members with publications:");
        foreach (Person member in researchTeam)
        {
            Console.WriteLine(member);
        }

        Console.WriteLine("Members with multiple publications:");
        foreach (Person member in researchTeam.MembersWithMultiplePublications())
        {
            Console.WriteLine(member);
        }

        Console.WriteLine("Recent publications (last year):");
        foreach (Paper publication in researchTeam.RecentPublications(1))
        {
            Console.WriteLine(publication);
        }
    }
}
