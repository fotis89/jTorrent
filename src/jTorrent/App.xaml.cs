using System;
using System.IO;
using System.Linq;
using System.Windows;
using jTorrent.Helpers;
using jTorrent.ViewModels;
using jTorrent.Windows;

namespace jTorrent
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static string AppDataFolder => Path.Combine(AppDataFolderRaw, "jTorrent");
		private static string AppDataFolderRaw => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		private static string AppDataFile => Path.Combine(AppDataFolder, "appData.json");
		private static string DownloadsFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

		protected override void OnStartup(StartupEventArgs e)
		{
			Directory.CreateDirectory(AppDataFolder);

			var torrentsSessionViewModel = new TorrentsSessionViewModel(AppDataFile, DownloadsFolder, new FilePathHelper(), new MessageboxHelper());
			var window = new MainWindow(new MainWindowViewModel(torrentsSessionViewModel));
			window.Show();

			if(e.Args.Any()) torrentsSessionViewModel.AddNewTorrentFromFile(e.Args[0]);
		}
	}
}