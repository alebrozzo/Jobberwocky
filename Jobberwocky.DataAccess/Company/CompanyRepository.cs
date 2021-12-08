using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jobberwocky.Domain;

namespace Jobberwocky.DataAccess
{
  public class CompanyRepository : ICompanyRepository
  {
    // For the sake of brevity, I am using an In Memory data repository
    // await Task.Yield(); fakes the asyncrony expected from a database or external api call
    private static readonly List<Company> companies = new List<Company>
    {
      new Company { Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-000000000000"), Name = "Avature", Description = "The coolest company." },
      new Company { Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-111111111111"), Name = "Netflix", Description = "You'll work with us and then go home and still be with us." },
      new Company { Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-222222222222"), Name = "Glovo", Description = "Bike your way up." },
      new Company { Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-333333333333"), Name = "ARANZ Medical", Description = "A health technology company." },
    };

    public async Task<Company> Get(Guid id)
    {
      await Task.Yield();
      return companies.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Guid> Add(Company company)
    {
      await Task.Yield();
      if (company.Id == Guid.Empty)
      {
        company.Id = Guid.NewGuid();
      }

      companies.Add(company);

      return company.Id;
    }

    public async Task Update(Company company)
    {
      await Task.Yield();
      var ix = companies.FindIndex(c => c.Id == company.Id);
      companies[ix] = company;
    }

    public async Task Delete(Guid id)
    {
      await Task.Yield();
      var ix = companies.FindIndex(c => c.Id == id);
      companies.RemoveAt(ix);
    }
  }
}
