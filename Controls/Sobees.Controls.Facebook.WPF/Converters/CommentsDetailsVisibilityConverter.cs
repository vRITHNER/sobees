#region

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Sobees.Library.BGenericLib;

#endregion

namespace Sobees.Controls.Facebook.Converters
{
  public class CommentsDetailsVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return "Collapsed";
      var comment = value as ObservableCollection<Comment>;
      if (comment == null) return "Collapsed";
      return comment.Count != 0 ? "Visible" : "Collapsed";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}