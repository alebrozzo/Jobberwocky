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
    private static readonly List<Company> companies = new List<Company>();

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
