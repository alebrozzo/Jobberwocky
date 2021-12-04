using Jobberwocky.DataAccess;
using Jobberwocky.Domain;

namespace Jobberwocky.Api.Services
{
  public class CompanyService : ICompanyService
  {
    private readonly ICompanyRepository companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
      this.companyRepository = companyRepository ?? throw new System.ArgumentNullException(nameof(companyRepository));
    }

    public Company Add(Company company)
    {
      return this.companyRepository.Add(company);
    }
  }
}
