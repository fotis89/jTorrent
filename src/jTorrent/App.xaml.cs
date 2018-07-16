using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using AutoMapper;
using jTorrent.Helpers;
using jTorrent.Models;
using jTorrent.Services;
using jTorrent.ViewModels;
using jTorrent.Windows;
using Microsoft.Win32;

namespace jTorrent
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private SingleInstanceHelper _singleHelper;
		private SytemTrayIconHelper _notifyIconIconHelper;

		private static string AppDataFolder => Path.Combine(AppDataFolderRaw, "jTorrent");
		private static string AppDataFolderRaw => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		private static string DownloadsFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

		protected override void OnStartup(StartupEventArgs e)
		{
			while (!Debugger.IsAttached) Thread.Sleep(100);

			_singleHelper = new SingleInstanceHelper();

			if (_singleHelper.IsNewInstance)
			{
				Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
				SetupMagnetLinkHandler();
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
			SetupAutomapper();

			// TODO: Add IOC Container / Service locator
			var userRequestsHelper = new UserRequestsHelper();
			var persistenceService = new PersistenceService(AppDataFolder);
			var torrentSessionService = new TorrentSessionService(DownloadsFolder);
			var torrentsSessionViewModel = new TransferListViewModel(userRequestsHelper, persistenceService, torrentSessionService);
			var mainWindowViewModel = new MainWindowViewModel(torrentsSessionViewModel);

			var window = new MainWindow(mainWindowViewModel);
			window.Show();

			_notifyIconIconHelper = new SytemTrayIconHelper(window);

			// TODO: Add a message broker
			Current.Exit += (sender, args) =>
			{
				torrentsSessionViewModel.OnAplicationExit();
				torrentSessionService.OnApplicationExit();
			};

			if (e.Args.Any()) torrentsSessionViewModel.AddNewTorrent(e.Args[0]);

			_singleHelper.StartNewProcessListener(torrentsSessionViewModel, window);
		}

		private static void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			switch (e.Exception)
			{
				case OperationException operationException:
					{
						MessageBox.Show(operationException.Reason, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
						e.Handled = true;
						break;
					}
			}
		}

		private static void SetupAutomapper()
		{
			Mapper.Initialize(m =>
			{
				m.CreateMap<TorrentViewModel, Torrent>();
				m.CreateMap<Torrent, TorrentViewModel>();
			});
		}

		private void SetupMagnetLinkHandler()
		{
			var magnetKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\magnet");
			magnetKey.SetValue("", "URL:Magnet link");
			magnetKey.SetValue("Content Type", "application/x-magnet");
			magnetKey.SetValue("URL Protocol", "");

			var path = Assembly.GetExecutingAssembly().Location;
			var defaultIcon = magnetKey.CreateSubKey("DefaultIcon");
			defaultIcon.SetValue("", $"\"{path}\",1");

			var command = magnetKey.CreateSubKey(@"shell\open\command");
			command.SetValue("", $"\"{path}\" \"%1\"");
		}
	}
}