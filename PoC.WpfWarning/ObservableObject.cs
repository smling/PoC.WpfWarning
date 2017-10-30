using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace PoC.WpfWarning
{
    public class ObservableObject : INotifyPropertyChanged, INotifyDataErrorInfo, IDataWarningInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #region Implementation of Interfaces.
        protected Dictionary<string, ICollection<string>> _validationErrors = new Dictionary<string, ICollection<string>>();
        protected Dictionary<string, ICollection<string>> _validationWarnings = new Dictionary<string, ICollection<string>>();

        public bool HasErrors
        {
            get { return _errors; }
            set { SetField(ref _errors, value); }
        }

        private bool _errors;

        public bool Errors
        {
            get { return _errors; }
            set { SetField(ref _errors, value); }
        }
        private bool _hasWarnings;
        public bool HasWarnings
        {
            get { return _hasWarnings; }
            set { SetField(ref _hasWarnings, value); }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event EventHandler<DataErrorsChangedEventArgs> WarningsChanged;

        public virtual void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
        public virtual void OnWarningsChanged(string propertyName)
        {
            if (WarningsChanged != null)
                WarningsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
        protected void UpdateValidationErrors(string propertyName, ICollection<string> errorMessage)
        {
            if (errorMessage.Count > 0)
            {
                /* Update the collection in the dictionary returned by the GetErrors method */
                _validationErrors[propertyName] = errorMessage;
                Errors = true;
            }
            else if (_validationErrors.ContainsKey(propertyName))
            {
                /* Remove all errors for this property */
                _validationErrors.Remove(propertyName);
                Errors = false;
            }
            /* Raise event to tell WPF to execute the GetErrors method */
            OnErrorsChanged(propertyName);
            OnPropertyChanged(propertyName);
        }

        public IEnumerable GetWarnings(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_validationWarnings.ContainsKey(propertyName))
                return null;
            return _validationWarnings[propertyName];
        }
        protected void UpdateValidationWarnings(string propertyName, ICollection<string> warningMessage)
        {
            if (warningMessage.Count > 0)
            {
                /* Update the collection in the dictionary returned by the GetErrors method */
                _validationWarnings[propertyName] = warningMessage;
                HasWarnings = true;
            }
            else if (_validationWarnings.ContainsKey(propertyName))
            {
                /* Remove all errors for this property */
                _validationWarnings.Remove(propertyName);
                HasWarnings = false;
            }
            /* Raise event to tell WPF to execute the GetErrors method */
            OnWarningsChanged(propertyName);
            OnPropertyChanged(propertyName);
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_validationErrors.ContainsKey(propertyName))
                return null;
            return _validationErrors[propertyName];
        }
        #endregion

        public bool ValidateProperty(ValidationRule validator, [CallerMemberName] string propertyName = null)
        {
            /* Call service asynchronously */
            bool result = false;
            ValidationResult validationResult = validator.Validate(this, null);
            if (validationResult != null)
            {
                ICollection<string> validationErrors = new List<string>();
                ICollection<string> warningMessages = new List<string>();
                if (validationResult.IsValid == false)
                {
                    validationErrors.Add(validationResult.ErrorContent.ToString());
                    result = validationResult.IsValid;
                }
                else if (validationResult is WarnedValidationResult)
                {
                    WarnedValidationResult warnedValidationResult = validationResult as WarnedValidationResult;
                    if (warnedValidationResult.IsWarned)
                    {       
                        warningMessages.Add(warnedValidationResult.ErrorContent.ToString());
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
                UpdateValidationErrors(propertyName, validationErrors);
                UpdateValidationWarnings(propertyName, warningMessages);
            }
            return result;
        }
    }

    public class WarningErrorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool warning = (bool) values[0];
            bool error = (bool) values[1];
            return ((!warning) && (!error));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
