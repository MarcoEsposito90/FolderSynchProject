using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FolderSynchWPFClient
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static FolderSynchService.FolderSynchServiceClient FolderSynchProxy;

        // callback for application startup
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
