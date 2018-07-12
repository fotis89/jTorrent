﻿using System;
using System.IO;
using System.Windows;
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
			var torrents = _session.get_torrents();

			foreach (var torrentHandle in torrents)
			{
				if(torrentHandle.need_save_resume_data()) torrentHandle.save_resume_data(0);
			}
			_session.pause();
			_session.Dispose();
		}

		public torrent_handle Add(string filePath, bool active, out string downloadLocation)
		{
			downloadLocation = null;

			try
			{
				var torrentHandle = AddFileToSession(filePath, out downloadLocation);
				torrentHandle.auto_managed(false);
				if (active)
				{
					torrentHandle.resume();
				}
				else
				{
					torrentHandle.pause();
				}
				return torrentHandle;
			}
			catch (Exception)
			{
				MessageBox.Show("Could not read torrent file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

			return null;
		}

		public void RemoveTorrentFromSession(torrent_handle torrentHandle, bool deleteFromDisk)
		{
			try
			{
				torrentHandle.pause();
				_session.remove_torrent(torrentHandle, deleteFromDisk ? 1 : 0);
			}
			catch (Exception)
			{
				//
			}
			finally
			{
				torrentHandle.Dispose();
			}
		}

		private torrent_handle AddFileToSession(string filePath, out string downloadLocation)
		{
			using (var addTorrentParams = new add_torrent_params { save_path = _downloadsFolder, ti = new torrent_info(filePath) })
			{
				var torrentHandle = _session.add_torrent(addTorrentParams);
				downloadLocation = Path.Combine(_downloadsFolder, torrentHandle.torrent_file().name());
				return torrentHandle;
			}
		}
	}
}