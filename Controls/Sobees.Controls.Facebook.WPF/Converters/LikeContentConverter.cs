#region

using System;
using System.Globalization;
using System.Windows.Data;
using Sobees.Library.BGenericLib;
#if !SILVERLIGHT
using Sobees.Library.BLocalizeLib;

#else
using Telerik.Windows.Controls;

#endif

#endregion

namespace Sobees.Controls.Facebook.Converters
{
  public class LikeContentConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return null;
      if (!value.GetType().Equals(typeof (Like))) return null;
      var like = value as Like;
      if (like == null) return null;
      if (like.LikeIt == 0)
      {
#if SILVERLIGHT
			  return LocalizationManager.GetString("btnLike");
#else
        return new LocText("Sobees.Configuration.BGlobals:Resources:btnLike").ResolveLocalizedValue();
#endif
      }
#if SILVERLIGHT
      return LocalizationManager.GetString("btnUnLike");
#else
      return new LocText("Sobees.Configuration.BGlobals:Resources:btnUnLike").ResolveLocalizedValue();
#endif
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}