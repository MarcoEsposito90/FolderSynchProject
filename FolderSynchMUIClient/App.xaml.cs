﻿using FolderSynchMUIClient.Pages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using FolderSynchMUIClient.FolderSynchService;
using FolderSynchMUIClient.StreamedTransferService;
using System.IO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

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

        /*********************************************************************/
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


        /*********************************************************************/
        private bool isUserInitialized = false;
        private User _User;
        public User User
        {
            get { return _User; }
            set
            {

                if (value != null)
                {
                    _User = value;
                    LocalFolders = getLocalFolders();
                    Application_Login();
                    isUserInitialized = true;
                }
                else
                {
                    FolderSynchProxy.logoutUser(_User);
                    Application_Logout();
                    _User = value;
                    isUserInitialized = false;
                }


            }
        }

        public LocalFolder Folder
        {
            get;
            set;
        }


        /*********************************************************************/
        public Dictionary<string, string> KnownUsers
        {
            get;
            private set;
        }

        public ObservableCollection<LocalFolder> LocalFolders
        {
            get;
            private set;
        }


        /*********************************************************************/
        public List<FolderWatcher> FolderWatchers
        {
            get;
            private set;
        }


        /* ---------------------------------------------------------------- */
        /* ------------ CALLBACKS ----------------------------------------- */
        /* ---------------------------------------------------------------- */

        /********************************************************************/
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            // first, open connection with the service
            FolderSynchProxy = new FolderSynchServiceContractClient();
            StreamTransferProxy = new StreamedTransferContractClient();
            Application.Current.Resources["ButtonBackgroundHover"] = Brushes.AliceBlue;

            // check for files
            UsersFileCheck();
            FoldersFileCheck();

            FolderWatchers = new List<FolderWatcher>();

            MainWindow mw = new MainWindow();
            mw.Show();
        }


        /********************************************************************/
        private void Application_Exit(object sender, ExitEventArgs e)
        {

            if (User != null)
            {
                User = null;
            }
        }


        /********************************************************************/
        private void Application_Login()
        {
            // start watching user's folders

            foreach (Folder f in User.Folders)
            {
                int index = LocalFolders.ToList().FindIndex(item => item.Name.Equals(f.Name));
                if (index >= 0)
                {
                    FolderWatcher fw = new FolderWatcher(f, LocalFolders.ElementAt(index));
                    FolderWatchers.Add(fw);
                    fw.watch();
                }
            }
        }


        /********************************************************************/
        private void Application_Logout()
        {

            foreach (FolderWatcher fw in FolderWatchers)
                fw.stopWatching();

            LocalFolders.Clear();
            FolderWatchers.Clear();
            FolderSynchProxy = new FolderSynchServiceContractClient();
        }

        /* ---------------------------------------------------------------- */
        /* ------------ USERS FILE ---------------------------------------- */
        /* ---------------------------------------------------------------- */

        /********************************************************************/
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
                while ((line = sr.ReadLine()) != null)
                {

                    string[] tokens = line.Split(';');
                    KnownUsers.Add(tokens[0], tokens[1]);
                }

                sr.Close();
                fs.Close();
            }
        }

        /********************************************************************/
        public void AddKnownUser(string username, string password)
        {
            string pw;

            if (KnownUsers.TryGetValue(username, out pw) && pw.Equals(password))
                return;

            string oldPassword = null;
            KnownUsers.TryGetValue(username, out oldPassword);

            FileStream fs = new FileStream("users.txt",
                                            oldPassword == null ? FileMode.Append : FileMode.Open,
                                            oldPassword == null ? FileAccess.Write : FileAccess.ReadWrite);

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

                sw.Close();
                fs.Close();
            }
        }


        /* ---------------------------------------------------------------- */
        /* ------------ FOLDERS FILE -------------------------------------- */
        /* ---------------------------------------------------------------- */

        /********************************************************************/
        private void FoldersFileCheck()
        {

            LocalFolders = new ObservableCollection<LocalFolder>();
            FileStream fs = new FileStream("folders.txt", FileMode.Open, FileAccess.Read);

            using (fs)
                fs.Close();
        }


        /********************************************************************/
        public ObservableCollection<LocalFolder> getLocalFolders()
        {
            if (isUserInitialized && LocalFolders != null)
                return LocalFolders;

            LocalFolders = new ObservableCollection<LocalFolder>();
            List<LocalFolder> allLocalFolders = getAllLocalFolders();

            foreach (LocalFolder lf in allLocalFolders)
                if (lf.Username.Equals(User.Username))
                    LocalFolders.Add(lf);

            return LocalFolders;
        }


        /********************************************************************/
        public void addLocalFolder(Folder folder, string path)
        {

            List<LocalFolder> allLocalFolders = getAllLocalFolders();
            LocalFolder lf = new LocalFolder(User.Username, folder.Name, path);
            allLocalFolders.Add(lf);

            StreamWriter sw = new StreamWriter("folders.txt", false);
            using (sw)
            {
                string output = JsonConvert.SerializeObject(allLocalFolders);
                sw.WriteLine(output);
                sw.Close();
            }

            LocalFolders.Add(lf);

            FolderWatcher fw = new FolderWatcher(folder, lf);
            FolderWatchers.Add(fw);
            fw.watch();
        }


        /*****************************************************************/
        public void addLocalFolder(Folder folder, LocalFolder lf)
        {

            List<LocalFolder> allLocalFolders = getAllLocalFolders();
            allLocalFolders.Add(lf);

            StreamWriter sw = new StreamWriter("folders.txt", false);
            using (sw)
            {
                string output = JsonConvert.SerializeObject(allLocalFolders);
                sw.WriteLine(output);
                sw.Close();
            }

            LocalFolders.Add(lf);

            FolderWatcher fw = new FolderWatcher(folder, lf);
            FolderWatchers.Add(fw);
            fw.watch();
        }


        /********************************************************************/
        private List<LocalFolder> getAllLocalFolders()
        {
            List<LocalFolder> allLocalFolders = new List<LocalFolder>();
            string fileContent = null;

            using (StreamReader sr = new StreamReader("folders.txt"))
            {
                fileContent = sr.ReadToEnd();
                sr.Close();
            }

            if (fileContent == null)
                return allLocalFolders;

            Object o = JsonConvert.DeserializeObject(fileContent);
            Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)o;

            foreach (Object i in array)
            {
                Newtonsoft.Json.Linq.JObject jo = (Newtonsoft.Json.Linq.JObject)i;
                LocalFolder lf = (LocalFolder)jo.ToObject(typeof(LocalFolder));
                allLocalFolders.Add(lf);
            }

            return allLocalFolders;
        }
    }
}
