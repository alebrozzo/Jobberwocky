using System;
using System.Threading.Tasks;
using Jobberwocky.Domain;

namespace Jobberwocky.DataAccess
{
  public interface IPostingRepository
  {
    /// <summary>
    /// Gets a posting by ID.
    /// </summary>
    /// <param name="id">The ID of the posting to retrieve.</param>
    /// <returns></returns>
    Task<Posting> Get(Guid id);

    /// <summary>
    /// Adds a posting to the repository.
    /// </summary>
    /// <param name="posting">The data to store.</param>
    /// <returns>The id of the new posting. Can be passed in parameter or automatically generated.</returns>
    Task<Guid> Add(Posting posting);

    /// <summary>
    /// Updates an existing posting. Searches by ID.
    /// </summary>
    /// <param name="posting">The new data for the posting. Assumes ID exists.</param>
    /// <returns></returns>
    Task Update(Posting posting);

    /// <summary>
    /// Deletes a posting from the repository.
    /// </summary>
    /// <param name="id">The ID of the posting to delete. Assumes ID exists.</param>
    /// <returns></returns>
    Task Delete(Guid id);
  }
}
