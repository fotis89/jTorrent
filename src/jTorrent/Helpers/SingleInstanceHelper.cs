using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
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
			if (args.Any()) host.AddNewTorrent(args[0]);
		}

		public void StartNewProcessListener(TorrentsSessionViewModel torrentsSessionViewModel, MainWindow window)
		{
			_uniqueInstanceHost = new ServiceHost(new SingleInstanceService(torrentsSessionViewModel, window), new Uri(UriString));
			_uniqueInstanceHost.Open();
		}

		#region Wcf Service

		[ServiceContract]
		private interface ISingleInstanceService
		{
			[OperationContract(IsOneWay = true)]
			void AddNewTorrent(string filePath);

			[OperationContract(IsOneWay = true)]
			void FocusApplication();
		}

		[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
		private class SingleInstanceService : ISingleInstanceService
		{
			private readonly TorrentsSessionViewModel _torrentsSessionViewModel;
			private readonly MainWindow _mainWindow;

			public SingleInstanceService(TorrentsSessionViewModel torrentsSessionViewModel, MainWindow mainWindow)
			{
				_torrentsSessionViewModel = torrentsSessionViewModel;
				_mainWindow = mainWindow;
			}

			public void AddNewTorrent(string filePath)
			{
				_torrentsSessionViewModel.AddNewTorrent(filePath);
			}

			public void FocusApplication()
			{
				_mainWindow.BringToFrond();
			}
		}

		#endregion
	}
}
