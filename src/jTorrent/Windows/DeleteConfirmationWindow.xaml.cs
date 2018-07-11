using System.Collections.Generic;
using System.Windows;
using jTorrent.ViewModels;

namespace jTorrent.Windows
{
	/// <summary>
	/// Interaction logic for DeleteConfirmationWindow.xaml
	/// </summary>
	public partial class DeleteConfirmationWindow
	{
		public bool DeleteFiles { get; set; }

		public DeleteConfirmationWindow(Window ownerWindow, IReadOnlyList<TorrentViewModel> selectedTorrents)
		{
			Owner = ownerWindow;
			InitializeComponent();

			var selectedTorrentsCount = selectedTorrents.Count;
			var torrentName = selectedTorrentsCount > 1
				? $"these {selectedTorrentsCount} torrents"
				: $"'{selectedTorrents[0].Name}'";
			Message.Text = $"Are you sure you want to delete {torrentName} from the transfer list?";
		}

		private void YesButton_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			DeleteFiles = DeleteFilesCheckbox.IsChecked == true;
			Close();
		}

		private void NoButton_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}