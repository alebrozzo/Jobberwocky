using Jobberwocky.Domain;
using System;
using System.Collections.Generic;

namespace Jobberwocky.Test.Helpers
{
  public static class TestDataCreator
  {
    public static Company Company(Guid? id = null, string name = null, string description = null)
    {
      return new Company()
      {
        Id = id ?? Guid.NewGuid(),
        Name = name ?? Guid.NewGuid().ToString().Substring(0, 20),
        Description = description ?? Guid.NewGuid().ToString(),
      };
    }

    public static Posting Posting(
      Guid? id = null,
      string description = null,
      string location = null,
      Guid? companyId = null,
      bool remoteAvailable = false,
      decimal? salariMin = 12000,
      decimal? salaryMax = 55000,
      params string[] tags)
    {
      var tagsToUse = new List<string>(tags);
      if (tagsToUse.Count == 0)
      {
        tagsToUse.Add(Guid.NewGuid().ToString().Substring(0, 5));
        tagsToUse.Add(Guid.NewGuid().ToString().Substring(0, 5));
      }

      return new Posting()
      {
        Id = id ?? Guid.NewGuid(),
        Description = description ?? Guid.NewGuid().ToString(),
        CompanyId = companyId ?? Guid.NewGuid(),
        Location = location ?? Guid.NewGuid().ToString(),
        RemoteAvailable = remoteAvailable,
        SalaryRangeMin = salariMin,
        SalaryRangeMax = salaryMax,
        Tags = new List<string>(tags),
        DateCreated = DateTime.UtcNow,
      };
    }
  }
}
