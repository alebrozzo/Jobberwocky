using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Jobberwocky.Api.Dtos;
using Jobberwocky.Domain;

namespace Jobberwocky.Api.Services
{
  public class JobberwockyExternalProvider : ISearchExternalProvider
  {
    // TODO: this could be taken from a config file or repository.
    private readonly string searchBaseUrl = @"http://localhost:8080/jobs";
    private readonly IHttpClientFactory httpClientFactory;

    public JobberwockyExternalProvider(IHttpClientFactory httpClientFactory)
    {
      this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<IEnumerable<Posting>> SearchJobPostings(PostingSearchDto postingSearchDto)
    {
      string searchUrl = $"{this.searchBaseUrl}?name={postingSearchDto.Keywords}&salary_min={postingSearchDto.SalaryMin}&country={postingSearchDto.Location}";

      var httpClient = this.httpClientFactory.CreateClient();
      httpClient.Timeout = TimeSpan.FromSeconds(10);

      var response = await httpClient.GetAsync(searchUrl);
      if (!response.IsSuccessStatusCode)
      {
        // TODO: Log an error.
        return new Posting[0];
      }

      var resultsAsString = await response.Content.ReadAsStringAsync();
      var results = this.ToJobberwockyResultDtos(resultsAsString);
      var filteredResults = this.ApplyOtherFilters(postingSearchDto, results);

      return filteredResults.Select(this.Map);
    }

    /// <summary>
    /// Map the received data to a class matching the Jobberwocky result type.
    /// </summary>
    /// <param name="jobberwockyResultsAsJson">a json string with the search results that will be mapped to an enumerable of <see cref="JobberwockyResultDto"/>.</param>
    /// <returns></returns>
    private IEnumerable<JobberwockyResultDto> ToJobberwockyResultDtos(string jobberwockyResultsAsJson)
    {
      var resultList = new List<JobberwockyResultDto>();
      var resultArrays = JsonSerializer.Deserialize<dynamic[]>(jobberwockyResultsAsJson);
      foreach (var result in resultArrays)
      {
        decimal.TryParse(result[1].ToString(), out decimal salary);
        var skillsAsString = result[3].ToString();
        resultList.Add(new JobberwockyResultDto
        {
          Name = result[0].ToString(),
          Salary = salary,
          Country = result[2].ToString(),
          Skills = JsonSerializer.Deserialize<string[]>(skillsAsString),
        });
      }

      return resultList;
    }

    /// <summary>
    /// Maps a <see cref="JobberwockyResultDto"/> to a proper <see cref="Posting"/>.
    /// </summary>
    /// <param name="jobberwockyDto">The object to map from.</param>
    /// <returns></returns>
    private Posting Map(JobberwockyResultDto jobberwockyDto)
    {
      return new Posting
      {
        Title = jobberwockyDto.Name,
        SalaryRangeMin = jobberwockyDto.Salary,
        SalaryRangeMax = jobberwockyDto.Salary,
        Location = jobberwockyDto.Country,
        Tags = jobberwockyDto.Skills,
      };
    }

    /// <summary>
    /// Apply filters that our service allows but Jobberwocky does not.
    /// </summary>
    /// <param name="postingSearchDto">The filters indicated by the user.</param>
    /// <param name="postings">The list of postings to filter.</param>
    /// <returns></returns>
    private IEnumerable<JobberwockyResultDto> ApplyOtherFilters(PostingSearchDto postingSearchDto, IEnumerable<JobberwockyResultDto> postings)
    {
      var filteredResults = postings;
      if (postingSearchDto.Tags != null && postingSearchDto.Tags.Count() > 0)
      {
        filteredResults = filteredResults.Where(posting => 
          posting.Skills.Any(skill => postingSearchDto.Tags.Contains(skill, StringComparer.OrdinalIgnoreCase))
        );
      }

      return filteredResults;
    }

    private class JobberwockyResultDto
    {
      public string Name { get; set; }

      public string Country { get; set; }

      public decimal Salary { get; set; }

      public IEnumerable<string> Skills { get; set; }
    }
  }
}
