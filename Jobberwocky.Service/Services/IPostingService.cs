﻿using System;
using System.Threading.Tasks;
using Jobberwocky.Api.Services.OperationHandling;
using Jobberwocky.Domain;

namespace Jobberwocky.Api.Services
{
  public interface IPostingService
  {
    /// <summary>
    /// Gets a posting by ID.
    /// </summary>
    /// <param name="id">The ID of the posting to retrieve.</param>
    /// <returns></returns>
    Task<OperationResult<Posting>> Get(Guid id);

    /// <summary>
    /// Adds a posting to the repository.
    /// </summary>
    /// <param name="posting">The data to store.</param>
    /// <returns>The new posting as retrieved from the repository.</returns>
    Task<OperationResult<Posting>> Add(Posting posting);

    /// <summary>
    /// Updates an existing posting. Searches by ID.
    /// </summary>
    /// <param name="posting">The data to store.</param>
    /// <returns>The new posting as retrieved from the repository.</returns>
    Task<OperationResult<Posting>> Update(Posting posting);

    /// <summary>
    /// Deletes a posting from the repository.
    /// </summary>
    /// <param name="id">The ID of the posting to delete.</param>
    /// <returns></returns>
    Task<OperationResult<bool>> Delete(Guid id);
  }
}
