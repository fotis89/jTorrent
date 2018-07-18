using System.Deployment.Application;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace jTorrent.Windows
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow
	{
		public AboutWindow()
		{
			Owner = Application.Current.MainWindow;

			Version += ApplicationDeployment.IsNetworkDeployed
				? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4)
				: Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
			InitializeComponent();
		}

		public string AppUrl { get; set; } = "https://github.com/fotis89/jTorrent";
		public string Version { get; } = "v";

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(AppUrl);
		}
	}
}