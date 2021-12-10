using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Jobberwocky.Api.Services;
using Jobberwocky.Domain;
using Jobberwocky.Test.Helpers;
using Jobberwocky.Api.Dtos;

namespace Jobberwocky.Test.Services
{
  public class JobberwockyExternalProviderTests
  {
    private const string NoMatches = "[]";

    private IHttpClientFactory httpClientFactory;
    private HttpClient httpClient;
    private MockHttpMessageHandler httpMessageHandler;

    [SetUp]
    public void Setup()
    {
      this.httpMessageHandler = new MockHttpMessageHandler();
      this.httpClient = new HttpClient(this.httpMessageHandler);
      this.httpClientFactory = Substitute.For<IHttpClientFactory>();
      this.httpClientFactory.CreateClient().Returns(this.httpClient);
    }

    [Test]
    public async Task CanCallExternalProviderApi()
    {
      this.httpMessageHandler.SetResponse(NoMatches, HttpStatusCode.OK);

      var jobberwockyProvider = this.CreateSut();
      _ = await jobberwockyProvider.SearchJobPostings(new PostingSearchDto());

      Assert.AreEqual(1, this.httpMessageHandler.NumberOfCalls);
    }

    [TestCase(NoMatches, 0, Description = "No results")]
    [TestCase("[[\"Developer\", 30000, \"Argentina\", [\"OOP\", \"PHP\", \"MySQL\"]]]", 1, Description = "Single result")]
    [TestCase("[[\"Developer\", 30000, \"Argentina\", [\"OOP\", \"PHP\", \"MySQL\"]],[\"Developer\", 40000, \"Brasil\", [\"React\"]]]", 2, Description = "Multiple results")]
    public async Task CanMapResultsToPosting(string results, int count)
    {
      this.httpMessageHandler.SetResponse(results, HttpStatusCode.OK);

      var jobberwockyProvider = this.CreateSut();
      var postings = await jobberwockyProvider.SearchJobPostings(new PostingSearchDto());

      Assert.AreEqual(1, this.httpMessageHandler.NumberOfCalls);
      Assert.AreEqual(count, postings.Count());
    }

    [Test]
    public async Task ResultsAreProperlyMapped()
    {
      var name = Guid.NewGuid().ToString();
      var salary = 12345;
      var country = Guid.NewGuid().ToString();
      var skills = new string[3] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
      var skillsAsString = string.Join("\", \"", skills);
      var results = $"[[\"{name}\", {salary}, \"{country}\", [\"{skillsAsString}\"]]]";
      this.httpMessageHandler.SetResponse(results, HttpStatusCode.OK);

      var jobberwockyProvider = this.CreateSut();
      var postings = await jobberwockyProvider.SearchJobPostings(new PostingSearchDto());

      var posting = postings.First();
      Assert.AreEqual(name, posting.Title);
      Assert.AreEqual(salary, posting.SalaryRangeMax);
      Assert.AreEqual(country, posting.Location);
      CollectionAssert.AreEquivalent(skills, posting.Tags);
    }

    private JobberwockyExternalProvider CreateSut()
    {
      return new JobberwockyExternalProvider(this.httpClientFactory);
    }
  }
}
