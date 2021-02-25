#region

using System;
using System.Globalization;
using System.Windows.Data;
using Sobees.Library.BGenericLib;

#endregion

namespace Sobees.Controls.Facebook.Converters
{
  public class LikeVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return "Collapsed";
      if (!value.GetType().Equals(typeof (Like))) return "Collapsed";
      var like = value as Like;
      if (like == null) return "Collapsed";
      if (like.Count == 0)
      {
        return "Collapsed";
      }

      return "Visible";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}