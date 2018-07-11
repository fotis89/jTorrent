using System.Collections.Generic;
using System.Linq;
using System.Windows;
using jTorrent.ViewModels;
using jTorrent.Windows;

namespace jTorrent.Helpers
{
	public class MessageboxHelper
	{
		public (bool remove, bool deleteFiles) ConfirmTorrentDeletion(IReadOnlyList<TorrentViewModel> torrents)
		{
			var mainWindow = Application.Current.Windows.OfType<MainWindow>().First();
			var window = new DeleteConfirmationWindow(mainWindow, torrents);

			var remove = window.ShowDialog() ?? false;
			return (remove, window.DeleteFiles);
		}
	}
}
