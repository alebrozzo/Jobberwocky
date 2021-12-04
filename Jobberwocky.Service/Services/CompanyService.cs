using Jobberwocky.DataAccess.Company;

namespace Jobberwocky.Api.Services
{
  public class CompanyService : ICompanyService
  {
    private readonly ICompanyRepository companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
      this.companyRepository = companyRepository ?? throw new System.ArgumentNullException(nameof(companyRepository));
    }

    public void Add()
    {
      this.companyRepository.Add();
    }
  }
}
