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
using System.IO;

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

        public Dictionary<string, string> KnownUsers
        {
            get;
            private set;
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


            // check for files
            UsersFileCheck();

            

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


        private void UsersFileCheck()
        {
            KnownUsers = new Dictionary<string, string>();

            FileStream fs = new FileStream("users.txt",
                                            FileMode.Open,
                                            FileAccess.Read);

            StreamReader sr = new StreamReader(fs);

            using (fs)
            using (sr)
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {

                    string[] tokens = line.Split(';');
                    KnownUsers.Add(tokens[0], tokens[1]);
                }

                fs.Close();
                sr.Close();
            }
        }

        public void AddKnownUser(string username, string password)
        {
            string pw;

            if (KnownUsers.TryGetValue(username, out pw) && pw.Equals(password))
                return;

            string oldPassword = null;
            KnownUsers.TryGetValue(username, out oldPassword);

            FileStream fs = new FileStream( "users.txt",
                                            oldPassword == null ? FileMode.Append : FileMode.Open,
                                            FileAccess.ReadWrite);

            StreamWriter sw = new StreamWriter(fs);
            

            using (fs)
            using (sw)
            {

                string allFile = null;
                if (oldPassword != null)
                {
                    StreamReader sr = new StreamReader(fs);

                    using (sr)
                    {

                        allFile = sr.ReadToEnd().Replace(username + ";" + oldPassword, username + ";" + password);
                        sr.Close();
                    }
                }
                else
                {
                    if (oldPassword == null)
                        sw.WriteLine(username + ";" + password);
                    else
                        sw.Write(allFile);

                }

                fs.Close();
                sw.Close();
            }
        }
    }
}
