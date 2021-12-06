using System;
using System.Threading.Tasks;
using Jobberwocky.Domain;

namespace Jobberwocky.DataAccess
{
  public interface ICompanyRepository
  {
    /// <summary>
    /// Gets a company by ID.
    /// </summary>
    /// <param name="id">The ID of the company to retrieve.</param>
    /// <returns></returns>
    Task<Company> Get(Guid id);

    /// <summary>
    /// Adds a company to the repository.
    /// </summary>
    /// <param name="company">The data to store.</param>
    /// <returns>The id of the new company. Can be passed in parameter or automatically generated.</returns>
    Task<Guid> Add(Company company);

    /// <summary>
    /// Updates an existing company. Searches by ID.
    /// </summary>
    /// <param name="company">The new data for the company. Assumes ID exists.</param>
    /// <returns></returns>
    Task Update(Company company);

    /// <summary>
    /// Deletes a company from the repository.
    /// </summary>
    /// <param name="id">The ID of the company to delete. Assumes ID exists.</param>
    /// <returns></returns>
    Task Delete(Guid id);
  }
}
