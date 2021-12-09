using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Jobberwocky.DataAccess;
using Jobberwocky.Domain;
using Jobberwocky.Test.Helpers;

namespace Jobberwocky.Test.Repositories
{
  public class PostingRepositoryTests
  {
    [SetUp]
    public void Setup()
    {
    }

    [TestCase(false, Description = "When ID is not provided")]
    [TestCase(true, Description = "When ID is provided")]
    [Order(1)]
    public async Task AddsPosting(bool idProvided)
    {
      var postingId = idProvided ? Guid.NewGuid() : Guid.Empty;
      var posting = TestDataCreator.Posting(id: postingId);
      var sut = this.CreateSut();

      var postingIdReturned = await sut.Add(posting);

      if (idProvided)
      {
        Assert.AreEqual(postingId, postingIdReturned);
      }

      var addedPosting = await sut.Get(postingIdReturned);
      Assert.AreNotEqual(Guid.Empty, postingIdReturned);
      Assert.AreEqual(postingIdReturned, addedPosting.Id);
      this.AssertPosting(posting, addedPosting);
    }

    [Test]
    [Order(2)]
    public async Task GetsPostingById()
    {
      var storedPosting = TestDataCreator.Posting();
      var sut = this.CreateSut();
      _ = await sut.Add(storedPosting);

      var retrievedPosting = await sut.Get(storedPosting.Id);

      Assert.AreEqual(storedPosting.Id, retrievedPosting.Id);
      this.AssertPosting(storedPosting, retrievedPosting);
    }


    [Test]
    [Order(3)]
    public async Task UpdatesPosting()
    {
      var storedPosting = TestDataCreator.Posting();
      var sut = this.CreateSut();
      _ = await sut.Add(storedPosting);

      storedPosting = TestDataCreator.Posting(id: storedPosting.Id);
      await sut.Update(storedPosting);

      var retrievedPosting = await sut.Get(storedPosting.Id);

      this.AssertPosting(storedPosting, retrievedPosting);
    }

    [Test]
    [Order(4)]
    public async Task DeletesPosting()
    {
      var storedPosting = TestDataCreator.Posting();
      var sut = this.CreateSut();
      _ = await sut.Add(storedPosting);

      await sut.Delete(storedPosting.Id);

      var retrievedPosting = await sut.Get(storedPosting.Id);

      Assert.IsNull(retrievedPosting);
    }

    [Test]
    public async Task SearchDoesNotThrowWhenNoFilters()
    {
      var sut = this.CreateSut();
      await sut.Add(TestDataCreator.Posting(title: "123 abcd 321"));

      IEnumerable<Posting> matchingResults = null;
      Assert.DoesNotThrowAsync(async () => matchingResults = await sut.Search(null, null, false, null, null));
      Assert.IsNotEmpty(matchingResults);
    }

    [TestCase("abcde1", 0, Description = "Single word, no matches")]
    [TestCase("abcde2", 1, Description = "Single word, single match")]
    [TestCase("abcde3", 5, Description = "Single word, multiple matches")]
    [TestCase("abcde4 xyz1", 0, Description = "Multiple words, no matches")]
    [TestCase("abcde5 xyz2", 1, Description = "Multiple words, single match")]
    [TestCase("abcde6 xyz3 jklmno1", 5, Description = "Multiple words, multiple matches")]
    public async Task FiltersByTitle(string keywords, int matchesCount)
    {
      var sut = this.CreateSut();
      await sut.Add(TestDataCreator.Posting(title: "123 abcd 321"));
      await sut.Add(TestDataCreator.Posting(title: "non-matching"));

      var matchingPostings = new List<Guid>(matchesCount);
      for (int i = 0; i < matchesCount; i++)
      {
        matchingPostings.Add(await sut.Add(TestDataCreator.Posting(title: $"123 {keywords} 321")));
      }

      var results = (await sut.Search(keywords, null, false, null, null)).ToList();

      Assert.AreEqual(matchesCount, results.Count);
      CollectionAssert.AreEquivalent(matchingPostings, results.Select(p => p.Id));
    }

    [Test]
    public async Task FiltersByTitleWhenSingleKeywordOfManyMatch()
    {
      var sut = this.CreateSut();
      await sut.Add(TestDataCreator.Posting(title: "123 abcd 321"));
      await sut.Add(TestDataCreator.Posting(title: "non-matching"));

      var matchingKeyword = Guid.NewGuid().ToString();
      var searchKeywords = $"{Guid.NewGuid()} {matchingKeyword} {Guid.NewGuid()} {Guid.NewGuid()}";
      var matchingPosting = await sut.Add(TestDataCreator.Posting(title: $"123 {matchingKeyword} 321"));

      var results = (await sut.Search(searchKeywords, null, false, null, null)).ToList();

      Assert.AreEqual(1, results.Count);
      Assert.AreEqual(matchingPosting, results[0].Id);
    }

