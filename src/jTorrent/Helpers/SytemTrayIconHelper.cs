using System;
using System.Windows.Forms;
using jTorrent.Windows;

namespace jTorrent.Helpers
{
	public class SytemTrayIconHelper
	{
		private readonly MainWindow _mainWindow;
		private NotifyIcon _notifyIcon;

		public SytemTrayIconHelper(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			_notifyIcon = CreateNotifyIcon();
			_notifyIcon.Click += NotifyIcon_Click;
		}

		private NotifyIcon CreateNotifyIcon()
		{
			return new NotifyIcon
			{
				Icon = Properties.Resources.icon,
				Visible = true,
				ContextMenu = new ContextMenu
				{
					MenuItems =
					{
						new MenuItem("Show", ShowMainWindow),
						new MenuItem("Exit", ApplicationExit)
					}
				}
			};
		}

		private void NotifyIcon_Click(object sender, EventArgs eventArgs)
		{
			_mainWindow.BringToFrond();
		}

		private void ShowMainWindow(object sender, EventArgs eventArgs)
		{
			_mainWindow.BringToFrond();
		}

		private void ApplicationExit(object sender, EventArgs eventArgs)
		{
			System.Windows.Application.Current.Shutdown();
		}
	}
}