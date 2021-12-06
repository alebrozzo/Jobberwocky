using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Jobberwocky.DataAccess;
using Jobberwocky.Domain;
using Jobberwocky.Test.Helpers;

namespace Jobberwocky.Test.Repositories
{
  public class CompanyRepositoryTests
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [Order(1)]
    public async Task AddsCompany()
    {
      var company = TestDataCreator.Company(id: Guid.Empty);
      var sut = this.CreateSut();

      var companyId = await sut.Add(company);

      var addedCompany = await sut.Get(companyId);
      Assert.AreNotEqual(Guid.Empty, companyId);
      Assert.AreEqual(companyId, addedCompany.Id);
      this.AssertCompany(company, addedCompany);
    }

    [Test]
    [Order(2)]
    public async Task GetsCompanyById()
    {
      var storedCompany = TestDataCreator.Company();
      var sut = this.CreateSut();
      _ = sut.Add(storedCompany);

      var retrievedCompany = await sut.Get(storedCompany.Id);

      Assert.AreEqual(storedCompany.Id, retrievedCompany.Id);
      this.AssertCompany(storedCompany, retrievedCompany);
    }


    [Test]
    [Order(3)]
    public async Task UpdatesCompany()
    {
      var storedCompany = TestDataCreator.Company();
      var sut = this.CreateSut();
      _ = sut.Add(storedCompany);

      storedCompany = TestDataCreator.Company(id: storedCompany.Id);
      await sut.Update(storedCompany);

      var retrievedCompany = await sut.Get(storedCompany.Id);

      this.AssertCompany(storedCompany, retrievedCompany);
    }

    [Test]
    [Order(4)]
    public async Task DeletesCompany()
    {
      var storedCompany = TestDataCreator.Company();
      var sut = this.CreateSut();
      _ = sut.Add(storedCompany);

      await sut.Delete(storedCompany.Id);

      var retrievedCompany = await sut.Get(storedCompany.Id);

      Assert.IsNull(retrievedCompany);
    }

    private void AssertCompany(Company expected, Company actual)
    {
      Assert.AreEqual(expected.Name, actual.Name);
      Assert.AreEqual(expected.Description, actual.Description);
    }

    private CompanyRepository CreateSut()
    {
      return new CompanyRepository();
    }
  }
}
