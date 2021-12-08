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
        Location = "Christchurch",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-333333333333"),
        Description = "A great software engineer and person to cover for a recent loss.",
        RemoteAvailable = true,
        SalaryRangeMin = 90000,
        SalaryRangeMax = 110000,
        Tags = new List<string> { "C#", "Agile", "Soft Skills" },
      },
      new Posting {
        Id = Guid.Parse("11111111-bbbb-cccc-dddd-000000000000"),
        Title = "Software Engineer",
        Location = "Barcelona",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-222222222222"),
        Description = "Main Responsibilities and Duties: blah blah blah",
        RemoteAvailable = true,
        SalaryRangeMin = 40000,
        SalaryRangeMax = 55000,
        Tags = new List<string> { "C#", ".NET", "TypeScript", "Angular" },
      },
      new Posting {
        Id = Guid.Parse("22222222-bbbb-cccc-dddd-000000000000"),
        Title = "Team Leader",
        Location = "Madrid",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-222222222222"),
        Description = "Main Responsibilities and Duties: blah blah blah",
        RemoteAvailable = true,
        SalaryRangeMin = 45000,
        SalaryRangeMax = 65000,
        Tags = new List<string> { "C#", ".NET", "Soft Skills" },
      },
      new Posting {
        Id = Guid.Parse("33333333-bbbb-cccc-dddd-000000000000"),
        Title = "Account Manager",
        Location = "Madrid",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-111111111111"),
        Description = "Needs to manage accounts like a pro",
        RemoteAvailable = false,
        SalaryRangeMin = 45000,
        SalaryRangeMax = 65000,
        Tags = new List<string> { "Management", "Soft Skills" },
      },
      new Posting {
        Id = Guid.Parse("44444444-bbbb-cccc-dddd-000000000000"),
        Title = "Sr. Software Engineer",
        Location = "Málaga",
        CompanyId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-000000000000"),
        Description = "We'll take anybody!",
        RemoteAvailable = false,
        SalaryRangeMin = 35000,
        SalaryRangeMax = 55000,
        Tags = new List<string> { "C#", ".NET", "TypeScript", "React", "Soft Skills", "JavaScript" },
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
      var ix = postings.FindIndex(c => c.Id == posting.Id);
      postings[ix] = posting;
    }

    public async Task Delete(Guid id)
    {
      await Task.Yield();
      var ix = postings.FindIndex(c => c.Id == id);
      postings.RemoveAt(ix);
    }
  }
}
