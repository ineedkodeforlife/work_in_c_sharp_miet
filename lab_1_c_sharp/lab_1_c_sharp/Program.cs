using System;

// Перечисление TimeFrame
enum TimeFrame
{
    Year,
    TwoYears,
    Long
}

// Класс Person
class Person
{
    public string Name { get; set; }
    public string LastName { get; set; }
}

// Класс Paper
class Paper
{
    public string Title { get; set; }
    public Person Author { get; set; }
    public DateTime PublicationDate { get; set; }

    public Paper(string title, Person author, DateTime publicationDate)
    {
        Title = title;
        Author = author;
        PublicationDate = publicationDate;
    }

    public Paper()
    {
        Title = "DefaultTitle";
        Author = new Person { Name = "DefaultAuthorName", LastName = "DefaultAuthorLastName" };
        PublicationDate = DateTime.Now;
    }

    public override string ToString()
    {
        return $"Title: {Title}, Author: {Author.Name} {Author.LastName}, Publication Date: {PublicationDate}";
    }
}

// Класс ResearchTeam
class ResearchTeam
{
    private string researchTopic;
    private string organization;
    private int registrationNumber;
    private TimeFrame timeFrame;
    private Paper[] papers;

    public ResearchTeam(string researchTopic, string organization, int registrationNumber, TimeFrame timeFrame)
    {
        this.researchTopic = researchTopic;
        this.organization = organization;
        this.registrationNumber = registrationNumber;
        this.timeFrame = timeFrame;
        papers = new Paper[0];
    }

    public ResearchTeam()
    {
        researchTopic = "DefaultTopic";
        organization = "DefaultOrganization";
        registrationNumber = 0;
        timeFrame = TimeFrame.Year;
        papers = new Paper[0];
    }

    public string ResearchTopic
    {
        get { return researchTopic; }
        set { researchTopic = value; }
    }

    public string Organization
    {
        get { return organization; }
        set { organization = value; }
    }

    public int RegistrationNumber
    {
        get { return registrationNumber; }
        set { registrationNumber = value; }
    }

    public TimeFrame TimeFrame
    {
        get { return timeFrame; }
        set { timeFrame = value; }
    }

    public Paper[] Papers
    {
        get { return papers; }
        set { papers = value; }
    }

    public Paper LatestPaper
    {
        get
        {
            if (papers.Length == 0)
            {
                return null;
            }

            Paper latestPaper = papers[0];
            foreach (var paper in papers)
            {
                if (paper.PublicationDate > latestPaper.PublicationDate)
                {
                    latestPaper = paper;
                }
            }
            return latestPaper;
        }
    }

    public bool this[TimeFrame tf]
    {
        get { return timeFrame == tf; }
    }

    public void AddPapers(params Paper[] newPapers)
    {
        int oldLength = papers.Length;
        Array.Resize(ref papers, oldLength + newPapers.Length);
        for (int i = 0; i < newPapers.Length; i++)
        {
            papers[oldLength + i] = newPapers[i];
        }
    }

    public override string ToString()
    {
        string paperList = string.Join("\n", papers.Select(p => p.ToString()));
        return $"Research Topic: {researchTopic}, Organization: {organization}, Registration Number: {registrationNumber}, Time Frame: {timeFrame}\nPublications:\n{paperList}";
    }

    public virtual string ToShortString()
    {
        return $"Research Topic: {researchTopic}, Organization: {organization}, Registration Number: {registrationNumber}, Time Frame: {timeFrame}";
    }
}

class Program
{
    static void Main()
    {
        ResearchTeam team = new ResearchTeam();
        Console.WriteLine("Research Team Info (Short):");
        Console.WriteLine(team.ToShortString());

        Console.WriteLine("\nTimeFrame.Year: " + team[TimeFrame.Year]);
        Console.WriteLine("TimeFrame.TwoYears: " + team[TimeFrame.TwoYears]);
        Console.WriteLine("TimeFrame.Long: " + team[TimeFrame.Long]);

        team.ResearchTopic = "NewTopic";
        team.Organization = "NewOrganization";
        team.RegistrationNumber = 12345;
        team.TimeFrame = TimeFrame.Long;

        Console.WriteLine("\nUpdated Research Team Info:");
        Console.WriteLine(team.ToString());

        Paper paper1 = new Paper("Paper1", new Person { Name = "Author1", LastName = "AuthorLastName1" }, DateTime.Now.AddYears(-2));
        Paper paper2 = new Paper("Paper2", new Person { Name = "Author2", LastName = "AuthorLastName2" }, DateTime.Now.AddYears(-1));
        team.AddPapers(paper1, paper2);

        Console.WriteLine("\nUpdated Research Team Info with Papers:");
        Console.WriteLine(team.ToString());

        Console.WriteLine("\nLatest Paper:");
        Console.WriteLine(team.LatestPaper);

        // Сравнение времени выполнения операций с массивами
        int numPapers = 1000000;
        Paper[] oneDimensionalArray = new Paper[numPapers];
        Paper[,] twoDimensionalArray = new Paper[numPapers, 1];
        Paper[][] jaggedArray = new Paper[numPapers][];
        for (int i = 0; i < numPapers; i++)
        {
            oneDimensionalArray[i] = new Paper();
            twoDimensionalArray[i, 0] = new Paper();
            jaggedArray[i] = new Paper[] { new Paper() };
        }

        Console.WriteLine("\nComparison of array performance:");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < numPapers; i++)
        {
            var paper = oneDimensionalArray[i];
        }
        stopwatch.Stop();
        Console.WriteLine($"Accessing elements in one-dimensional array: {stopwatch.ElapsedMilliseconds}ms");

        stopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < numPapers; i++)
        {
            var paper = twoDimensionalArray[i, 0];
        }
        stopwatch.Stop();
        Console.WriteLine($"Accessing elements in two-dimensional array: {stopwatch.ElapsedMilliseconds}ms");

        stopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < numPapers; i++)
        {
            var paper = jaggedArray[i][0];
        }
        stopwatch.Stop();
        Console.WriteLine($"Accessing elements in jagged array: {stopwatch.ElapsedMilliseconds}ms");
    }
}