    [TestCase("abcde1", 0, Description = "Single word, no matches")]
    [TestCase("abcde2", 1, Description = "Single word, single match")]
    [TestCase("abcde3", 5, Description = "Single word, multiple matches")]
    [TestCase("abcde4 xyz1", 0, Description = "Multiple words, no matches")]
    [TestCase("abcde5 xyz2", 1, Description = "Multiple words, single match")]
    [TestCase("abcde6 xyz3 jklmno1", 5, Description = "Multiple words, multiple matches")]
    public async Task FiltersByTags(string tags, int matchesCount)
    {
      var sut = this.CreateSut();
      await sut.Add(TestDataCreator.Posting(tags: "123 abcd 321".Split(' ')));
      await sut.Add(TestDataCreator.Posting(tags: "non-matching".Split(' ')));

      var matchingPostings = new List<Guid>(matchesCount);
      for (int i = 0; i < matchesCount; i++)
      {
        matchingPostings.Add(await sut.Add(TestDataCreator.Posting(tags: $"123 {tags} 321".Split(' '))));
      }

      var results = (await sut.Search(null, null, false, null, tags.Split(' '))).ToList();

      Assert.AreEqual(matchesCount, results.Count);
      CollectionAssert.AreEquivalent(matchingPostings, results.Select(p => p.Id));
    }

    [Test]
    public async Task FiltersByTagWhenSingleKeywordOfManyMatch()
    {
      var sut = this.CreateSut();
      await sut.Add(TestDataCreator.Posting(tags: "123 abcd 321"));
      await sut.Add(TestDataCreator.Posting(tags: "non-matching"));

      var matchingTag = Guid.NewGuid().ToString();
      var searchTags = $"{Guid.NewGuid()} {Guid.NewGuid()} {Guid.NewGuid()} {matchingTag}".Split(' ');
      var matchingPosting = await sut.Add(TestDataCreator.Posting(tags: $"123 {matchingTag} 321".Split(' ')));

      var results = (await sut.Search(null, null, false, null, searchTags)).ToList();

      Assert.AreEqual(1, results.Count);
      Assert.AreEqual(matchingPosting, results[0].Id);
    }

    [TestCase("location001", 0, Description = "No matches")]
    [TestCase("location002", 1, Description = "Single match")]
    [TestCase("location003", 3, Description = "Multiple matches")]
    public async Task FiltersByLocation(string location, int matchesCount)
    {
      var sut = this.CreateSut();
      await sut.Add(TestDataCreator.Posting(location: "Mordor"));
      await sut.Add(TestDataCreator.Posting(location: "Narnia"));

      var matchingPostings = new List<Guid>(matchesCount);
      for (int i = 0; i < matchesCount; i++)
      {
        matchingPostings.Add(await sut.Add(TestDataCreator.Posting(location: location)));
      }

      var results = (await sut.Search(null, location, false, null, null)).ToList();

      Assert.AreEqual(matchesCount, results.Count);
      CollectionAssert.AreEquivalent(matchingPostings, results.Select(p => p.Id));
    }

    [Test]
    public async Task FiltersByRemoteAvailable()
    {
      var sut = this.CreateSut();
      var matchingPostingId = await sut.Add(TestDataCreator.Posting(remoteAvailable: true));
      var nonMatchingPostingId = await sut.Add(TestDataCreator.Posting(remoteAvailable: false));

      var results = (await sut.Search(null, null, true, null, null)).ToList();

      Assert.IsNotEmpty(results);
      Assert.IsTrue(results.Any(p => p.Id == matchingPostingId));
      Assert.IsFalse(results.Any(p => p.Id == nonMatchingPostingId));
    }

    [Test]
    public async Task FiltersBySalary()
    {
      var sut = this.CreateSut();
      var matchingPostingId = await sut.Add(TestDataCreator.Posting(salaryMax: 50000));
      var nonMatchingPostingId = await sut.Add(TestDataCreator.Posting(salaryMax: 20000));

      var results = (await sut.Search(null, null, false, 30000, null)).ToList();

      Assert.IsNotEmpty(results);
      Assert.IsTrue(results.Any(p => p.Id == matchingPostingId));
      Assert.IsFalse(results.Any(p => p.Id == nonMatchingPostingId));
    }

    private void AssertPosting(Posting expected, Posting actual)
    {
      Assert.AreEqual(expected.CompanyId, actual.CompanyId);
      Assert.AreEqual(expected.Title, actual.Title);
      Assert.AreEqual(expected.Description, actual.Description);
      Assert.AreEqual(expected.Location, actual.Location);
      Assert.AreEqual(expected.SalaryRangeMin, actual.SalaryRangeMin);
      Assert.AreEqual(expected.SalaryRangeMax, actual.SalaryRangeMax);
      CollectionAssert.AreEquivalent(expected.Tags, actual.Tags);
    }

    private PostingRepository CreateSut()
    {
      return new PostingRepository();
    }
  }
}
