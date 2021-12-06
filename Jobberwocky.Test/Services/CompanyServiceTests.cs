using System;
using NUnit.Framework;
using NSubstitute;
using Jobberwocky.Api.Services;
using Jobberwocky.DataAccess;
using Jobberwocky.Domain;
using Jobberwocky.Test.Helpers;
using System.Threading.Tasks;

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
    public void CanAddCompanyWhenDataIsValid()
    {
      var companyToCreate = TestDataCreator.Company();
      this.companyRepository.Add(default).ReturnsForAnyArgs(Task.FromResult(companyToCreate.Id));

      var companyService = this.CreateSut();
      _ = companyService.Add(companyToCreate);

      this.companyRepository.Received(1).Add(companyToCreate);
    }

    private CompanyService CreateSut()
    {
      return new CompanyService(this.companyRepository);
    }
  }
}
