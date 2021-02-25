using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Sobees.Controls.Twitter.Cls;
using Sobees.Controls.Twitter.ViewModel;
using Sobees.Tools.Logging;

namespace Sobees.Controls.Twitter.Converters
{
  public class FavoriteButtonVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        var grid = value as Control;
        TwitterWorkspaceViewModel ctx = null;
        if (grid != null)
        {
          ctx = grid.Tag as TwitterWorkspaceViewModel;
        }

        if ((ctx == null) || (!ctx.WorkspaceSettings.Type.Equals(EnumTwitterType.Favorites)))
          return targetType;

        return Visibility.Collapsed;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }

      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
  public class UnFavoriteButtonVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        var grid = value as Control;
        TwitterWorkspaceViewModel ctx = null;
        if (grid != null)
        {
          ctx = grid.Tag as TwitterWorkspaceViewModel;
        }

        if ((ctx == null) || (ctx.WorkspaceSettings.Type.Equals(EnumTwitterType.Favorites)))
          return Visibility.Visible;

        return Visibility.Collapsed;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }

      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}