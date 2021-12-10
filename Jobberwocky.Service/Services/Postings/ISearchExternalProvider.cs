using System.Collections.Generic;
using System.Threading.Tasks;
using Jobberwocky.Api.Dtos;
using Jobberwocky.Domain;

namespace Jobberwocky.Api.Services
{
  public interface ISearchExternalProvider
  {
    Task<IEnumerable<Posting>> SearchJobPostings(PostingSearchDto postingSearchDto);
  }
}
