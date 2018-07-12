using System.Collections.Generic;
using System.IO;
using jTorrent.ViewModels;
using Newtonsoft.Json;

namespace jTorrent.Services
{
	public class PersistenceService
	{
		private static readonly object FileLocker = new object();
		private readonly string _appDataFile;
		private readonly string _appDataFolder;
		private readonly string _torrentsFolder;

		public PersistenceService(string appDataFolder)
		{
			_appDataFolder = appDataFolder;
			_appDataFile = Path.Combine(appDataFolder, "torrents.json");
			_torrentsFolder = Path.Combine(appDataFolder, "torrents");

			Directory.CreateDirectory(_appDataFolder);
			Directory.CreateDirectory(_torrentsFolder);
		}

		public void SaveTorrentInfos(IReadOnlyCollection<TorrentViewModel> torrents)
		{
			var appData = JsonConvert.SerializeObject(torrents);

			lock (FileLocker)
			{
				File.WriteAllText(_appDataFile, appData);
			}
		}

		public List<TorrentViewModel> LoadTorrentInfos()
		{
			var torrentInfos = new List<TorrentViewModel>();
			lock (FileLocker)
			{
				if (!File.Exists(_appDataFile)) return torrentInfos;
				var appDate = File.ReadAllText(_appDataFile);
				torrentInfos = JsonConvert.DeserializeObject<List<TorrentViewModel>>(appDate);
			}
			return torrentInfos;
		}

		public string PersistTorrentFile(string filepath)
		{
			var newFilePath = CreateNewFilePath(filepath);
			File.Copy(filepath, newFilePath);
			return newFilePath;
		}

		private string CreateNewFilePath(string filepath)
		{
			var fileName = Path.GetFileName(filepath);
			var newFilePath = Path.Combine(_torrentsFolder, fileName);
			return newFilePath;
		}

		public void RemoveTorrentFile(string torrentFilePath)
		{
			File.Delete(torrentFilePath);
		}
	}
}