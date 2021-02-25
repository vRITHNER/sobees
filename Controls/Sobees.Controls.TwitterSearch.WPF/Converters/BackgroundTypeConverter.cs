using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Sobees.Controls.Twitter.Converters
{
  public class BackgroundTypeConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var type = value is int ? (int) value : 0;
#if !SILVERLIGHT
      switch (type)
      {
        case 1:
          return Application.Current.FindResource("PathStyleTweetTypeReplies"); //replies
        case 2:
          return Application.Current.FindResource("PathStyleTweetTypeDm"); //DM

      }
#endif
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}