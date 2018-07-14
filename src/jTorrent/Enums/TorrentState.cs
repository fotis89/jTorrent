namespace jTorrent.Enums
{
	public enum TorrentState
	{
		Paused,
		CheckingFiles,
		DownloadingMetadata,
		Downloading,
		Finished,
		Seeding,
		Allocating,
		CheckingResumeData,
	}
}