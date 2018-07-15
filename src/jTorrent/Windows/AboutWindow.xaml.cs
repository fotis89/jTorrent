using System;
using System.Diagnostics;
using System.Windows;
using jTorrent.ViewModels;

namespace jTorrent.Windows
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow
	{
		public AboutWindow()
		{
			Owner = Application.Current.MainWindow;
			InitializeComponent();
		}

		public string AppUrl { get; set; } = "https://github.com/fotis89/jTorrent";

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(AppUrl);
		}
	}
}