using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using jTorrent.Commands;
using jTorrent.Helpers;
using jTorrent.Services;

namespace jTorrent.ViewModels
{
	public class TorrentsSessionViewModel : BaseViewModel
	{
		private readonly TorrentSessionService _torrentSessionService;
		private readonly UserRequestsHelper _userRequestsHelper;
		private readonly PersistenceService _persistenceService;

		public DelegateCommand AddTorrentFromFile { get; set; }
		public DelegateCommand DeleteTorrent { get; set; }
		public DelegateCommand ResumeDownload { get; set; }
		public DelegateCommand PauseDownload { get; set; }

		public ObservableCollection<TorrentViewModel> Torrents { get; } = new ObservableCollection<TorrentViewModel>();

		public TorrentsSessionViewModel(UserRequestsHelper userRequestsHelper, PersistenceService persistenceService, TorrentSessionService torrentSessionService)
		{
			_userRequestsHelper = userRequestsHelper;
			_persistenceService = persistenceService;
			_torrentSessionService = torrentSessionService;

			LoadTorrentInfos();

			Torrents.CollectionChanged += Torrents_CollectionChanged;

			StartUpdateLoop();
			CreateCommands();
		}

		private void LoadTorrentInfos()
		{
			foreach (var torrentViewModel in _persistenceService.LoadTorrentInfos())
			{
				torrentViewModel.TorrentHandle =_torrentSessionService.Add(torrentViewModel.TorrentFilePath, torrentViewModel.Active, out _);
				AddToCollection(torrentViewModel);
			}
		}

		#region Torrent Management Commands

		private void CreateCommands()
		{
			AddTorrentFromFile = CreateAddTorrentFromFileCommand();
			ResumeDownload = CreateResumeDownloadCommand();
			PauseDownload = CreatePauseDownloadCommand();
			DeleteTorrent = CreateDeleteTorrentCommand();
		}

		private DelegateCommand CreateAddTorrentFromFileCommand()
		{
			return new DelegateCommand(p =>
			{
				var filePath = _userRequestsHelper.RequestTorrentFilePath();
				if (filePath != null) AddNewTorrentFromFile(filePath);
			});
		}

		private DelegateCommand CreateResumeDownloadCommand()
		{
			return new DelegateCommand(
				p => TorrentList(p).ForEach(t => t.Resume()),
				p => TorrentList(p).Any(t => !t.Active)
			);
		}

		private DelegateCommand CreatePauseDownloadCommand()
		{
			return new DelegateCommand(
				p => TorrentList(p).ForEach(t => t.Pause()),
				p => TorrentList(p).Any(t => t.Active)
			);
		}

		private DelegateCommand CreateDeleteTorrentCommand()
		{
			return new DelegateCommand(
				p => RemoveTorrents(TorrentList(p)),
				p => TorrentList(p).Any());
		}

		public void ResetCommands()
		{
			PauseDownload.OnCanExecuteChanged();
			ResumeDownload.OnCanExecuteChanged();
			DeleteTorrent.OnCanExecuteChanged();
		}

		private static List<TorrentViewModel> TorrentList(object p)
		{
			return (p as IEnumerable)?.Cast<TorrentViewModel>().ToList() ?? new List<TorrentViewModel>();
		}

		#endregion

		#region Torrents Management

		public void AddNewTorrentFromFile(string filepath)
		{
			var newPath = _persistenceService.PersistTorrentFile(filepath);
			var torrentHandle = _torrentSessionService.Add(newPath, true, out var downloadLocation);
			if (torrentHandle is null) return;

			var torrentFile = torrentHandle.torrent_file();
			var torrentViewModel = new TorrentViewModel
			{
				Name = torrentFile.name(),
				Size = torrentFile.total_size(),
				QueuePosition = torrentHandle.queue_position(),
				TorrentFilePath = newPath,
				TorrentHandle = torrentHandle,
				DownloadLocation = downloadLocation,
				Active = true
			};

			AddToCollection(torrentViewModel);
		}

		private void RemoveTorrents(IReadOnlyList<TorrentViewModel> torrents)
		{
			if (!torrents.Any()) return;
			var (remove, deleteFiles) = _userRequestsHelper.RequestTorrentDeletionConfirmation(torrents);
			if (!remove) return;

			foreach (var torrentViewModel in torrents)
			{
				RemoveFromCollection(torrentViewModel);
				_torrentSessionService.RemoveTorrentFromSession(torrentViewModel.TorrentHandle, deleteFiles);
				_persistenceService.RemoveTorrentFile(torrentViewModel.TorrentFilePath);
			}
		}

		private void AddToCollection(TorrentViewModel torrentViewModel)
		{
			Torrents.Add(torrentViewModel);
			torrentViewModel.PropertyChanged += TorrentViewModel_PropertyChanged;
		}

		private void RemoveFromCollection(TorrentViewModel torrentViewModel)
		{
			Torrents.Remove(torrentViewModel);
			torrentViewModel.PropertyChanged -= TorrentViewModel_PropertyChanged;
		} 

		#endregion

		private void StartUpdateLoop()
		{
			Task.Run(async () =>
			{
				while (true)
				{
					Torrents.ToList().ForEach(t => t.UpdateStatus());
					await Task.Delay(250);
				}
			});
		}

		private void TorrentViewModel_PropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName == nameof(TorrentViewModel.Active)) ResetCommands();
		}

		private void Torrents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ResetCommands();
		}

		public void OnAplicationExit()
		{
			_persistenceService.SaveTorrentInfos(Torrents);
		}
	}
}