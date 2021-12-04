namespace Jobberwocky.Api.Extensions
{
  using Jobberwocky.Api.Services;
  using Microsoft.Extensions.DependencyInjection;

  public static class MyConfigServiceCollectionExtensions
  {
    public static IServiceCollection AddDependencyInjectionConfig(
      this IServiceCollection services)
    {
      services.AddScoped<ICompanyService, CompanyService>();

      return services;
    }
  }
}
