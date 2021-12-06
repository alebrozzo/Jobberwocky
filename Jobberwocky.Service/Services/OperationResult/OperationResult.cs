using System;
using System.Collections.Generic;

namespace Jobberwocky.Api.Services.OperationHandling
{
  /// <summary>
  ///   Generic Operation Result that provides a mechanism to return a result.
  /// </summary>
  /// <typeparam name="T">The result type.</typeparam>
  public sealed class OperationResult<T>
  {
    public OperationResult(OperationStatus status, T result, IReadOnlyList<string> errors)
    {
      this.Status = status;
      this.Result = result;
      this.Errors = errors;
    }

    #region Properties
    /// <summary>
    ///   Gets a value indicating whether the Status is Success.
    /// </summary>
    /// <value>
    ///   <c>true</c> if success; otherwise, <c>false</c>.
    /// </value>
    public bool Success => IsSuccessfulResult(this.Status);

    /// <summary>
    ///   Gets the operation result status.
    /// </summary>
    public OperationStatus Status { get; }

    /// <summary>
    ///   Gets the error messages.
    /// </summary>
    public IReadOnlyCollection<string> Errors { get; }

    /// <summary>
    ///   Gets a successful operation's result.
    /// </summary>
    public T Result { get; }
    #endregion

    /// <summary>
    ///   Creates an Operation Result with a Success status and the provided result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    public static OperationResult<T> Ok(T result)
    {
      return new OperationResult<T>(OperationStatus.Success, result, null);
    }

    /// <summary>
    ///   Creates an Operation Result with the provided error status and optional error messages.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <param name="errors">The errors.</param>
    /// <returns><see cref="IOperationResult{T}"/></returns>
    /// <exception cref="System.ArgumentException">status - Error call should not specify success</exception>
    public static OperationResult<T> Error(OperationStatus status, params string[] errors)
    {
      if (IsSuccessfulResult(status))
      {
        throw new ArgumentException(@"Error call should not specify success", nameof(status));
      }

      return new OperationResult<T>(status, default, errors);
    }

    private static bool IsSuccessfulResult(OperationStatus status)
    {
      return status == OperationStatus.Success || status == OperationStatus.NotModified;
    }
  }
}
