using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using jTorrent.Commands;
using jTorrent.Helpers;
using ltnet;
using Newtonsoft.Json;

namespace jTorrent.ViewModels
{
	public class TorrentsSessionViewModel : ViewModelBase
	{
		private readonly string _appDataFile;
		private readonly session _session = new session();
		private readonly string _downloadsFolder;
		private readonly FilePathHelper _filePathHelper;
		private readonly MessageboxHelper _messageboxHelper;
		private static readonly object FileLocker = new object();

		public DelegateCommand AddTorrentFromFile { get; set; }
		public DelegateCommand DeleteTorrent { get; set; }
		public DelegateCommand ResumeDownload { get; set; }
		public DelegateCommand PauseDownload { get; set; }

		public ObservableCollection<TorrentViewModel> Torrents { get; } = new ObservableCollection<TorrentViewModel>();

		public TorrentsSessionViewModel(string appDataFile, string downloadsFolder, FilePathHelper filePathHelper, MessageboxHelper messageboxHelper)
		{
			_appDataFile = appDataFile;
			_downloadsFolder = downloadsFolder;
			_filePathHelper = filePathHelper;
			_messageboxHelper = messageboxHelper;

			InitializeTorrentsCollection();

			Torrents.CollectionChanged += Torrents_CollectionChanged;
			AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

			StartUpdateLoop();
			CreateCommands();
		}

		#region Commands

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
				var filePath = _filePathHelper.GetTorrentFilePath();
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

		private static List<TorrentViewModel> TorrentList(object p)
		{
			return (p as IEnumerable)?.Cast<TorrentViewModel>().ToList() ?? new List<TorrentViewModel>();
		}

		#endregion

		private void CurrentDomain_ProcessExit(object sender, EventArgs e)
		{
			SaveTorrentInfos();
			_session.pause();
			_session.Dispose();
			SaveTorrentInfos();
		}

		private void InitializeTorrentsCollection()
		{
			if (!File.Exists(_appDataFile)) return;

			var appDate = File.ReadAllText(_appDataFile);
			var torrentInfos = JsonConvert.DeserializeObject<List<TorrentViewModel>>(appDate);

			foreach (var torrentInfo in torrentInfos)
			{
				torrentInfo.TorrentHandle = AddToSession(torrentInfo.FilePath);
				torrentInfo.PropertyChanged += TorrentInfo_PropertyChanged;
				Torrents.Add(torrentInfo);

				if (torrentInfo.Active)
				{
					torrentInfo.Resume();
				}
				else
				{
					torrentInfo.Pause();
				}
			}
		}

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

		private torrent_handle AddToSession(string infoFilePath)
		{
			using (var addTorrentParams = new add_torrent_params { save_path = _downloadsFolder, ti = new torrent_info(infoFilePath) })
			{
				return _session.add_torrent(addTorrentParams);
			}
		}

		private void TorrentInfo_PropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName == nameof(TorrentViewModel.Active)) ResetCommands();
		}

		private void Torrents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ResetCommands();
		}

		public void ResetCommands()
		{
			PauseDownload.OnCanExecuteChanged();
			ResumeDownload.OnCanExecuteChanged();
			DeleteTorrent.OnCanExecuteChanged();
		}

		private void SaveTorrentInfos()
		{
			lock (FileLocker)
			{
				var appData = JsonConvert.SerializeObject(Torrents.ToList());
				File.WriteAllText(_appDataFile, appData);
			}
		}

		public void AddNewTorrentFromFile(string filepath)
		{
			try
			{
				// todo: copy torrent to appdata folder
				var torrentHandle = AddToSession(filepath);
				var torrentFile = torrentHandle.torrent_file();
				var torrentInfo = new TorrentViewModel
				{
					Name = torrentFile.name(),
					Size = torrentFile.total_size(),
					Number = NextNumberForTorrent,
					FilePath = filepath,
					TorrentHandle = torrentHandle,
					Folder = Path.Combine(_downloadsFolder, torrentFile.name()),
					Active = true
				};

				torrentInfo.PropertyChanged += TorrentInfo_PropertyChanged;
				torrentInfo.Resume();
				Torrents.Add(torrentInfo);
			}
			catch
			{
				MessageBox.Show("Could not read torrent file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void RemoveTorrents(List<TorrentViewModel> torrents)
		{
			if (!torrents.Any()) return;
			var (remove, deleteFiles) = _messageboxHelper.ConfirmTorrentDeletion(torrents);
			if (remove) torrents.ForEach(t => RemoveTorrent(t, deleteFiles));
		}

		private void RemoveTorrent(TorrentViewModel torrentViewModel, bool deleteFromDisk)
		{
			try
			{
				torrentViewModel.PropertyChanged -= TorrentInfo_PropertyChanged;
				Torrents.Remove(torrentViewModel);
				torrentViewModel.Pause();
				_session.remove_torrent(torrentViewModel.TorrentHandle, deleteFromDisk ? 1 : 0);
			}
			catch (Exception)
			{
				//
			}
			finally
			{
				torrentViewModel.TorrentHandle.Dispose();
			}
		}

		private int NextNumberForTorrent => Torrents.Any() ? Torrents.Max(t => t.Number) + 1 : 1;
	}
}