using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static FolderSynchService.FolderSynchServiceContractClient FolderSynchProxy;


        private void Application_Startup(object sender, StartupEventArgs e)
        {

            // first, open connection with the service
            FolderSynchProxy = new FolderSynchService.FolderSynchServiceContractClient();
            Application.Current.Resources["ButtonBackgroundHover"] = Brushes.AliceBlue;

            // opening the main window
            MainWindow mw = new MainWindow();
            mw.Show();
        }
    }
}
