using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Jobberwocky.Api.Controllers
{
  [Route("api/companies")]
  [ApiController]
  public class CompanyController : ControllerBase
  {
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
    public void Post([FromBody] string value)
    {
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
