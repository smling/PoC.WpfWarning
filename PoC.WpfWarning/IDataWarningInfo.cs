using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.WpfWarning
{
    public interface IDataWarningInfo
    {
        event EventHandler<DataErrorsChangedEventArgs> WarningsChanged;

        System.Collections.IEnumerable GetWarnings(string propertyName);

        bool HasWarnings { get; }
    }
}
