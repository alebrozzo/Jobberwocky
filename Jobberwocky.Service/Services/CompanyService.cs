using System;
using System.Threading.Tasks;
using Jobberwocky.Api.Services.OperationHandling;
using Jobberwocky.DataAccess;
using Jobberwocky.Domain;

namespace Jobberwocky.Api.Services
{
  public class CompanyService : ICompanyService
  {
    private readonly ICompanyRepository companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
      this.companyRepository = companyRepository ?? throw new System.ArgumentNullException(nameof(companyRepository));
    }

    public async Task<OperationResult<Company>> Get(Guid id)
    {
      if (id == Guid.Empty)
      {
        return OperationResult<Company>.Error(OperationStatus.ValidationError, "Please provide an ID to search by.");
      }

      var company = await this.companyRepository.Get(id);
      if (company == null)
      {
        return OperationResult<Company>.Error(OperationStatus.NotFound);
      }

      return OperationResult<Company>.Ok(company);
    }

    public async Task<OperationResult<Company>> Add(Company company)
    {
      var validationResult = this.ValidateCompany(company);
      if (validationResult != null)
      {
        return validationResult;
      }

      if (company.Id != Guid.Empty)
      {
        var existingCompany = await this.companyRepository.Get(company.Id);
        if (existingCompany != null)
        {
          return OperationResult<Company>.Error(OperationStatus.ValidationError, "A company with the provided ID already exists.");
        }
      }

      var companyId = await this.companyRepository.Add(company);
      var retrievedCompany = await this.companyRepository.Get(companyId);

      return OperationResult<Company>.Ok(retrievedCompany);
    }

    public async Task<OperationResult<Company>> Update(Company company)
    {
      var validationResult = this.ValidateCompany(company);
      if (validationResult != null)
      {
        return validationResult;
      }

      if (company.Id == Guid.Empty)
      {
        return OperationResult<Company>.Error(OperationStatus.ValidationError, "Please provide the ID of the company to update.");
      }

      var existingCompany = await this.companyRepository.Get(company.Id);
      if (existingCompany == null)
      {
        return OperationResult<Company>.Error(OperationStatus.NotFound);
      }

      await this.companyRepository.Update(company);

      return OperationResult<Company>.Ok(company);
    }

    public async Task<OperationResult<bool>> Delete(Guid id)
    {
      if (id == Guid.Empty)
      {
        return OperationResult<bool>.Error(OperationStatus.ValidationError, "Please provide the ID of the company to delete.");
      }

      var retrievedCompany = await this.companyRepository.Get(id);
      if (retrievedCompany == null)
      {
        return OperationResult<bool>.Error(OperationStatus.NotFound);
      }

      await this.companyRepository.Delete(id);
      return OperationResult<bool>.Ok(true);
    }

    private OperationResult<Company> ValidateCompany(Company company)
    {
      if (company == null)
      {
        return OperationResult<Company>.Error(OperationStatus.ValidationError, "Company data is missing.");
      }

      if (string.IsNullOrWhiteSpace(company.Name))
      {
        return OperationResult<Company>.Error(OperationStatus.ValidationError, "Company name is missing.");
      }

      if (company.Name.Length < 5 || company.Name.Length > 20)
      {
        return OperationResult<Company>.Error(OperationStatus.ValidationError, "Company name must be between 5 and 20 characters long.");
      }

      if (string.IsNullOrWhiteSpace(company.Description))
      {
        return OperationResult<Company>.Error(OperationStatus.ValidationError, "Company description must not be empty.");
      }

      if (company.Description.Length > 500)
      {
        return OperationResult<Company>.Error(OperationStatus.ValidationError, "Company description must be up to 500 characters long.");
      }

      return null;
    }
  }
}
