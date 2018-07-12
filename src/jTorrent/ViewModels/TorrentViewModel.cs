using System;
using ltnet;
using Newtonsoft.Json;

namespace jTorrent.ViewModels
{
	public class TorrentViewModel : BaseViewModel
	{
		public TorrentViewModel()
		{
			Guid = Guid.NewGuid();
		}

		public Guid Guid { get; set; }

		public int QueuePosition { get; set; }
		public string Name { get; set; }
		public long Size { get; set; }
		public string DownloadLocation { get; set; }
		public string TorrentFilePath { get; set; }

		[JsonIgnore]
		public long DownloadRate { get; set; }
		[JsonIgnore]
		public long UploadRate { get; set; }
		[JsonIgnore]
		public int Peers { get; set; }
		[JsonIgnore]
		public int Seeds { get; set; }

		public bool Active { get; set; }
		public float Progress { get; set; }


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
			QueuePosition = TorrentHandle.queue_position();
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