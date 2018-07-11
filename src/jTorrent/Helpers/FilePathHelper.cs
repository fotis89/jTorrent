using Microsoft.Win32;

namespace jTorrent.Helpers
{
	public class FilePathHelper
	{
		private readonly OpenFileDialog _openFileDialog;

		public FilePathHelper()
		{
			_openFileDialog = new OpenFileDialog
			{
				CheckFileExists = true,
				Filter = "Torrent file|*.torrent",
			};
		}

		public string GetTorrentFilePath()
		{
			var dialogResult = _openFileDialog.ShowDialog();
			return dialogResult != true ? null : _openFileDialog.FileName;
		}
	}
}