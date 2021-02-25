#region

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Sobees.Library.BGenericLib;

#endregion

namespace Sobees.Controls.Facebook.Converters
{
  public class BtnAllCommentsMultiVisibilityConverter : IMultiValueConverter
  {
    #region IMultiValueConverter Members

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == null) return Visibility.Collapsed;
      if (values.Length < 2) return Visibility.Collapsed;

      var comments = values[0] as ObservableCollection<Comment>;
      var nbComments = 0;
      if (values[1] == null) return Visibility.Collapsed;
      int.TryParse(values[1].ToString(),
        out nbComments);
      if (comments != null)
      {
        if (nbComments > 3 & comments.Count != nbComments)
        {
          return Visibility.Visible;
        }
      }
      return Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}