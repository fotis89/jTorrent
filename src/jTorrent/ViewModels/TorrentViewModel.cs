using System;
using jTorrent.Enums;
using ltnet;

namespace jTorrent.ViewModels
{
	public class TorrentViewModel : BaseViewModel
	{
		public int QueuePosition { get; set; }
		public string Name { get; set; }
		public long Size { get; set; }
		public string DownloadLocation { get; set; }
		public string TorrentSource { get; set; }
		public long DownloadRate { get; set; }
		public long UploadRate { get; set; }
		public int Peers { get; set; }
		public int Seeds { get; set; }
		public TorrentState State { get; set; }
		public bool Active { get; set; }
		public float Progress { get; set; }
		public torrent_handle TorrentHandle { get; set; }

		public void UpdateStatus()
		{
			var status = TorrentHandle.status();
			Progress = (float)Math.Round(status.progress * 100, 2);
			UploadRate = status.upload_rate;
			DownloadRate = status.download_rate;
			Peers = status.num_peers;
			Seeds = status.num_seeds;
			State = (TorrentState)(Active ? status.state : 0);

			var ti = TorrentHandle.torrent_file();
			if (ti != null)
			{
				Size = ti.total_size();
			}
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