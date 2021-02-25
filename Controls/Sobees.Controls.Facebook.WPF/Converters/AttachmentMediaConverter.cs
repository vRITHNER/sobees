#region

using System;
using System.Globalization;
using System.Windows.Data;
using Sobees.Library.BFacebookLibV2.Objects.Attachments;

#endregion

namespace Sobees.Controls.Facebook.Converters
{
  public class AttachementMediaConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var fa = value as FacebookAttachement;
      if (fa?.Medias == null) return null;
      if (fa.Medias.Medias.Count < 1) return null;

      if (parameter != null && parameter.ToString().ToLower().Equals("src"))
        return fa.Medias.Medias[0].Image.Source;

      return fa.Medias.Medias[0].Image.Source;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}