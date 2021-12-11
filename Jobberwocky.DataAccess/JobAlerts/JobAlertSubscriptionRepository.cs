using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jobberwocky.Domain;

namespace Jobberwocky.DataAccess
{
  public class JobAlertSubscriptionRepository : IJobAlertSubscriptionRepository
  {
    // For the sake of brevity, I am using an In Memory data repository
    // await Task.Yield(); fakes the asyncrony expected from a database or external api call
    private static readonly List<JobAlertSubscription> jobAlertSubscriptions = new List<JobAlertSubscription>
    {
      new JobAlertSubscription { Id = Guid.Parse("eeeeeeee-bbbb-cccc-dddd-000000000000"), Email = "john.doe@myspace.com", Keywords = "java", LastEmailSentDate = DateTime.UtcNow },
      new JobAlertSubscription { Id = Guid.Parse("eeeeeeee-bbbb-cccc-dddd-111111111111"), Email = "jane.doe@myspace.com", Keywords = "javascript", LastEmailSentDate = DateTime.UtcNow.AddDays(-1) },
      new JobAlertSubscription { Id = Guid.Parse("eeeeeeee-bbbb-cccc-dddd-222222222222"), Email = "peter@abc.com.es", Keywords = "java", LastEmailSentDate = DateTime.UtcNow },
      new JobAlertSubscription { Id = Guid.Parse("eeeeeeee-bbbb-cccc-dddd-333333333333"), Email = "alice@chat.co.nz", Keywords = "react", LastEmailSentDate = DateTime.UtcNow },
    };

    public async Task<IEnumerable<JobAlertSubscription>> GetAll()
    {
      await Task.Yield();
      return jobAlertSubscriptions;
    }
  }
}
