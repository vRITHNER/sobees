#region

using System;
using System.Globalization;
using System.Windows.Data;
using Sobees.Library.BGenericLib;

#endregion

namespace Sobees.Controls.Facebook.Converters
{
  public class DeleteCommentVisibility : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return "Collapsed";
      var comment = value as Comment;
      if (comment != null && comment.CanRemoveComment == 1)
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