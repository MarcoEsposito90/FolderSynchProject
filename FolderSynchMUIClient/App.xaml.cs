using FolderSynchMUIClient.FolderSynchService;
using FolderSynchMUIClient.Pages;
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
        public FolderSynchServiceContractClient FolderSynchProxy
        {
            get;
            set;
        }

        public User User
        {
            get;
            set;
        }

        public Folder Folder {
            get;
            set;
        }

        /* ---------------------------------------------------------------- */
        /* ------------ CALLBACKS ----------------------------------------- */
        /* ---------------------------------------------------------------- */
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            // first, open connection with the service
            FolderSynchProxy = new FolderSynchServiceContractClient();
            Application.Current.Resources["ButtonBackgroundHover"] = Brushes.AliceBlue;

            // opening the main window
            //SecondWindow sw = new SecondWindow();
            //sw.Show();
            MainWindow mw = new MainWindow();
            mw.Show();
        }


        
    }
}
