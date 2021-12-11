using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Jobberwocky.Api.Dtos;
using Jobberwocky.Api.Services;
using Jobberwocky.Api.Services.OperationHandling;
using Jobberwocky.DataAccess;
using Jobberwocky.Domain;
using Jobberwocky.Test.Helpers;

namespace Jobberwocky.Test.Services
{
  public class PostingServiceTests
  {
    private IPostingRepository postingRepository;
    private ICompanyRepository companyRepository;
    private IEnumerable<ISearchExternalProvider> externalProviders;

    private Company defaultCompany;

    [SetUp]
    public void Setup()
    {
      this.postingRepository = Substitute.For<IPostingRepository>();
      this.companyRepository = Substitute.For<ICompanyRepository>();
      this.externalProviders = Substitute.For<IEnumerable<ISearchExternalProvider>>();
      this.defaultCompany = TestDataCreator.Company();
      this.companyRepository.Get(this.defaultCompany.Id).Returns(defaultCompany);
    }

    [Test]
    public async Task CanGetPostingById()
    {
      var postingToCreate = TestDataCreator.Posting(companyId: defaultCompany.Id);
      this.postingRepository.Get(default).ReturnsForAnyArgs(postingToCreate);

      var postingService = this.CreateSut();
      var result = await postingService.Get(postingToCreate.Id);

      Assert.AreEqual(OperationStatus.Success, result.Status);
      Assert.AreEqual(defaultCompany.Name, result.Result.CompanyName);
      await this.postingRepository.Received(1).Get(postingToCreate.Id);
    }

    [Test]
    public async Task GetReturnsValidationErrorWhenIdEmpty()
    {
      var postingService = this.CreateSut();
      var result = await postingService.Get(Guid.Empty);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.postingRepository.DidNotReceiveWithAnyArgs().Get(default);
    }

    [Test]
    public async Task GetReturnsNotFoundWhenIdUnknown()
    {
      var postingId = Guid.NewGuid();
      var postingService = this.CreateSut();
      var result = await postingService.Get(postingId);

      Assert.AreEqual(OperationStatus.NotFound, result.Status);
      await this.postingRepository.Received(1).Get(postingId);
    }

    [TestCase("12345", "12345", 100, 200, Description = "Title and description minimum character long")]
    [TestCase("12345678", "12345678", null, null, Description = "No salary range")]
    [TestCase("12345678", "12345678", 100, null, Description = "No maximum salary")]
    [TestCase("12345678", "12345678", null, 200, Description = "No minimum salary")]
    public async Task CanAddPostingWhenDataIsValid(string title, string description, decimal? salaryMin, decimal? salaryMax)
    {
      var postingToCreate = TestDataCreator.Posting(companyId: defaultCompany.Id, title: title, description: description, salaryMin: salaryMin, salaryMax: salaryMax);
      postingToCreate.Id = Guid.Empty;
      var createdId = Guid.NewGuid();
      this.postingRepository.Add(default).ReturnsForAnyArgs(createdId);
      this.postingRepository.Get(default).ReturnsForAnyArgs(postingToCreate);

      var postingService = this.CreateSut();
      var result = await postingService.Add(postingToCreate);

      Assert.AreEqual(OperationStatus.Success, result.Status);
      await this.postingRepository.Received(1).Add(postingToCreate);
    }

    [TestCase("", "Valid description", 100, 200, Description = "Title blank")]
    [TestCase("          ", "Valid description", 100, 200, Description = "Title empty spaces")]
    [TestCase("Inv", "Valid description", 100, 200, Description = "Title too short")]
    [TestCase("Valid Title", "", 100, 200, Description = "Description blank")]
    [TestCase("Valid Title", "        ", 100, 200, Description = "Description empty spaces")]
    [TestCase("Valid Title", "1234", 100, 200, Description = "Description too short")]
    [TestCase("Valid Title", "12345678", -100, 200, Description = "Negative minimum salary")]
    [TestCase("Valid Title", "12345678", null, -200, Description = "Negative maximum salary")]
    [TestCase("Valid Title", "12345678", 200, 100, Description = "Salary range invalid")]
    public async Task CannotAddPostingWhenDataIsNotValid(string title, string description, decimal? salaryMin, decimal? salaryMax)
    {
      var postingToCreate = TestDataCreator.Posting(companyId: defaultCompany.Id, title: title, description: description, salaryMin: salaryMin, salaryMax: salaryMax);
      this.postingRepository.Add(default).ReturnsForAnyArgs(postingToCreate.Id);

      var postingService = this.CreateSut();
      var result = await postingService.Add(postingToCreate);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.postingRepository.DidNotReceiveWithAnyArgs().Add(default);
    }

