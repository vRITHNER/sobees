using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Sobees.Library.BTwitterLib;

namespace Sobees.Controls.Twitter.Converters
{
	public class ViaVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var te = value as TwitterEntry;
			if (te == null) return Visibility.Collapsed;

			if (te.SourceName == null) return Visibility.Collapsed;

			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
