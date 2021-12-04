using Jobberwocky.Domain;

namespace Jobberwocky.DataAccess
{
  public interface ICompanyRepository
  {
    public Company Add(Company company);
  }
}
