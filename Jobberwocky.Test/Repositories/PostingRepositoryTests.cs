using System;
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
      _ = sut.Add(storedPosting);

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
      _ = sut.Add(storedPosting);

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
      _ = sut.Add(storedPosting);

      await sut.Delete(storedPosting.Id);

      var retrievedPosting = await sut.Get(storedPosting.Id);

      Assert.IsNull(retrievedPosting);
    }

    private void AssertPosting(Posting expected, Posting actual)
    {
      Assert.AreEqual(expected.CompanyId, actual.CompanyId);
      Assert.AreEqual(expected.Description, actual.Description);
      Assert.AreEqual(expected.DateCreated, actual.DateCreated);
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
