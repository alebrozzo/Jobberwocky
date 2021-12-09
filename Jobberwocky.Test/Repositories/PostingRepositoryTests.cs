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

    [TestCase("abcde1", 0, Description = "Single word, no matches")]
    [TestCase("abcde2", 1, Description = "Single word, single match")]
    [TestCase("abcde3", 5, Description = "Single word, multiple matches")]
    [TestCase("abcde4 xyz1", 0, Description = "Multiple words, no matches")]
    [TestCase("abcde5 xyz2", 1, Description = "Multiple words, single match")]
    [TestCase("abcde6 xyz3 jklmno1", 5, Description = "Multiple words, multiple matches")]
    public async Task FiltersByTitle(string keywords, int matchesCount)
    {
      var sut = this.CreateSut();
      var nonMatchingTasks = new List<Task<Guid>>(2)
      {
        sut.Add(TestDataCreator.Posting(title: "123 abcd 321")),
        sut.Add(TestDataCreator.Posting(title: "non-matching")),
      };

      var matchingTasks = new List<Task<Guid>>(matchesCount);
      for (int i = 0; i < matchesCount; i++)
      {
        matchingTasks.Add(sut.Add(TestDataCreator.Posting(title: $"123 {keywords} 321")));
      }

      Task.WaitAll(nonMatchingTasks.Concat(matchingTasks).ToArray());

      var results = (await sut.Search(keywords, null, false, null, null)).ToList();

      Assert.AreEqual(matchesCount, results.Count);
      CollectionAssert.AreEqual(matchingTasks.Select(t=> t.Result), results.Select(p => p.Id));
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
