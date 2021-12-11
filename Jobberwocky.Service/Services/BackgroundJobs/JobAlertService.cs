using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Jobberwocky.DataAccess;
using Jobberwocky.Domain;

namespace Jobberwocky.Api.Services.BackgroundJobs
{
  public class JobAlertService : BackgroundService
  {
    private readonly IJobAlertSubscriptionRepository jobAlertSubscriptionRepository;
    private readonly IPostingRepository postingRepository;

    public JobAlertService(IServiceProvider serviceProvider)
    {
      this.jobAlertSubscriptionRepository = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IJobAlertSubscriptionRepository>();
      this.postingRepository = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IPostingRepository>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      // Note: This is not configured to a real email client so no actual emails will be sent.
      var client = new SmtpClient("SmtpServerHost");
      while (!stoppingToken.IsCancellationRequested)
      {
        // TODO: this could be _greatly_ improved.
        var jobAlertSubscriptions = await this.jobAlertSubscriptionRepository.GetAll();
        foreach (var subscription in jobAlertSubscriptions)
        {
          var postingsMatching = await this.postingRepository.Search(subscription.Keywords, null, false, null, new string[0]);
          var postingsToSend = postingsMatching.Where(p => p.DateCreated > subscription.LastEmailSentDate);
          if (postingsToSend.Count() > 0)
          {
            var mailMessage = this.GetPostingsEmailMessage(subscription, postingsToSend);
            // don't await.
            client.SendAsync(mailMessage, stoppingToken);
          }
        }

        // sleep for 20 seconds to demonstrate how this works.
        await Task.Delay(1000 * 20);
      }
    }

    private MailMessage GetPostingsEmailMessage(JobAlertSubscription subscription, IEnumerable<Posting> postings)
    {
      string body = "Hi there! We have some new job postings just for you!\n";
      foreach (var posting in postings)
      {
        body += $"{posting.CompanyName ?? "A company"} is looking for {posting.Title} in {posting.Location}!\n";
      }

      var mailMessage = new MailMessage
      {
        From = new MailAddress("jobalerts@jobberwocky.com.es"),
        Body = body,
        Subject = "New Job Alerts!",
      };
      mailMessage.To.Add(subscription.Email);

      return mailMessage;
    }
  }
}
