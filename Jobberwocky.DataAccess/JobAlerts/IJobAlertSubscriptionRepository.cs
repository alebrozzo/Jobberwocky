using System.Collections.Generic;
using System.Threading.Tasks;
using Jobberwocky.Domain;

namespace Jobberwocky.DataAccess
{
  public interface IJobAlertSubscriptionRepository
  {
    /// <summary>
    /// Retrieves all job alerts subscriptions.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<JobAlertSubscription>> GetAll();
  }
}
