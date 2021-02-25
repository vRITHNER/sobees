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
  public class CommentsInfoVisibilityConverter : IMultiValueConverter
  {
    #region IMultiValueConverter Members

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == null) return Visibility.Collapsed;
      if (values.Length < 2) return Visibility.Collapsed;

      var comments = values[0] as ObservableCollection<Comment>;
      var like = values[1] as Like;
      if (comments == null)
      {
        if (like == null)
        {
          return Visibility.Collapsed;
        }
        return like.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
      }

      if (comments.Count != 0 | (like != null && like.Count != 0))
      {
        return Visibility.Visible;
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