    [Test]
    public async Task CannotAddPostingWhenIdAlreadyExists()
    {
      var postingToCreate = TestDataCreator.Posting(companyId: defaultCompany.Id);
      this.postingRepository.Get(postingToCreate.Id).ReturnsForAnyArgs(postingToCreate);

      var postingService = this.CreateSut();
      var result = await postingService.Add(postingToCreate);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.postingRepository.DidNotReceiveWithAnyArgs().Add(default);
    }

    [Test]
    public async Task CannotAddPostingWhenCompanyDoesNotExist()
    {
      var postingToCreate = TestDataCreator.Posting(companyId: Guid.NewGuid());
      this.postingRepository.Get(postingToCreate.Id).ReturnsForAnyArgs(postingToCreate);
      this.companyRepository.Get(postingToCreate.CompanyId.Value).Returns((Company)null);

      var postingService = this.CreateSut();
      var result = await postingService.Add(postingToCreate);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.postingRepository.DidNotReceiveWithAnyArgs().Add(default);
    }

    [TestCase("12345", 100, 200, Description = "Minimum character long")]
    [TestCase("12345678901234567890", 100, 200, Description = "Maximum character long")]
    [TestCase("12345678", null, null, Description = "No salary range")]
    [TestCase("12345678", 100, null, Description = "No maximum salary")]
    [TestCase("12345678", null, 200, Description = "No minimum salary")]
    public async Task CanUpdatePostingWhenDataIsValid(string description, decimal? salaryMin, decimal? salaryMax)
    {
      var postingToUpdate = TestDataCreator.Posting(companyId: defaultCompany.Id, description: description, salaryMin: salaryMin, salaryMax: salaryMax);
      this.postingRepository.Get(postingToUpdate.Id).Returns(postingToUpdate);

      var postingService = this.CreateSut();
      var result = await postingService.Update(postingToUpdate);

      Assert.AreEqual(OperationStatus.Success, result.Status);
      await this.postingRepository.Received(1).Update(postingToUpdate);
    }

    [TestCase("", "Valid description", 100, 200, Description = "Title blank")]
    [TestCase("          ", "Valid description", 100, 200, Description = "Title empty spaces")]
    [TestCase("Inv", "Valid description", 100, 200, Description = "Title too short")]
    [TestCase("Valid Title", "", 100, 200, Description = "Description blank")]
    [TestCase("Valid Title", "        ", 100, 200, Description = "Description empty spaces")]
    [TestCase("Valid Title", "1234", 100, 200, Description = "Description too short")]
    [TestCase("Valid Title", "12345678", -100, 200, Description = "Negative minimum salary")]
    [TestCase("Valid Title", "12345678", null, -200, Description = "Negative maximum salary")]
    [TestCase("Valid Title", "12345678", 200, 100, Description = "Salary range invalid")]
    public async Task CanUpdatePostingWhenDataIsValid(string title, string description, decimal? salaryMin, decimal? salaryMax)
    {
      var postingToUpdate = TestDataCreator.Posting(companyId: defaultCompany.Id, title: title, description: description, salaryMin: salaryMin, salaryMax: salaryMax);
      this.postingRepository.Get(postingToUpdate.Id).Returns(postingToUpdate);

      var postingService = this.CreateSut();
      var result = await postingService.Update(postingToUpdate);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.postingRepository.DidNotReceiveWithAnyArgs().Update(default);
    }

    [Test]
    public async Task CannotUpdatePostingWhenIdIsNotValid()
    {
      var postingToUpdate = TestDataCreator.Posting(id: Guid.Empty);
      this.postingRepository.Get(postingToUpdate.Id).Returns(postingToUpdate);

      var postingService = this.CreateSut();
      var result = await postingService.Update(postingToUpdate);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.postingRepository.DidNotReceiveWithAnyArgs().Update(default);
    }

