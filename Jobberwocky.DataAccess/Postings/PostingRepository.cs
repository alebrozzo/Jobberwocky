using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jobberwocky.Domain;

namespace Jobberwocky.DataAccess
{
  public class PostingRepository : IPostingRepository
  {
    // For the sake of brevity, I am using an In Memory data repository
    // await Task.Yield(); fakes the asyncrony expected from a database or external api call
    private static readonly List<Posting> postings = new List<Posting>
    {
      new Posting {
        Id = Guid.Parse("00000000-bbbb-cccc-dddd-000000000000"),
        Title = "Sr. Software Engineer",
        Location = "Christchurch, New Zealand",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-333333333333"),
        Description = "A great software engineer and person to cover for a recent loss.",
        RemoteAvailable = true,
        SalaryRangeMin = 90000,
        SalaryRangeMax = 110000,
        DateCreated = DateTime.UtcNow.AddDays(-15),
        Tags = new List<string> { "C#", "Agile", "Soft Skills" },
      },
      new Posting {
        Id = Guid.Parse("11111111-bbbb-cccc-dddd-000000000000"),
        Title = "Software Engineer",
        Location = "Barcelona, Spain",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-222222222222"),
        Description = "Main Responsibilities and Duties: blah blah blah",
        RemoteAvailable = true,
        SalaryRangeMin = 40000,
        SalaryRangeMax = 55000,
        DateCreated = DateTime.UtcNow.AddDays(-25),
        Tags = new List<string> { "C#", ".NET", "TypeScript", "Angular" },
      },
      new Posting {
        Id = Guid.Parse("22222222-bbbb-cccc-dddd-000000000000"),
        Title = "Team Leader",
        Location = "Madrid, Spain",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-222222222222"),
        Description = "Main Responsibilities and Duties: blah blah blah",
        RemoteAvailable = true,
        SalaryRangeMin = 45000,
        SalaryRangeMax = 65000,
        DateCreated = DateTime.UtcNow.AddDays(-6),
        Tags = new List<string> { "C#", ".NET", "Soft Skills" },
      },
      new Posting {
        Id = Guid.Parse("33333333-bbbb-cccc-dddd-000000000000"),
        Title = "Account Manager",
        Location = "Madrid, Spain",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-111111111111"),
        Description = "Needs to manage accounts like a pro",
        RemoteAvailable = false,
        SalaryRangeMin = 45000,
        SalaryRangeMax = 65000,
        DateCreated = DateTime.UtcNow.AddDays(-1),
        Tags = new List<string> { "Management", "Soft Skills" },
      },
      new Posting {
        Id = Guid.Parse("44444444-bbbb-cccc-dddd-000000000000"),
        Title = "Sr. Software Engineer",
        Location = "Málaga, Spain",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-000000000000"),
        Description = "We'll take anybody!",
        RemoteAvailable = false,
        SalaryRangeMin = 35000,
        SalaryRangeMax = 55000,
        DateCreated = DateTime.UtcNow.AddDays(-2),
        Tags = new List<string> { "C#", ".NET", "TypeScript", "React", "Soft Skills", "JavaScript" },
      },
      new Posting {
        Id = Guid.Parse("55555555-bbbb-cccc-dddd-000000000000"),
        Title = "Jr. Developer - Frontend",
        Location = "Comodoro Rivadavia, Argentina",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-000000000000"),
        Description = "Winds of change",
        RemoteAvailable = false,
        SalaryRangeMin = 15000,
        SalaryRangeMax = 25000,
        DateCreated = DateTime.UtcNow.AddDays(-7),
        Tags = new List<string> { "TypeScript", "React", "JavaScript" },
      },
      new Posting {
        Id = Guid.Parse("66666666-bbbb-cccc-dddd-000000000000"),
        Title = "Jr. Developer - Backend",
        Location = "Comodoro Rivadavia, Argentina",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-000000000000"),
        Description = "Winds of change backend",
        RemoteAvailable = false,
        SalaryRangeMin = 15000,
        SalaryRangeMax = 25000,
        DateCreated = DateTime.UtcNow.AddDays(-7),
        Tags = new List<string> { "C#", ".NET" },
      },
      new Posting {
        Id = Guid.Parse("77777777-bbbb-cccc-dddd-000000000000"),
        Title = "Project Manager",
        Location = "Auckland, New Zealand",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-333333333333"),
        Description = "Come and join us!",
        RemoteAvailable = true,
        SalaryRangeMin = 45000,
        SalaryRangeMax = 55000,
        DateCreated = DateTime.UtcNow.AddDays(-9),
        Tags = new List<string> { "Management", "Soft Skills" },
      },
      new Posting {
        Id = Guid.Parse("88888888-bbbb-cccc-dddd-000000000000"),
        Title = "Kotlin developer",
        Location = "Barcelona, Spain",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-000000000000"),
        Description = "A great software engineer and person to cover for a recent loss.",
        RemoteAvailable = true,
        SalaryRangeMin = 40000,
        SalaryRangeMax = 450000,
        DateCreated = DateTime.UtcNow.AddDays(-1),
        Tags = new List<string> { "Kotlin", "Agile", "Soft Skills" },
      },
      new Posting {
        Id = Guid.Parse("99999999-bbbb-cccc-dddd-000000000000"),
        Title = "Jr. Backend Engineed",
        Location = "Madrid, Spain",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-111111111111"),
        Description = "Urgently looking for a person to fill our role!",
        RemoteAvailable = true,
        SalaryRangeMin = 45000,
        SalaryRangeMax = 50000,
        DateCreated = DateTime.UtcNow.AddDays(-2),
        Tags = new List<string> { "Java", "English" },
      },
    };

