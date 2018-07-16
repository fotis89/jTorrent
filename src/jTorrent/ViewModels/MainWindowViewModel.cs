using System.Windows;
using System.Windows.Input;
using jTorrent.Commands;
using jTorrent.Windows;

namespace jTorrent.ViewModels
{
	public class MainWindowViewModel : BaseViewModel
	{
		public ICommand ExitAppCommand { get; set; } = new DelegateCommand(p => Application.Current.Shutdown());
		public ICommand AboutWindowCommand { get; set; } = new DelegateCommand(p => new AboutWindow().ShowDialog());

		public TransferListViewModel TransferListViewModel { get; }

		public MainWindowViewModel(TransferListViewModel transferListViewModel)
		{
			TransferListViewModel = transferListViewModel;
		}
	}
}