#region

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Sobees.Library.BGenericLib;

#endregion

#if SILVERLIGHT
using Sobees.Library.BGenericLib;
using Sobees.Tools.Binding;
#endif

namespace Sobees.Controls.Facebook.Converters
{
  public class CommentsMultiValueVisibilityConverter : IMultiValueConverter
  {
    #region IMultiValueConverter Members

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (values == null || values[0] == null || values[1] == null)
        {
          return "Collapsed";
        }

        var nbComments = 0;
        var nbLikes = 0;

        if (values[0].GetType().Equals(typeof (ObservableCollection<Comment>)))
        {
          var tempComm = new ObservableCollection<Comment>();
          tempComm = (ObservableCollection<Comment>) values[0];
          if (tempComm != null) nbComments = tempComm.Count;
        }


        if (values[1].GetType().Equals(typeof (Like)))
        {
          var tempLike = new Like();
          tempLike = (Like) values[1];
          if (tempLike != null) nbLikes = tempLike.Count;
        }


        if (nbComments > 0 || nbLikes > 0)
        {
          return "Visible";
        }

        return "Collapsed";
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return "Collapsed";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }

//#endif
}