using System.Collections.Generic;

namespace Jobberwocky.Api.Dtos
{
  public class PostingSearchDto
  {
    public string Keywords { get; set; }
 
    public string Location { get; set; }
    
    public bool RemoteAllowed { get; set; }
    
    public decimal? SalaryMin { get; set; }
    
    public IEnumerable<string> Tags { get; set; }
  }
}
