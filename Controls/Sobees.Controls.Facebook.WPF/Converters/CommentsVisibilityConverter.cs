#region

using System;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace Sobees.Controls.Facebook.Converters
{
  public class CommentsVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return "Collapsed";
      if (!value.GetType().Equals(typeof (int))) return "Collapsed";
      int nbComments;
      int.TryParse(value.ToString(),
        out nbComments);

      if (nbComments > 0)
      {
        return "Visible";
      }

      return "Collapsed";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}