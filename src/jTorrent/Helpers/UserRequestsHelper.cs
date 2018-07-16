using System.Collections.Generic;
using System.Linq;
using System.Windows;
using jTorrent.ViewModels;
using jTorrent.Windows;
using Microsoft.Win32;

namespace jTorrent.Helpers
{
	public class UserRequestsHelper
	{
		private readonly OpenFileDialog _openFileDialog;

		public UserRequestsHelper()
		{
			_openFileDialog = new OpenFileDialog
			{
				CheckFileExists = true,
				Filter = "Torrent file|*.torrent",
			};
		}

		public string RequestTorrentFilePath()
		{
			var dialogResult = _openFileDialog.ShowDialog();
			return dialogResult != true ? null : _openFileDialog.FileName;
		}

		public (bool remove, bool deleteFiles) RequestTorrentDeletionConfirmation(IReadOnlyList<TorrentViewModel> torrents)
		{
			var window = new DeleteConfirmationWindow(torrents);

			var remove = window.ShowDialog() ?? false;
			return (remove, window.DeleteFiles);
		}
	}
}