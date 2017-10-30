using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PoC.WpfWarning
{
    public class WarnedValidationResult: ValidationResult
    {
        private bool _isWarnded;

        public WarnedValidationResult(bool isError, bool isWarned, object errorContent) : base(isError, errorContent)
        {
            IsWarned = isWarned;
        }

        public bool IsWarned
        {
            get { return _isWarnded; }
            set {  _isWarnded= value; }
        }
    }
}
