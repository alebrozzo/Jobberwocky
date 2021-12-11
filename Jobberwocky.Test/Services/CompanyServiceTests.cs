using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Jobberwocky.Api.Services;
using Jobberwocky.Api.Services.OperationHandling;
using Jobberwocky.DataAccess;
using Jobberwocky.Test.Helpers;

namespace Jobberwocky.Test.Services
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
    public async Task CanGetCompanyById()
    {
      var companyToCreate = TestDataCreator.Company();
      this.companyRepository.Get(default).ReturnsForAnyArgs(companyToCreate);

      var companyService = this.CreateSut();
      var result = await companyService.Get(companyToCreate.Id);

      Assert.AreEqual(OperationStatus.Success, result.Status);
      await this.companyRepository.Received(1).Get(companyToCreate.Id);
    }

    [Test]
    public async Task GetReturnsValidationErrorWhenIdEmpty()
    {
      var companyService = this.CreateSut();
      var result = await companyService.Get(Guid.Empty);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.companyRepository.DidNotReceiveWithAnyArgs().Get(default);
    }

    [Test]
    public async Task GetReturnsNotFoundWhenIdUnknown()
    {
      var companyId = Guid.NewGuid();
      var companyService = this.CreateSut();
      var result = await companyService.Get(companyId);

      Assert.AreEqual(OperationStatus.NotFound, result.Status);
      await this.companyRepository.Received(1).Get(companyId);
    }

    [TestCase("12345", Description = "Minimum character long")]
    [TestCase("12345678901234567890", Description = "Maximum character long")]
    public async Task CanAddCompanyWhenDataIsValid(string name)
    {
      var companyToCreate = TestDataCreator.Company(name: name);
      this.companyRepository.Add(default).ReturnsForAnyArgs(companyToCreate.Id);

      var companyService = this.CreateSut();
      var result = await companyService.Add(companyToCreate);

      Assert.AreEqual(OperationStatus.Success, result.Status);
      await this.companyRepository.Received(1).Add(companyToCreate);
    }

    [TestCase("1234", Description = "Name too short")]
    [TestCase("123456789012345678901", Description = "Name too long")]
    [TestCase("", Description = "Name is empty")]
    public async Task CannotAddCompanyWhenDataIsNotValid(string name)
    {
      var companyToCreate = TestDataCreator.Company(name: name);
      this.companyRepository.Add(default).ReturnsForAnyArgs(companyToCreate.Id);

      var companyService = this.CreateSut();
      var result = await companyService.Add(companyToCreate);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.companyRepository.DidNotReceiveWithAnyArgs().Add(default);
    }

    [Test]
    public async Task CannotAddCompanyWhenIdlreadyExists()
    {
      var companyToCreate = TestDataCreator.Company();
      this.companyRepository.Get(companyToCreate.Id).ReturnsForAnyArgs(companyToCreate);

      var companyService = this.CreateSut();
      var result = await companyService.Add(companyToCreate);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.companyRepository.DidNotReceiveWithAnyArgs().Add(default);
    }

    [TestCase("12345", Description = "Minimum character long")]
    [TestCase("12345678901234567890", Description = "Maximum character long")]
    public async Task CanUpdateCompanyWhenDataIsValid(string name)
    {
      var companyToUpdate = TestDataCreator.Company(name: name);
      this.companyRepository.Get(companyToUpdate.Id).Returns(companyToUpdate);

      var companyService = this.CreateSut();
      var result = await companyService.Update(companyToUpdate);

      Assert.AreEqual(OperationStatus.Success, result.Status);
      await this.companyRepository.Received(1).Update(companyToUpdate);
    }

    [TestCase("1234", Description = "Name too short")]
    [TestCase("123456789012345678901", Description = "Name too long")]
    public async Task CannotUpdateCompanyWhenDataIsNotValid(string name)
    {
      var companyToUpdate = TestDataCreator.Company(name: name);
      this.companyRepository.Get(companyToUpdate.Id).Returns(companyToUpdate);

      var companyService = this.CreateSut();
      var result = await companyService.Update(companyToUpdate);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.companyRepository.DidNotReceiveWithAnyArgs().Update(default);
    }

    [Test]
    public async Task CannotUpdateCompanyWhenIdIsNotValid()
    {
      var companyToUpdate = TestDataCreator.Company(id: Guid.Empty);
      this.companyRepository.Get(companyToUpdate.Id).Returns(companyToUpdate);

      var companyService = this.CreateSut();
      var result = await companyService.Update(companyToUpdate);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.companyRepository.DidNotReceiveWithAnyArgs().Update(default);
    }

    [Test]
    public async Task UpdateReturnsNotFoundIfIdUnknown()
    {
      var companyToUpdate = TestDataCreator.Company();

      var companyService = this.CreateSut();
      var result = await companyService.Update(companyToUpdate);

      Assert.AreEqual(OperationStatus.NotFound, result.Status);
    }

    [Test]
    public async Task CanDeleteCompanyWhenIdIsValid()
    {
      var companyToUpdate = TestDataCreator.Company();
      this.companyRepository.Get(companyToUpdate.Id).Returns(companyToUpdate);

      var companyService = this.CreateSut();
      var result = await companyService.Delete(companyToUpdate.Id);

      Assert.AreEqual(OperationStatus.Success, result.Status);
      await this.companyRepository.Received(1).Delete(companyToUpdate.Id);
    }

    [Test]
    public async Task CannotDeleteCompanyWhenIdIsNotValid()
    {
      var companyToUpdate = TestDataCreator.Company(id: Guid.Empty);
      this.companyRepository.Get(companyToUpdate.Id).Returns(companyToUpdate);

      var companyService = this.CreateSut();
      var result = await companyService.Delete(companyToUpdate.Id);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.companyRepository.DidNotReceiveWithAnyArgs().Delete(default);
    }

    [Test]
    public async Task CannotDeleteCompanyWhenIdIsUnknown()
    {
      var companyToUpdate = TestDataCreator.Company();

      var companyService = this.CreateSut();
      var result = await companyService.Delete(companyToUpdate.Id);

      Assert.AreEqual(OperationStatus.NotFound, result.Status);
      await this.companyRepository.DidNotReceiveWithAnyArgs().Delete(default);
    }

    private CompanyService CreateSut()
    {
      return new CompanyService(this.companyRepository);
    }
  }
}
