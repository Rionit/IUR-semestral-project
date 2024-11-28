using IUR_Semestral_Work.Support;
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
    public class PasswordValidationRule : ValidationRule
    {
        //public ValidationRuleProxy Proxy { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string password = value as string;

            if (password == null || password.Length < 8)
            {
                return new ValidationResult(false, "Password has to be more than 8 characters long");
            }/*
            else if (!Proxy.IsRegistering && !ProfileManager.UserExists(Proxy.Username, password))
            {
                return new ValidationResult(false, "Wrong password or username!");
            }*/

            return ValidationResult.ValidResult;
        }
    }

}
