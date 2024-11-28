using IUR_Semestral_Work.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace IUR_Semestral_Work.ValidationRules
{
    public class UsernameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string username = value as string;

            if (string.IsNullOrWhiteSpace(username))
            {
                return new ValidationResult(false, "Username cannot be empty");
            }

            return ValidationResult.ValidResult;
        }
    }
}
