#region

using System;
using System.Globalization;
using System.Windows.Data;
#if !SILVERLIGHT
using Sobees.Library.BLocalizeLib;

#else
using Telerik.Windows.Controls;

#endif

#endregion

namespace Sobees.Controls.Facebook.Converters
{
  public class NumberCommentConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return null;
      if (!value.GetType().Equals(typeof (int))) return null;

      int nbComments;
      int.TryParse(value.ToString(),
        out nbComments);
      var comments = string.Empty;
      if (nbComments > 1)
      {
#if SILVERLIGHT
				comments = string.Format("{0}",
                                 nbComments) + LocalizationManager.GetString("txtComments");
#else
        comments = $"{nbComments}" + new LocText("Sobees.Configuration.BGlobals:Resources:txtComments").ResolveLocalizedValue();
#endif
      }
      else if (nbComments == 1)
      {
#if SILVERLIGHT
        comments = string.Format("{0}",
                                 nbComments) + LocalizationManager.GetString("txtComment");
#else
        comments = $"{nbComments}" + new LocText("Sobees.Configuration.BGlobals:Resources:txtComment").ResolveLocalizedValue();
        ;
#endif
      }

      return comments;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}