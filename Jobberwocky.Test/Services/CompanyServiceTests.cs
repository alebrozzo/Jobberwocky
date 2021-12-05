using NUnit.Framework;
using NSubstitute;
using Jobberwocky.Api.Services;
using Jobberwocky.DataAccess;
using Jobberwocky.Domain;

namespace Jobberwocky.Test
{
  public class CompanyServiceTests
  {
    private ICompanyRepository companyRepository;

    [SetUp]
    public void Setup()
    {
      this.companyRepository = Substitute.For<ICompanyRepository>();
    }

    [Test]
    public void CanAddCompany()
    {
      var companyCreated = new Company();
      this.companyRepository.Add(default).ReturnsForAnyArgs(companyCreated);

      var companyService = this.CreateSut();
      var newCompany = new Company();
      _ = companyService.Add(newCompany);

      this.companyRepository.Received(1).Add(newCompany);
    }

    private CompanyService CreateSut()
    {
      return new CompanyService(this.companyRepository);
    }
  }
}
