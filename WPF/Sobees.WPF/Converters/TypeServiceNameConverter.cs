using System;
using System.Globalization;
using System.Windows.Data;
using Sobees.Infrastructure.Cls;


#if !SILVERLIGHT
using Sobees.Library.BLocalizeLib;
#else
using Telerik.Windows.Controls;
#endif

namespace Sobees.Converters
{
  public class TypeServiceNameConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value,
                          Type targetType,
                          object parameter,
                          CultureInfo culture)
    {
      var type = value is EnumAccountType ? (EnumAccountType) value : EnumAccountType.Twitter;

      if (type == EnumAccountType.Twitter)
      {
        return "Twitter";
      }
      if (type == EnumAccountType.Facebook)
      {
        return "Facebook";
      }
      //if (type == EnumAccountType.MySpace)
      //{
      //  return "MySpace";
      //}
      if (type == EnumAccountType.LinkedIn)
      {
        return "LinkedIn";
      }
      //if (type == EnumAccountType.Rss)
      //{
      //  return "RSS";
      //}
      //if (type == EnumAccountType.NyTimes)
      //{
      //  return "New York Times";
      //}
      if (type == EnumAccountType.TwitterSearch)
      {
        return new LocText("Sobees.Configuration.BGlobals:Resources:RTS").ResolveLocalizedValue();
      }
      return string.Empty;
    }

    public object ConvertBack(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}