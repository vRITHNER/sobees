using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Sobees.Infrastructure.Cls;
#if !SILVERLIGHT

#endif

namespace Sobees.Converters
{
  public class TypeServiceImageConverter : IValueConverter
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
          return "/Sobees.Templates;Component/Images/Services/twitter.png";
        }
        if (type == EnumAccountType.Facebook)
        {
          return "/Sobees.Templates;Component/Images/Services/facebook.png";
        }
        if (type == EnumAccountType.MySpace)
        {
          return "/Sobees.Templates;Component/Images/Services/myspace.png";
        }
        if (type == EnumAccountType.LinkedIn)
        {
          return "/Sobees.Templates;Component/Images/Services/linkedin.png";
        }
        if (type == EnumAccountType.TwitterSearch)
        {
          return "/Sobees.Templates;Component/Images/Services/search.png";
        }    
      if (type == EnumAccountType.Rss)
        {
          return "/Sobees.Templates;Component/Images/Services/rss.png";
        }  
      if (type == EnumAccountType.NyTimes)
        {
          return "/Sobees.Templates;Component/Images/Services/nytimes.png";
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