using Microsoft.AspNetCore.Mvc;

namespace ASP_202.Services.Validation
{
    public interface IValidationService
    {
        bool Validate(String source, params ValidationTerms[] terms);
    }
}
