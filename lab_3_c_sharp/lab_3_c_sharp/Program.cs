using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

class Person : INameAndCopy
{
    public string Name { get; set; }
    public string LastName { get; set; }


    public Person(string name)
    {
        Name = name;
    }

    public object DeepCopy()
    {
        return new Person(Name) { LastName = LastName };
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

    public override string ToString()
    {
        return $"Person: {Name} {LastName}";
    }
}

class Paper : INameAndCopy, IComparable<Paper>
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
        Author = new Person("Default_name") { LastName = "DefaultAuthorSurName" };
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

    public int CompareTo(Paper other)
    {
        return PublicationDate.CompareTo(other.PublicationDate);
    }
}

class PaperComparer : IComparer<Paper>
{
    public int Compare(Paper x, Paper y)
    {
        return StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name);
    }
}

class Team : INameAndCopy
{
    protected string organization;
    protected int registrationNumber;

    public string Organization
    {
        get { return organization; }
        set { organization = value; }
    }

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

    public Team(string org, int regNumber)
    {
        Organization = org;
        RegistrationNumber = regNumber;
    }

    public Team()
    {
        Organization = "";
        RegistrationNumber = 0;
    }

    public virtual object DeepCopy()
    {
        return new Team(Organization, RegistrationNumber);
    }

    string INameAndCopy.Name
    {
        get { return Organization; }
        set { Organization = value; }
    }

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

    public override string ToString()
    {
        return $"Team: Organization: {Organization}, Registration Number: {RegistrationNumber}";
    }
}

class ResearchTeam : Team, INameAndCopy
{
    private string researchTopic;
    private TimeFrame timeFrame;
    private List<Person> members;
    private List<Paper> publications;

    public string ResearchTopic
    {
        get { return researchTopic; }
        set { researchTopic = value; }
    }

    public List<Person> Members
    {
        get { return members; }
    }

    public List<Paper> Publications
    {
        get { return publications; }
    }

    public ResearchTeam(string researchTopic, string org, int regNumber, TimeFrame timeFrame)
        : base(org, regNumber)
    {
        this.researchTopic = researchTopic;
        this.timeFrame = timeFrame;
        members = new List<Person>();
        publications = new List<Paper>();
    }

    public ResearchTeam()
         : base()
    {
        timeFrame = TimeFrame.Year;
        members = new List<Person>();
        publications = new List<Paper>();
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

            Paper latest = publications[0];
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

    string INameAndCopy.Name
    {
        get { return ResearchTopic; }
        set { ResearchTopic = value; }
    }

    public void AddMembers(params Person[] newMembers)
    {
        members.AddRange(newMembers.Select(member => (Person)member.DeepCopy()));
    }

    public void AddPapers(params Paper[] newPapers)
    {
        publications.AddRange(newPapers.Select(publication => (Paper)publication.DeepCopy()));
    }

    public override string ToString()
    {
        string memberList = string.Join(", ", members);
        string publicationList = string.Join("\n", publications);
        return $"ResearchTeam: Organization: {Organization}, Registration Number: {RegistrationNumber}, " +
               $"Research Topic: {ResearchTopic}, TimeFrame: {TimeFrame}\n" +
               $"Members: {memberList}\n" +
               $"Publications:\n{publicationList}";
    }

    public string ToShortString()
    {
        return $"ResearchTeam: Organization: {Organization}, Registration Number: {RegistrationNumber}, " +
               $"Research Topic: {ResearchTopic}, TimeFrame: {TimeFrame}";
    }

    public IEnumerator GetEnumerator()
    {
        foreach (Person member in members)
        {
            if (publications.Any(paper => paper.Author == member))
            {
                yield return member;
            }
        }
    }

    public IEnumerable<Person> MembersWithMultiplePublications()
    {
        var publicationCounts = publications.GroupBy(paper => paper.Author)
                                           .ToDictionary(group => group.Key, group => group.Count());

        foreach (var kvp in publicationCounts)
        {
            if (kvp.Value > 1)
            {
                yield return kvp.Key;
            }
        }
    }

    public IEnumerable<Paper> RecentPublications(int years)
    {
        DateTime currentDate = DateTime.Now;
        return publications.Where(publication => currentDate.Year - publication.PublicationDate.Year <= years);
    }

    public void SortPublicationsByDate()
    {
        publications.Sort();
    }

    public void SortPublicationsByName()
    {
        publications.Sort(new PaperComparer());
    }

    public void SortPublicationsByAuthor()
    {
        publications.Sort((paper1, paper2) => string.Compare(paper1.Author.LastName, paper2.Author.LastName, StringComparison.OrdinalIgnoreCase));
    }
}

class ResearchTeamCollection<TKey>
{
    private Dictionary<TKey, ResearchTeam> teams;
    private KeySelector<TKey> keySelector;