    [Test]
    public async Task UpdateReturnsNotFoundIfIdUnknown()
    {
      var postingToUpdate = TestDataCreator.Posting(companyId: defaultCompany.Id);

      var postingService = this.CreateSut();
      var result = await postingService.Update(postingToUpdate);

      Assert.AreEqual(OperationStatus.NotFound, result.Status);
    }

    [Test]
    public async Task CanDeletePostingWhenIdIsValid()
    {
      var postingToUpdate = TestDataCreator.Posting(companyId: defaultCompany.Id);
      this.postingRepository.Get(postingToUpdate.Id).Returns(postingToUpdate);

      var postingService = this.CreateSut();
      var result = await postingService.Delete(postingToUpdate.Id);

      Assert.AreEqual(OperationStatus.Success, result.Status);
      await this.postingRepository.Received(1).Delete(postingToUpdate.Id);
    }

    [Test]
    public async Task CannotDeletePostingWhenIdIsNotValid()
    {
      var postingToUpdate = TestDataCreator.Posting(id: Guid.Empty);
      this.postingRepository.Get(postingToUpdate.Id).Returns(postingToUpdate);

      var postingService = this.CreateSut();
      var result = await postingService.Delete(postingToUpdate.Id);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.postingRepository.DidNotReceiveWithAnyArgs().Delete(default);
    }

    [Test]
    public async Task CannotDeletePostingWhenIdIsUnknown()
    {
      var postingToUpdate = TestDataCreator.Posting(companyId: defaultCompany.Id);

      var postingService = this.CreateSut();
      var result = await postingService.Delete(postingToUpdate.Id);

      Assert.AreEqual(OperationStatus.NotFound, result.Status);
      await this.postingRepository.DidNotReceiveWithAnyArgs().Delete(default);
    }

    [TestCase("word1 word2", "somewhere", true, 54321, "tag1,tag2", Description = "All criteria set")]
    [TestCase("word1", null, false, null, "", Description = "Minimal criteria set")]
    public async Task CanSearchForPostings(
      string keywords,
      string location,
      bool remoteAllowed,
      decimal? salaryMin,
      string tags)
    {
      var posting1 = TestDataCreator.Posting(companyId: defaultCompany.Id);
      var posting2 = TestDataCreator.Posting(companyId: defaultCompany.Id);
      var searchResult = new List<Posting>(2) { posting1, posting2 };
      var searchCriteria = new PostingSearchDto
      {
        Keywords = keywords,
        Location = location,
        RemoteAllowed = remoteAllowed,
        SalaryMin = salaryMin,
        Tags = tags.Split(','),
      };

      this.postingRepository
        .Search(searchCriteria.Keywords, searchCriteria.Location, searchCriteria.RemoteAllowed, searchCriteria.SalaryMin, searchCriteria.Tags)
        .Returns(searchResult);

      var postingService = this.CreateSut();
      var result = await postingService.Search(searchCriteria);

      Assert.AreEqual(OperationStatus.Success, result.Status);
      Assert.AreEqual(searchResult.Count, result.Result.Count());
    }

    [Test]
    public async Task SearchValidatesCriteriaIsNotNull()
    {
      var postingService = this.CreateSut();
      var result = await postingService.Search(null);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.postingRepository.DidNotReceiveWithAnyArgs().Search(default, default, default, default, default);
    }

    [TestCase(null, Description = "Null value")]
    [TestCase("", Description = "Empty string")]
    [TestCase("                ", Description = "Spaces")]
    public async Task SearchValidatesKeywordPresent(string keywords)
    {
      var searchCriteria = new PostingSearchDto { Keywords = keywords };

      var postingService = this.CreateSut();
      var result = await postingService.Search(searchCriteria);

      Assert.AreEqual(OperationStatus.ValidationError, result.Status);
      await this.postingRepository.DidNotReceiveWithAnyArgs().Search(default, default, default, default, default);
    }

    private PostingService CreateSut()
    {
      return new PostingService(this.postingRepository, this.companyRepository, this.externalProviders);
    }
  }
}
