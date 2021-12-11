namespace Jobberwocky.Api.Framework
{
  using Microsoft.Extensions.DependencyInjection;
  using Jobberwocky.Api.Services;
  using Jobberwocky.DataAccess;

  public static class MyConfigServiceCollectionExtensions
  {
    public static IServiceCollection AddDependencyInjectionConfig(
      this IServiceCollection services)
    {
      services.AddScoped<ICompanyRepository, CompanyRepository>();
      services.AddScoped<IPostingRepository, PostingRepository>();
      services.AddScoped<IJobAlertSubscriptionRepository, JobAlertSubscriptionRepository>();
      services.AddScoped<ICompanyService, CompanyService>();
      services.AddScoped<IPostingService, PostingService>();
      services.AddScoped<ISearchExternalProvider, JobberwockyExternalProvider>();

      return services;
    }
  }
}
