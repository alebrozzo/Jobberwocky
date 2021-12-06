using Microsoft.AspNetCore.Mvc;
using Jobberwocky.Api.Services.OperationHandling;

namespace Jobberwocky.Api.Controllers
{
  public class JobberController : Controller
  {
    public IActionResult ServiceResultToHttp<T>(OperationResult<T> operationResult)
    {
      switch (operationResult.Status)
      {
        case OperationStatus.Success:
        case OperationStatus.NotModified:
          return this.Ok(operationResult.Result);
        case OperationStatus.Forbidden:
          return this.Forbid();
        case OperationStatus.NotFound:
          return this.NotFound();
        case OperationStatus.ValidationError:
          return this.BadRequest(operationResult.Errors);
        case OperationStatus.Conflict:
          return this.Conflict(operationResult.Errors);
        default:
          throw new System.Exception("Error when executing request");
      }
    }
  }
}
