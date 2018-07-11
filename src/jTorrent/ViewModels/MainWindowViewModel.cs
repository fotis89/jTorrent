using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using jTorrent.Commands;
using jTorrent.Windows;

namespace jTorrent.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{



		public ICommand ExitAppCommand { get; set; } = new DelegateCommand(p => Application.Current.Shutdown());
		public ICommand AboutWindowCommand { get; set; } = new DelegateCommand(p => new AboutWindow().ShowDialog());

		public TorrentsSessionViewModel TorrentsSessionViewModel { get; }

		public MainWindowViewModel(TorrentsSessionViewModel torrentsSessionViewModel)
		{
			TorrentsSessionViewModel = torrentsSessionViewModel;
		}
	}
}