using System;
using System.IO;
using System.Linq;
using ltnet;

namespace jTorrent.Services
{
	public class TorrentSessionService
	{
		private readonly session _session;
		private readonly string _downloadsFolder;

		public TorrentSessionService(string downloadsFolder)
		{
			_downloadsFolder = downloadsFolder;
			_session = new session();
		}

		public void OnApplicationExit()
		{
			_session.pause();
			_session.Dispose();
		}

		public (torrent_handle torrentHandle, string downloadLocation, string name) AddTorrent(string filePath, bool active)
		{
			var torrentAddInfo = AddToSession(filePath);
			torrentAddInfo.torrentHandle.auto_managed(false);

			if (active) torrentAddInfo.torrentHandle.resume();
			else torrentAddInfo.torrentHandle.pause();

			return torrentAddInfo;
		}

		public void RemoveTorrent(torrent_handle torrentHandle, bool deleteFromDisk)
		{
			try
			{
				torrentHandle.pause();
				_session.remove_torrent(torrentHandle, deleteFromDisk ? 1 : 0);
			}
			catch
			{
				//
			}
			finally
			{
				torrentHandle.Dispose();
			}
		}

		private (torrent_handle torrentHandle, string downloadLocation, string name) AddToSession(string source)
		{
			using (var addTorrentParams = CreateAddTorrentParams(source))
			{
				var name = addTorrentParams.ti?.name() ?? addTorrentParams.name;
				var alreadyExists = _session.get_torrents().Any(t => t.torrent_file().name() == name);
				if (alreadyExists) throw new OperationException("Torrent already exists");

				var torrentHandle = _session.add_torrent(addTorrentParams);
				var downloadLocation = Path.Combine(_downloadsFolder, name);
				return (torrentHandle, downloadLocation, name);
			}
		}

		private add_torrent_params CreateAddTorrentParams(string source)
		{
			var addTorrentParams = new add_torrent_params { save_path = _downloadsFolder };

			try
			{
				if (source.StartsWith("magnet:"))
				{
					var errorCode = new error_code();
					new magnet_uri().parse_magnet_uri(source, addTorrentParams, errorCode);
				}

				if (File.Exists(source))
				{
					addTorrentParams.ti = new torrent_info(source);
				}
			}
			catch
			{
				addTorrentParams.Dispose();
				addTorrentParams = null;
			}

			return addTorrentParams ?? throw new OperationException("Torrent source is invalid");
		}
	}
}
