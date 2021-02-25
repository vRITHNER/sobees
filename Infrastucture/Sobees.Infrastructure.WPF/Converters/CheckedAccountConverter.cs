using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Sobees.Infrastructure.Cls;
using Sobees.Tools.Logging;
#if SILVERLIGHT
using Sobees.Tools.Binding;
#endif

namespace Sobees.Infrastructure.Converters
{
  public class CheckedAccountConverter : IMultiValueConverter
  {
    #region IMultiValueConverter Members
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      lock (this)
      {
        try
        {
          var account = values[0] as UserAccount;
          var lst = values[1] as ObservableCollection<UserAccount>;
          if (lst != null)
            if (lst.Contains(account))
            {
              return true;
            }
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this, ex);
        }
        return false;
      }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      // TODO: return accounts ? ...no lo se...
      return null;
    }

    #endregion
  }
}
