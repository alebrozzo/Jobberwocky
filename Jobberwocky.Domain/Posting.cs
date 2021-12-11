using System;
using System.Collections.Generic;

namespace Jobberwocky.Domain
{
  public class Posting
  {
    public Guid Id { get; set; }

    public Guid? CompanyId { get; set; }

    public string CompanyName { get; set; }

    public string Title { get; set; }

    public string Location { get; set; }

    public bool? RemoteAvailable { get; set; }

    public string Description { get; set; }

    public decimal? SalaryRangeMin { get; set; }

    public decimal? SalaryRangeMax { get; set; }

    public DateTime DateCreated { get; set; }

    public IEnumerable<string> Tags { get; set; }
  }
}
