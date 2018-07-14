namespace jTorrent
{
	public static class Extensions
	{
		public static string SplitCamelCase(this string input)
		{
			if (string.IsNullOrEmpty(input)) return string.Empty; ;
			return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
		}
	}
}