using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using jTorrent.Models;
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
			var torrentsModel = torrents.Select(Mapper.Map<Torrent>).ToList();
			var appData = JsonConvert.SerializeObject(torrentsModel);

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
				var torrents = JsonConvert.DeserializeObject<List<TorrentViewModel>>(appDate);
				torrentInfos = torrents.Select(Mapper.Map<TorrentViewModel>).ToList();
			}
			return torrentInfos;
		}

		public string PersistTorrentFile(string filepath)
		{
			var newFilePath = Path.Combine(_torrentsFolder, $"{Guid.NewGuid()}.torrent");
			File.Copy(filepath, newFilePath);
			return newFilePath;
		}

		public void RemoveTorrentFile(string torrentFilePath)
		{
			if (string.IsNullOrWhiteSpace(torrentFilePath) || !File.Exists(torrentFilePath)) return;
			File.Delete(torrentFilePath);
		}
	}
}