    public ResearchTeamCollection(KeySelector<TKey> keySelector)
    {
        teams = new Dictionary<TKey, ResearchTeam>();
        this.keySelector = keySelector;
    }

    public void AddDefaults()
    {
        AddResearchTeams(
            new ResearchTeam("ResearchTopic1", "Org1", 1, TimeFrame.Year),
            new ResearchTeam("ResearchTopic2", "Org2", 2, TimeFrame.TwoYears),
            new ResearchTeam("ResearchTopic3", "Org3", 3, TimeFrame.Long)
        );
    }

    public void AddResearchTeams(params ResearchTeam[] newTeams)
    {
        foreach (ResearchTeam team in newTeams)
        {
            teams.Add(keySelector(team), team);
        }
    }

    public DateTime LastPublicationDate
    {
        get
        {
            if (teams.Count == 0 || teams.All(pair => pair.Value.Publications.Count == 0))
                return default(DateTime);

            return teams.SelectMany(pair => pair.Value.Publications)
                        .Max(publication => publication.PublicationDate);
        }
    }


    public IEnumerable<KeyValuePair<TKey, ResearchTeam>> TimeFrameGroup(TimeFrame value)
    {
        return teams.Where(pair => pair.Value.TimeFrame == value);
    }

    public IEnumerable<IGrouping<TimeFrame, KeyValuePair<TKey, ResearchTeam>>> TimeFrameGroup()
    {
        return teams.GroupBy(pair => pair.Value.TimeFrame);
    }

    public override string ToString()
    {
        return string.Join("\n", teams.Select(pair => $"{pair.Key}: {pair.Value}"));
    }

    public string ToShortString()
    {
        return string.Join("\n", teams.Select(pair => $"{pair.Key}: {pair.Value.ToShortString()}"));
    }
}

delegate TKey KeySelector<TKey>(ResearchTeam rt);

class Program
{
    static void Main(string[] args)
    {
        ResearchTeam researchTeam = new ResearchTeam("ResearchTopic", "Org2", 2, TimeFrame.TwoYears);

        researchTeam.AddMembers(new Person("Person1"), new Person("Person2"));
        researchTeam.AddPapers(
            new Paper("Paper1", new Person("Author1"), DateTime.Now),
            new Paper("Paper2", new Person("Author2"), DateTime.Now)
        );

        Console.WriteLine("Original ResearchTeam:");
        Console.WriteLine(researchTeam);

        Console.WriteLine("\nSorting publications by date:");
        researchTeam.SortPublicationsByDate();
        Console.WriteLine(researchTeam);

        Console.WriteLine("\nSorting publications by name:");
        researchTeam.SortPublicationsByName();
        Console.WriteLine(researchTeam);

        Console.WriteLine("\nSorting publications by author:");
        researchTeam.SortPublicationsByAuthor();
        Console.WriteLine(researchTeam);

        ResearchTeamCollection<string> collection = new ResearchTeamCollection<string>(rt => rt.Organization);

        Console.WriteLine("\nAdding default teams to collection:");
        collection.AddDefaults();
        Console.WriteLine(collection);

        Console.WriteLine("\nLast publication date in the collection:");
        Console.WriteLine(collection.LastPublicationDate);

        Console.WriteLine("\nTimeFrameGroup for TwoYears:");
        foreach (var pair in collection.TimeFrameGroup(TimeFrame.TwoYears))
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }

        Console.WriteLine("\nTimeFrameGroup:");
        foreach (var group in collection.TimeFrameGroup())
        {
            Console.WriteLine($"TimeFrame: {group.Key}");
            foreach (var pair in group)
            {
                Console.WriteLine($"  {pair.Key}: {pair.Value.ToShortString()}");
            }
        }
    }
}
