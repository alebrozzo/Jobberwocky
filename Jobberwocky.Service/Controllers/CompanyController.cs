using Jobberwocky.Api.Services;
using Jobberwocky.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Jobberwocky.Api.Controllers
{
  [Route("api/companies")]
  [ApiController]
  public class CompanyController : ControllerBase
  {
    private readonly ICompanyService companyService;

    public CompanyController(ICompanyService companyService)
    {
      this.companyService = companyService ?? throw new System.ArgumentNullException(nameof(companyService));
    }

    [HttpGet]
    public List<string> Get()
    {
      return new List<string>() { "Company 1", "Company 2" };
    }

    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    [HttpPost]
    public Company Post([FromBody] Company company)
    {
      return this.companyService.Add(company);
    }

    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
