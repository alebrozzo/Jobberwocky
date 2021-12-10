using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Jobberwocky.Api.Dtos;
using Jobberwocky.Api.Services.OperationHandling;
using Jobberwocky.DataAccess;
using Jobberwocky.Domain;

namespace Jobberwocky.Api.Services
{
  public class JobberwockyExternalProvider : ISearchExternalProvider
  {
    private readonly IHttpClientFactory httpClientFactory;

    public JobberwockyExternalProvider(IHttpClientFactory httpClientFactory)
    {
      this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<IEnumerable<Posting>> SearchJobPostings(PostingSearchDto postingSearchDto)
    {
      var httpClient = this.httpClientFactory.CreateClient();
      httpClient.Timeout = TimeSpan.FromSeconds(10);

      var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:8080/jobs");
      var response = await httpClient.SendAsync(request);
      if (!response.IsSuccessStatusCode)
      {
        // TODO: Log an error
        return new Posting[0];
      }

      var results = await response.Content.ReadAsStringAsync();
      return this.Map(results);
    }

    private IEnumerable<Posting> Map(string jobberwockyResults)
    {
      throw new NotImplementedException();
    }
  }
}
