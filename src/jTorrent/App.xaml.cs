using System;
using System.IO;
using System.Linq;
using System.Windows;
using jTorrent.Helpers;
using jTorrent.Services;
using jTorrent.ViewModels;
using jTorrent.Windows;

namespace jTorrent
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private SingleInstanceHelper _singleHelper;

		private static string AppDataFolder => Path.Combine(AppDataFolderRaw, "jTorrent");
		private static string AppDataFolderRaw => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		private static string DownloadsFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
		
		protected override void OnStartup(StartupEventArgs e)
		{
			_singleHelper = new SingleInstanceHelper();

			if (_singleHelper.IsNewInstance)
			{
				StartApplication(e);
			}
			else
			{
				_singleHelper.InformExistingInstance(e.Args);
				Current.Shutdown();
			}
		}

		private void StartApplication(StartupEventArgs e)
		{
			// TODO: Add IOC Container / Service locator
			var userRequestsHelper = new UserRequestsHelper();
			var persistenceService = new PersistenceService(AppDataFolder);
			var torrentSessionService = new TorrentSessionService(DownloadsFolder);
			var torrentsSessionViewModel = new TorrentsSessionViewModel(userRequestsHelper, persistenceService, torrentSessionService);
			var mainWindowViewModel = new MainWindowViewModel(torrentsSessionViewModel);

			var window = new MainWindow(mainWindowViewModel);
			window.Show();

			// TODO: Add a message broker
			Current.Exit += (sender, args) =>
			{
				torrentsSessionViewModel.OnAplicationExit();
				torrentSessionService.OnApplicationExit();
			};

			if (e.Args.Any()) torrentsSessionViewModel.AddNewTorrentFromFile(e.Args[0]);

			_singleHelper.HostNewProcessListener(torrentsSessionViewModel, window);
		}
	}
}