using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Jobberwocky.Api.Services;
using Jobberwocky.Domain;

namespace Jobberwocky.Api.Controllers
{
  [Route("api/companies")]
  [ApiController]
  public class CompanyController : JobberController
  {
    private readonly ICompanyService companyService;

    public CompanyController(ICompanyService companyService)
    {
      this.companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
      var result = await this.companyService.Get(id);
      return this.ServiceResultToHttp(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Company company)
    {
      var result = await this.companyService.Add(company);
      return this.ServiceResultToHttp(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] Company company)
    {
      if (id != company.Id)
      {
        return this.BadRequest("Id from url must match id in body.");
      }

      var result = await this.companyService.Update(company);
      return this.ServiceResultToHttp(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      var result = await this.companyService.Delete(id);
      return this.ServiceResultToHttp(result);
    }
  }
}
