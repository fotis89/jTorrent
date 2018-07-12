using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using jTorrent.ViewModels;
using jTorrent.Windows;

namespace jTorrent.Helpers
{
	public class SingleInstanceHelper
	{
		private readonly int _currentSessionId;
		private readonly bool _isNewInstance;
		private readonly Mutex _mutex;
		private ServiceHost _uniqueInstanceHost;

		private string UriString => $"net.pipe://localhost/jTorrent{_currentSessionId}/singleinstanceservice";

		public SingleInstanceHelper()
		{
			_currentSessionId = Process.GetCurrentProcess().SessionId;
			_mutex = new Mutex(false, $"application.jTorrent.{_currentSessionId}", out _isNewInstance);
		}

		public bool IsNewInstance => _isNewInstance;

		public void InformExistingInstance(string[] args)
		{
			var host = new ChannelFactory<ISingleInstanceService>(new NetNamedPipeBinding(), new EndpointAddress(UriString)).CreateChannel();
			host.FocusApplication();
			if (args.Any()) host.AddNewTorrentFile(args[0]);
		}

		public void HostNewProcessListener(TorrentsSessionViewModel torrentsSessionViewModel, MainWindow window)
		{
			_uniqueInstanceHost = new ServiceHost(new SingleInstanceService(torrentsSessionViewModel, window), new Uri(UriString));
			_uniqueInstanceHost.Open();
		}
	}

	[ServiceContract]
	public interface ISingleInstanceService
	{
		[OperationContract(IsOneWay = true)]
		void AddNewTorrentFile(string filePath);

		[OperationContract(IsOneWay = true)]
		void FocusApplication();
	}

	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
	public class SingleInstanceService : ISingleInstanceService
	{
		private readonly TorrentsSessionViewModel _torrentsSessionViewModel;
		private readonly MainWindow _mainWindow;

		public SingleInstanceService(TorrentsSessionViewModel torrentsSessionViewModel, MainWindow mainWindow)
		{
			_torrentsSessionViewModel = torrentsSessionViewModel;
			_mainWindow = mainWindow;
		}

		public void AddNewTorrentFile(string filePath)
		{
			_torrentsSessionViewModel.AddNewTorrentFromFile(filePath);
		}

		public void FocusApplication()
		{
			if (_mainWindow.WindowState == WindowState.Minimized) _mainWindow.WindowState = WindowState.Normal;
			_mainWindow.Activate();
			_mainWindow.Focus();
		}
	}
}
