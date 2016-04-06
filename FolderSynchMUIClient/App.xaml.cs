using FolderSynchMUIClient.Pages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ServicesProject;
using FolderSynchMUIClient.FolderSynchService;
using FolderSynchMUIClient.StreamedTransferService;

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        /* ---------------------------------------------------------------- */
        /* ------------ STATICS ------------------------------------------- */
        /* ---------------------------------------------------------------- */

        public static int MAX_BUFFERED_TRANSFER_FILE_SIZE = 10485760;   // 10 MB



        /* ---------------------------------------------------------------- */
        /* ------------ PROPERTIES ---------------------------------------- */
        /* ---------------------------------------------------------------- */
        public FolderSynchServiceContractClient FolderSynchProxy
        {
            get;
            private set;
        }

        public StreamedTransferContractClient StreamTransferProxy
        {
            get;
            private set;
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
            StreamTransferProxy = new StreamedTransferContractClient();
            Application.Current.Resources["ButtonBackgroundHover"] = Brushes.AliceBlue;

            // opening the main window
            //SecondWindow sw = new SecondWindow();
            //sw.Show();
            MainWindow mw = new MainWindow();
            mw.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {

            if(User != null)
            {
                FolderSynchProxy.logoutUser(User);
                User = null;
            }
        }
    }
}
