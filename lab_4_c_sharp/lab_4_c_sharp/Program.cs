using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

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

public enum Revision
{
    Remove,
    Replace,
    Property
}

public class ResearchTeamsChangedEventArgs<TKey> : EventArgs
{
    public string CollectionName { get; }
    public Revision ChangeType { get; }
    public string PropertyName { get; }
    public int RegistrationNumber { get; }

    public ResearchTeamsChangedEventArgs(string collectionName, Revision changeType, string propertyName, int registrationNumber)
    {
        CollectionName = collectionName;
        ChangeType = changeType;
        PropertyName = propertyName;
        RegistrationNumber = registrationNumber;
    }

    public override string ToString()
    {
        return $"{CollectionName} - {ChangeType}: {PropertyName} ({RegistrationNumber})";
    }
}

public delegate void ResearchTeamsChangedHandler<TKey>(object source, ResearchTeamsChangedEventArgs<TKey> args);

class ResearchTeam : Team, INameAndCopy, INotifyPropertyChanged
{
    private string researchTopic;
    private TimeFrame timeFrame;
    private List<Person> members;
    private List<Paper> publications = new List<Paper>();

    public string ResearchTopic
    {
        get { return researchTopic; }
        set
        {
            if (researchTopic != value)
            {
                researchTopic = value;
                OnPropertyChanged();
            }
        }
    }

