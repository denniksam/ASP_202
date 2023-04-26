using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;

namespace ASP_202.Services.Validation
{
    public class ValidationServiceV1 : IValidationService
    {
        public bool Validate(string source, params ValidationTerms[] terms)
        {
            if (terms.Length == 0)
            {
                throw new ArgumentException("No terms for validator");
            }
            if(terms.Length == 1 && terms[0] == ValidationTerms.None)
            {
                return true;
            }
            bool result = true;
            if(terms.Contains(ValidationTerms.NotEmpty))
            {
                result &= !String.IsNullOrEmpty(source);
                // result = result && x
            }
            if (terms.Contains(ValidationTerms.Email))
            {
                result &= ValidateEmail(source);
            }
            if (terms.Contains(ValidationTerms.Login))
            {
                result &= ValidateLogin(source);
            }
            if (terms.Contains(ValidationTerms.RealName))
            {
                result &= ValidateRealName(source);
            }
            if (terms.Contains(ValidationTerms.Passsword))
            {
                result &= ValidatePassword(source);
            }
            return result;
        }
        private static bool ValidatePassword(String source)
        {
            return source.Length >= 3;
        }
        private static bool ValidateRegex(String source, String pattern)
        {
            return Regex.IsMatch(source, pattern);
        }

        private static bool ValidateEmail(String source)
        {
            return ValidateRegex(source,
                @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$");            
        }

        private static bool ValidateLogin(String source)
        {
            return ValidateRegex(source,
                @"^\w{3,}$");
        }
        private static bool ValidateRealName(String source)
        {
            return ValidateRegex(source,
                @"^[a-zA-Zа-яА-Я]+([-' ][a-zA-Zа-яА-Я]+)*$");
        }
    }
}
