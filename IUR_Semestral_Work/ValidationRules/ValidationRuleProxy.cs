using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IUR_Semestral_Work.ValidationRules
{
    public class ValidationRuleProxy : DependencyObject
    {
        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(ValidationRuleProxy));

        public static readonly DependencyProperty IsRegisteringProperty =
            DependencyProperty.Register("IsRegistering", typeof(bool), typeof(ValidationRuleProxy));

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public bool IsRegistering
        {
            get { return (bool)GetValue(IsRegisteringProperty); }
            set { SetValue(IsRegisteringProperty, value); }
        }
    }

}