    public TimeFrame TimeFrame
    {
        get { return timeFrame; }
        set
        {
            if (timeFrame != value)
            {
                timeFrame = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

/*    public TimeFrame TimeFrame
    {
        get { return timeFrame; }
        set { timeFrame = value; }
    }*/

    public List<Paper> Publications
    {
        get { return publications; }
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
    private Dictionary<TKey, ResearchTeam> teamsDictionary = new Dictionary<TKey, ResearchTeam>();
    private ResearchTeamsChangedHandler<TKey> researchTeamsChangedHandler;

    public string CollectionName { get; }

    public ResearchTeamCollection(string collectionName)
    {
        CollectionName = collectionName;
    }

    public event ResearchTeamsChangedHandler<TKey> ResearchTeamsChanged
    {
        add => researchTeamsChangedHandler += value;
        remove => researchTeamsChangedHandler -= value;
    }

    public bool Remove(ResearchTeam rt)
    {
        var key = teamsDictionary.FirstOrDefault(x => x.Value == rt).Key;
        if (key != null)
        {
            teamsDictionary.Remove(key);
            OnResearchTeamsChanged(Revision.Remove, "", rt.RegistrationNumber);
            return true;
        }
        return false;
    }

    public bool Replace(ResearchTeam rtold, ResearchTeam rtnew)
    {
        var key = teamsDictionary.FirstOrDefault(x => x.Value == rtold).Key;
        if (key != null)
        {
            teamsDictionary[key] = rtnew;
            OnResearchTeamsChanged(Revision.Replace, "", rtold.RegistrationNumber);
            return true;
        }
        return false;
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
            TKey key = GetKey(team);

            if (key != null)
            {
                ResearchTeam existingTeam = GetTeamByKey(key);

                if (existingTeam == null)
                {
                    teamsDictionary.Add(key, team);
                    OnResearchTeamsChanged(Revision.Property, "ResearchTeam added", team.RegistrationNumber);
                }
                else
                {
                    Console.WriteLine($"Error adding team: Team with key '{key}' already exists.");
                }
            }
            else
            {
                Console.WriteLine($"Error adding team: Invalid key for team.");
            }
        }
    }



    private TKey GetKey(ResearchTeam team)
    {
        if (team == null)
        {
            throw new ArgumentNullException(nameof(team), "Team cannot be null");
        }

        TKey key = GenerateKeyForTeam(team);

        if (teamsDictionary.ContainsKey(key))
        {
            Console.WriteLine($"Error adding team: Team with key '{key}' already exists.");
            throw new InvalidOperationException("Team already exists in dictionary");
        }

        return key;
    }



    private TKey GenerateKeyForTeam(ResearchTeam team)
    {
        // Assuming ResearchTeam has properties Organization and RegistrationNumber
        // You should modify this based on the actual properties you want to include in the key.
        return (TKey)(object)$"{team.Organization}_{team.RegistrationNumber}";
    }



    public ResearchTeam GetTeamByKey(TKey key)
    {
        if (teamsDictionary.TryGetValue(key, out var team))
        {
            return team;
        }

        // Если ключ не найден, можно вернуть null или сгенерировать исключение, в зависимости от ваших потребностей.
        return null;
    }


    public DateTime LastPublicationDate
    {
        get
        {
            if (teamsDictionary.Count == 0 || teamsDictionary.All(pair => pair.Value.Publications.Count == 0))
                return default(DateTime);

            return teamsDictionary.SelectMany(pair => pair.Value.Publications)
                                  .Max(publication => publication.PublicationDate);
        }
    }


    public IEnumerable<KeyValuePair<TKey, ResearchTeam>> TimeFrameGroup(TimeFrame value)
    {
        return teamsDictionary.Where(pair => pair.Value.TimeFrame == value);
    }

    public IEnumerable<IGrouping<TimeFrame, KeyValuePair<TKey, ResearchTeam>>> TimeFrameGroup()
    {
        return teamsDictionary.GroupBy(pair => pair.Value.TimeFrame);
    }

    public override string ToString()
    {
        return string.Join("\n", teamsDictionary.Select(pair => $"{pair.Key}: {pair.Value}"));
    }

    public string ToShortString()
    {
        return string.Join("\n", teamsDictionary.Select(pair => $"{pair.Key}: {pair.Value.ToShortString()}"));
    }

    private void OnResearchTeamsChanged(Revision changeType, string propertyName, int registrationNumber)
    {
        researchTeamsChangedHandler?.Invoke(this, new ResearchTeamsChangedEventArgs<TKey>(CollectionName, changeType, propertyName, registrationNumber));
    }

}

public class TeamsJournalEntry
{
    public string CollectionName { get; }
    public Revision ChangeType { get; }
    public string PropertyName { get; }
    public int RegistrationNumber { get; }

    public TeamsJournalEntry(string collectionName, Revision changeType, string propertyName, int registrationNumber)
    {
        CollectionName = collectionName;
        ChangeType = changeType;
        PropertyName = propertyName;
        RegistrationNumber = registrationNumber;
    }

    public override string ToString()
    {
        return $"{CollectionName} - {ChangeType}: {PropertyName} ({RegistrationNumber})";
    }
}

public class TeamsJournal
{
    private List<TeamsJournalEntry> journalEntries = new List<TeamsJournalEntry>();

    public TeamsJournal()
    {
    }

    public void HandleResearchTeamsChanged<TKey>(object source, ResearchTeamsChangedEventArgs<TKey> args)
    {
        var entry = new TeamsJournalEntry(args.CollectionName, args.ChangeType, args.PropertyName, args.RegistrationNumber);
        journalEntries.Add(entry);
    }

    public override string ToString()
    {
        return string.Join("\n", journalEntries);
    }
}

class Program
{
    static void Main()
    {
        // Step 1: Create two ResearchTeamCollection<string>
        var collection1 = new ResearchTeamCollection<string>("Collection1");
        var collection2 = new ResearchTeamCollection<string>("Collection2");

        // Step 2: Create TeamsJournal and subscribe to ResearchTeamsChanged events
        var journal = new TeamsJournal();
        collection1.ResearchTeamsChanged += journal.HandleResearchTeamsChanged;
        collection2.ResearchTeamsChanged += journal.HandleResearchTeamsChanged;

        // Step 3: Make changes to the collections
        // Add elements
        collection1.AddResearchTeams(new ResearchTeam("Team1", "Org1", 1, TimeFrame.Year));
        collection2.AddResearchTeams(new ResearchTeam("Team2", "Org2", 2, TimeFrame.TwoYears));

        // Modify values
        var team1 = collection1.GetTeamByKey("Team1");
        if (team1 != null)
        {
            team1.ResearchTopic = "NewTopic";
        }

        var team2 = collection2.GetTeamByKey("Team2");
        if (team2 != null)
        {
            team2.ResearchTopic = "ModifiedTopic";
        }

        // Remove an element
        collection1.Remove(team1);

        // Replace an element
        var newTeam = new ResearchTeam("NewTeam", "Org3", 3, TimeFrame.Long);
        collection2.Replace(team2, newTeam);

        // Step 4: Display TeamsJournal data
        Console.WriteLine(journal.ToString());
    }
}

