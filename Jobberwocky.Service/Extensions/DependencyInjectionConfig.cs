namespace Jobberwocky.Api.Extensions
{
  using Jobberwocky.Api.Services;
  using Jobberwocky.DataAccess;
  using Microsoft.Extensions.DependencyInjection;

  public static class MyConfigServiceCollectionExtensions
  {
    public static IServiceCollection AddDependencyInjectionConfig(
      this IServiceCollection services)
    {
      services.AddScoped<ICompanyRepository, CompanyRepository>();
      services.AddScoped<IPostingRepository, PostingRepository>();
      services.AddScoped<ICompanyService, CompanyService>();
      services.AddScoped<IPostingService, PostingService>();

      return services;
    }
  }
}
