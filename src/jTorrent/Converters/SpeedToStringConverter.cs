using System;
using System.Globalization;

namespace jTorrent.Converters
{
	public class SpeedToStringConverter : SizeToStringConverter
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return base.Convert(value, targetType, parameter, culture) + "/s";
		}
	}
}