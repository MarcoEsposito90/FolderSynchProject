using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class FolderSynchApplication : Application
    {
        public static FolderSynchService.FolderSynchServiceClient FolderSynchProxy;


        private void Application_Startup(object sender, StartupEventArgs e)
        {

            // first, open connection with the service
            FolderSynchProxy = new FolderSynchService.FolderSynchServiceClient();

            // opening the main window
            MainWindow mw = new MainWindow();
            mw.Show();
        }
    }
}
