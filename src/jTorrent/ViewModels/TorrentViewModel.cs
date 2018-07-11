using System;
using ltnet;
using Newtonsoft.Json;

namespace jTorrent.ViewModels
{
	public class TorrentViewModel : ViewModelBase
	{
		public TorrentViewModel()
		{
			Guid = Guid.NewGuid();
		}

		public Guid Guid { get; set; }

		public int Number { get; set; }
		public string Name { get; set; }

		public long Size { get; set; }
		public long DownloadRate { get; set; }
		public long UploadRate { get; set; }
		public int Peers { get; set; }
		public int Seeds { get; set; }

		public bool Active { get; set; }
		public float Progress { get; set; }

		public string Folder { get; set; }
		public string FilePath { get; set; }

		[JsonIgnore]
		public torrent_handle TorrentHandle { get; set; }

		public void UpdateStatus()
		{
			var status = TorrentHandle.status();
			Progress = (float)Math.Round(status.progress * 100, 2);
			UploadRate = status.upload_rate;
			DownloadRate = status.download_rate;
			Peers = status.num_peers;
			Seeds = status.num_seeds;
		}

		public void Resume()
		{
			TorrentHandle.resume();
			Active = true;
		}

		public void Pause()
		{
			TorrentHandle.pause();
			Active = false;
		}
	}
}