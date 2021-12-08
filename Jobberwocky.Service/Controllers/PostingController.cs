using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Jobberwocky.Api.Services;
using Jobberwocky.Domain;

namespace Jobberwocky.Api.Controllers
{
  [Route("api/postings")]
  [ApiController]
  public class PostingController : JobberController
  {
    private readonly IPostingService postingService;

    public PostingController(IPostingService postingService)
    {
      this.postingService = postingService ?? throw new ArgumentNullException(nameof(postingService));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
      var result = await this.postingService.Get(id);
      return this.ServiceResultToHttp(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Posting posting)
    {
      var result = await this.postingService.Add(posting);
      return this.ServiceResultToHttp(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] Posting posting)
    {
      if (id != posting.Id)
      {
        return this.BadRequest("Id from url must match id in body.");
      }

      var result = await this.postingService.Update(posting);
      return this.ServiceResultToHttp(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      var result = await this.postingService.Delete(id);
      return this.ServiceResultToHttp(result);
    }
  }
}
