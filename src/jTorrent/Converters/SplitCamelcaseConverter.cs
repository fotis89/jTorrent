﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace jTorrent.Converters
{
	public class SplitCamelCaseConverter : IValueConverter
	{
		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value?.ToString().SplitCamelCase();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}