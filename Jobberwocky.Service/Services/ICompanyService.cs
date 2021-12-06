using System;
using System.Threading.Tasks;
using Jobberwocky.Api.Services.OperationHandling;
using Jobberwocky.Domain;

namespace Jobberwocky.Api.Services
{
  public interface ICompanyService
  {
    /// <summary>
    /// Gets a company by ID.
    /// </summary>
    /// <param name="id">The ID of the company to retrieve.</param>
    /// <returns></returns>
    Task<OperationResult<Company>> Get(Guid id);

    /// <summary>
    /// Adds a company to the repository.
    /// </summary>
    /// <param name="company">The data to store.</param>
    /// <returns>The new company as retrieved from the repository.</returns>
    Task<OperationResult<Company>> Add(Company company);

    /// <summary>
    /// Updates an existing company. Searches by ID.
    /// </summary>
    /// <param name="company">The data to store.</param>
    /// <returns>The new company as retrieved from the repository.</returns>
    Task<OperationResult<Company>> Update(Company company);

    /// <summary>
    /// Deletes a company from the repository.
    /// </summary>
    /// <param name="id">The ID of the company to delete.</param>
    /// <returns></returns>
    Task<OperationResult<bool>> Delete(Guid id);
  }
}
