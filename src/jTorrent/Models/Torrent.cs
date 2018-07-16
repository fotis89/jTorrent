using System;

namespace jTorrent.Models
{
	public class Torrent
	{
		public int Version { get; set; } = 1;
		public Guid Guid { get; set; }
		public int QueuePosition { get; set; }
		public string Name { get; set; }
		public long Size { get; set; }
		public string DownloadLocation { get; set; }
		public string TorrentSource { get; set; }
		public bool Active { get; set; }
		public string MagnetUri { get; set; }
	}
}