    public async Task<Posting> Get(Guid id)
    {
      await Task.Yield();
      return postings.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Guid> Add(Posting posting)
    {
      await Task.Yield();
      if (posting.Id == Guid.Empty)
      {
        posting.Id = Guid.NewGuid();
      }

      posting.DateCreated = DateTime.UtcNow;
      postings.Add(posting);

      return posting.Id;
    }

    public async Task Update(Posting posting)
    {
      await Task.Yield();
      posting.DateCreated = DateTime.UtcNow; // since the value does not come from controller, fake it here. It shouldn't be updated in real life.
      var ix = postings.FindIndex(c => c.Id == posting.Id);
      postings[ix] = posting;
    }

    public async Task Delete(Guid id)
    {
      await Task.Yield();
      var ix = postings.FindIndex(c => c.Id == id);
      postings.RemoveAt(ix);
    }

    public async Task<IEnumerable<Posting>> Search(
      string keywords,
      string location,
      bool remote,
      decimal? salaryMin,
      IEnumerable<string> tags)
    {
      await Task.Yield();
      var keywordList = keywords?.Split(' ', StringSplitOptions.RemoveEmptyEntries).AsEnumerable();
      var returnPostings = postings.Where(p => this.Matches(p, keywordList, location, remote, salaryMin, tags));
      return returnPostings;
    }

    private bool Matches(
      Posting posting,
      IEnumerable<string> keywords,
      string location,
      bool remote,
      decimal? salaryMin,
      IEnumerable<string> tags)
    {
      if (remote && posting.RemoteAvailable.HasValue && !posting.RemoteAvailable.Value)
      {
        return false;
      }

      // if user requested a minimum salary, check that it is lower than the offered by posting.
      // if posting does not include a range, then do not discard posting.
      if (salaryMin.HasValue && posting.SalaryRangeMax.HasValue && posting.SalaryRangeMax.Value < salaryMin.Value)
      {
        return false;
      }

      if (!string.IsNullOrWhiteSpace(location) && !posting.Location.Contains(location, StringComparison.OrdinalIgnoreCase))
      {
        return false;
      }

      if (keywords != null && keywords.Count() > 0)
      {
        var titleList = posting.Title.Split(' ', StringSplitOptions.RemoveEmptyEntries).AsEnumerable();
        bool containsKeyword = titleList.Any(word => keywords.Contains(word, StringComparer.OrdinalIgnoreCase));
        if (!containsKeyword)
        {
          return false;
        }
      }

      if (tags != null && tags.Count() > 0)
      {
        bool containsTag = posting.Tags.Any(tag => tags.Contains(tag, StringComparer.OrdinalIgnoreCase));
        if (!containsTag)
        {
          return false;
        }
      }

      return true;
    }
  }
}
