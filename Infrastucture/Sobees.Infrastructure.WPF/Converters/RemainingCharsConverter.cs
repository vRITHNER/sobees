using System;
using System.Globalization;
using System.Windows.Data;
using Sobees.Infrastructure.Controls;
#if SILVERLIGHT
using Sobees.Tools.Binding;
#endif

namespace Sobees.Infrastructure.Converters
{
//#if SILVERLIGHT
//  public class RemainingCharsConverter : IValueConverter
//  {
//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//      BTextBox textBox = value as BTextBox;
//      if(textBox == null) return null;

//      if(textBox.MaxCharacters < 1) return null;
//      if(string.IsNullOrEmpty(textBox.Text)) return textBox.MaxCharacters;

//      return textBox.MaxCharacters - textBox.Text.Length;
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//      throw new NotImplementedException();
//    }
//  }
//#else
  public class RemainingCharsConverter : IMultiValueConverter
  {
    #region IMultiValueConverter Members

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if(values == null ||values[0] == null || values[1] == null )
        {
          return 0;
        }
        var textLength = (int) values[0];
        var maxNbOfChars = (int) values[1];

        return (maxNbOfChars - textLength).ToString();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return 0;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
//#endif
}