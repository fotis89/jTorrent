using System;
using System.Globalization;
using System.Windows.Data;

namespace jTorrent.Converters
{
	public class SizeToStringConverter : IValueConverter
	{
		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is null) return null;
			var bytes = (long) value;
			var asDouble = (double) bytes;
			if (asDouble < 1024)
			{
				return asDouble + " B";
			}

			if (asDouble < 1_048_576)
			{
				return (asDouble / 1024).ToString("N") + " KB";
			}

			if (asDouble < 1_073_741_824)
			{
				return (asDouble / 1_048_576).ToString("N") + " MB";
			}

			return (asDouble / 1_073_741_824).ToString("N") + " GB";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}