using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PoC.WpfWarning
{
    public class User : ObservableObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set {
                if (_name != value)
                {
                    SetField(ref _name, value);
                    ValidateProperty(new NameValidor());
                }
            }
        }
    }

    public class NameValidor : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new WarnedValidationResult(true, false, null);
            User source = value as User;
            if (source != null)
            {
                if (string.IsNullOrEmpty(source.Name))
                    result = new ValidationResult(false, "Name could not be empty.");
                else if (source.Name == "aaa")
                    result = new WarnedValidationResult(true, true, "Message cannot be " + source.Name);
            }            
            return result;
        }
    }
}
