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

    [TestCase(false, TestName = "When ID is not provided")]
    [TestCase(true, TestName = "When ID is provided")]
    [Order(1)]
    public async Task AddsCompany(bool idProvided)
    {
      var companyId = idProvided ? Guid.NewGuid() : Guid.Empty;
      var company = TestDataCreator.Company(id: companyId);
      var sut = this.CreateSut();

      var companyIdReturned = await sut.Add(company);
      
      if (idProvided)
      {
        Assert.AreEqual(companyId, companyIdReturned);
      }

      var addedCompany = await sut.Get(companyIdReturned);
      Assert.AreNotEqual(Guid.Empty, companyIdReturned);
      Assert.AreEqual(companyIdReturned, addedCompany.Id);
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
