using System;

namespace Jobberwocky.Domain
{
  public class JobAlertSubscription
  {
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string Keywords { get; set; }

    public DateTime LastEmailSentDate { get; set; }
  }
}
