using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jobberwocky.Domain;

namespace Jobberwocky.DataAccess
{
  public class PostingRepository : IPostingRepository
  {
    // For the sake of brevity, I am using an In Memory data repository
    // await Task.Yield(); fakes the asyncrony expected from a database or external api call
    private static readonly List<Posting> postings = new List<Posting>();

    public async Task<Posting> Get(Guid id)
    {
      await Task.Yield();
      return postings.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Guid> Add(Posting posting)
    {
      await Task.Yield();
      if (posting.Id == Guid.Empty)
      {
        posting.Id = Guid.NewGuid();
      }

      postings.Add(posting);

      return posting.Id;
    }

    public async Task Update(Posting posting)
    {
      await Task.Yield();
      var ix = postings.FindIndex(c => c.Id == posting.Id);
      postings[ix] = posting;
    }

    public async Task Delete(Guid id)
    {
      await Task.Yield();
      var ix = postings.FindIndex(c => c.Id == id);
      postings.RemoveAt(ix);
    }
  }
}
