using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Sobees.Controls.Twitter.Cls;

namespace Sobees.Controls.Twitter.Converters
{
  public class BackgroundTypeConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var type = value is int ? (int) value : 0;

      switch (type)
      {
#if !SILVERLIGHT
        case 1:
          return Application.Current.FindResource("PathStyleTweetTypeReplies"); //replies
        case 2:
          return Application.Current.FindResource("PathStyleTweetTypeDm"); //DM
#else
        case 1:
          return Application.Current.Resources["PathStyleTweetTypeReplies"]; //replies
        case 2:
          return Application.Current.Resources["PathStyleTweetTypeDm"]; //DM
#endif

      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}