using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using jTorrent.ViewModels;

namespace jTorrent.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindowViewModel MainWindowViewModel { get; }

		public MainWindow(MainWindowViewModel mainWindowViewModel)
		{
			MainWindowViewModel = mainWindowViewModel;
			InitializeComponent();
		}

		private void TorrentsDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var fileName = TorrentsDataGrid.SelectedItems.OfType<TorrentViewModel>().ToList().First().DownloadLocation;
			if (Directory.Exists(fileName) || File.Exists(fileName))
			{
				Process.Start(fileName);
			}
		}

		private void TorrentsDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			MainWindowViewModel.TorrentsSessionViewModel.ResetCommands();
		}

		public void BringToFrond()
		{
			if (Visibility != Visibility.Visible) Visibility = Visibility.Visible;
			if (WindowState == WindowState.Minimized) WindowState = WindowState.Normal;
			Activate();
			Focus();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			Visibility = Visibility.Collapsed;
		}
	}
}