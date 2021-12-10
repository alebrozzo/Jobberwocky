using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jobberwocky.Api.Dtos;
using Jobberwocky.Api.Services.OperationHandling;
using Jobberwocky.DataAccess;
using Jobberwocky.Domain;

namespace Jobberwocky.Api.Services
{
  public class PostingService : IPostingService
  {
    private readonly IPostingRepository postingRepository;
    private readonly ICompanyRepository companyRepository;
    private readonly IEnumerable<ISearchExternalProvider> externalProviders;

    public PostingService(
      IPostingRepository postingRepository, 
      ICompanyRepository companyRepository,
      IEnumerable<ISearchExternalProvider> externalProviders)
    {
      this.postingRepository = postingRepository ?? throw new ArgumentNullException(nameof(postingRepository));
      this.companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
      this.externalProviders = externalProviders ?? throw new ArgumentNullException(nameof(externalProviders));
    }

    public async Task<OperationResult<Posting>> Get(Guid id)
    {
      if (id == Guid.Empty)
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Please provide an ID to search by.");
      }

      var posting = await this.postingRepository.Get(id);
      if (posting == null)
      {
        return OperationResult<Posting>.Error(OperationStatus.NotFound);
      }

      var company = await this.companyRepository.Get(posting.CompanyId.Value);
      posting.CompanyName = company.Name;
      return OperationResult<Posting>.Ok(posting);
    }

    public async Task<OperationResult<Posting>> Add(Posting posting)
    {
      var validationResult = await this.ValidatePosting(posting);
      if (validationResult != null)
      {
        return validationResult;
      }

      if (posting.Id != Guid.Empty)
      {
        var existingPosting = await this.postingRepository.Get(posting.Id);
        if (existingPosting != null)
        {
          return OperationResult<Posting>.Error(OperationStatus.ValidationError, "A posting with the provided ID already exists.");
        }
      }

      var postingId = await this.postingRepository.Add(posting);
      return await this.Get(postingId);
    }

    public async Task<OperationResult<Posting>> Update(Posting posting)
    {
      var validationResult = await this.ValidatePosting(posting);
      if (validationResult != null)
      {
        return validationResult;
      }

      if (posting.Id == Guid.Empty)
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Please provide the ID of the posting to update.");
      }

      var existingPosting = await this.postingRepository.Get(posting.Id);
      if (existingPosting == null)
      {
        return OperationResult<Posting>.Error(OperationStatus.NotFound);
      }

      await this.postingRepository.Update(posting);

      return OperationResult<Posting>.Ok(posting);
    }

    public async Task<OperationResult<bool>> Delete(Guid id)
    {
      if (id == Guid.Empty)
      {
        return OperationResult<bool>.Error(OperationStatus.ValidationError, "Please provide the ID of the posting to delete.");
      }

      var retrievedPosting = await this.postingRepository.Get(id);
      if (retrievedPosting == null)
      {
        return OperationResult<bool>.Error(OperationStatus.NotFound);
      }

      await this.postingRepository.Delete(id);
      return OperationResult<bool>.Ok(true);
    }

    public async Task<OperationResult<IEnumerable<Posting>>> Search(PostingSearchDto postingSearchDto)
    {
      if (postingSearchDto == null)
      {
        return OperationResult<IEnumerable<Posting>>.Error(OperationStatus.ValidationError, "No search criteria provided.");
      }

      if (string.IsNullOrWhiteSpace(postingSearchDto.Keywords))
      {
        return OperationResult<IEnumerable<Posting>>.Error(OperationStatus.ValidationError, "Please provide at least one search keyword.");
      }

      var searchResult = await this.postingRepository.Search(
        postingSearchDto.Keywords,
        postingSearchDto.Location,
        postingSearchDto.RemoteAllowed,
        postingSearchDto.SalaryMin,
        postingSearchDto.Tags
      );

      // TODO: this definitely has room for improvement
      foreach (var posting in searchResult)
      {
        posting.CompanyName = (await this.companyRepository.Get(posting.CompanyId.Value)).Name;
      }

      // Search from all other registered external providers
      var externalProviderTasks = new List<Task<IEnumerable<Posting>>>();
      foreach (var externalProvider in this.externalProviders)
      {
        externalProviderTasks.Add(externalProvider.SearchJobPostings(postingSearchDto));
      }

      var externalPostings = await Task.WhenAll(externalProviderTasks);
      var combinedResults = searchResult.Concat(externalPostings.SelectMany(p => p));
      combinedResults.OrderBy(p => p.DateCreated);
      return OperationResult<IEnumerable<Posting>>.Ok(combinedResults);
    }

    private async Task<OperationResult<Posting>> ValidatePosting(Posting posting)
    {
      if (posting == null)
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Posting data is missing.");
      }

      if (string.IsNullOrWhiteSpace(posting.Title))
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Posting title is missing.");
      }

      if (string.IsNullOrWhiteSpace(posting.Description))
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Posting description is missing.");
      }

      if (!posting.CompanyId.HasValue || posting.CompanyId == Guid.Empty)
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Posting company is missing.");
      }

      if (posting.Title.Length < 5 || posting.Title.Length > 100)
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Posting title must be between 5 and 100 characters long.");
      }

      if (posting.Description.Length < 5 || posting.Description.Length > 10000)
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Posting description must be between 5 and 10000 characters long.");
      }

      if (posting.SalaryRangeMin < 0)
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Posting salary range (minimum) must be greater than zero.");
      }

      if (posting.SalaryRangeMax < 0)
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Posting salary range (maximum) must be greater than zero.");
      }

      if (posting.SalaryRangeMin.HasValue && posting.SalaryRangeMax.HasValue && posting.SalaryRangeMax < posting.SalaryRangeMin)
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Posting salary range is invalid.");
      }

      var company = await this.companyRepository.Get(posting.CompanyId.Value);
      if (company is null)
      {
        return OperationResult<Posting>.Error(OperationStatus.ValidationError, "Posting company is invalid.");
      }

      return null;
    }
  }
}
