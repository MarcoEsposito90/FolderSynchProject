using FolderSynchMUIClient.Pages;
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
                    _LocalFolders = setLocalFolders();
                    Application_Login();
                    isUserInitialized = true;
                    Console.WriteLine("user set finished");
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

        private ObservableCollection<LocalFolder> _LocalFolders;
        public ObservableCollection<LocalFolder> LocalFolders
        {
            get
            {
                Console.WriteLine("localfolders getter waiting for lock");
                lock (_LocalFolders)
                {
                    Console.WriteLine("localfolders getter acquired lock");
                    return _LocalFolders;
                }
            }
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
            Console.WriteLine("Application_Login called");

            foreach (Folder f in User.Folders)
            {
                Console.WriteLine("Checking for folder: " + f.FolderName);
                int index = _LocalFolders.ToList().FindIndex(item => item.Name.Equals(f.FolderName));
                if (index != -1)
                {
                    FolderWatcher fw = new FolderWatcher(f, _LocalFolders.ElementAt(index));
                    FolderWatchers.Add(fw);
                }
                else
                    Console.WriteLine("No local Folder");
            }
        }


        /********************************************************************/
        private void Application_Logout()
        {
            // 1) stop watching folder changes ------------------
            foreach (FolderWatcher fw in FolderWatchers)
                fw.stopWatching();

            // 2) save current folders state --------------------
            List<LocalFolder> list = getAllLocalFolders();
            foreach (LocalFolder lf in _LocalFolders)
            {
                int found = list.FindIndex(i => i.Name.Equals(lf.Name) && i.Username.Equals(lf.Username));
                if (found != -1)
                    list.RemoveAt(found);

                list.Add(lf);
            }

            lock (this)
            {
                StreamWriter sw = new StreamWriter("folders.txt", false);
                using (sw)
                {
                    string output = JsonConvert.SerializeObject(list);
                    sw.WriteLine(output);
                    sw.Close();
                }
            }

            // 3) clean everything ------------------------------
            _LocalFolders.Clear();
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
            lock (this)
            {
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

        }

        /********************************************************************/
        public void AddKnownUser(string username, string password)
        {
            string pw;

            if (KnownUsers.TryGetValue(username, out pw) && pw.Equals(password))
                return;

            string oldPassword = null;
            KnownUsers.TryGetValue(username, out oldPassword);

            lock (this)
            {
                string allFile = null;

                if (oldPassword != null)
                {
                    FileStream fs = new FileStream("users.txt",
                                            oldPassword == null ? FileMode.Append : FileMode.Open,
                                            oldPassword == null ? FileAccess.Write : FileAccess.ReadWrite);

                    StreamReader sr = new StreamReader(fs);


                    using (fs)
                    using (sr)
                    {

                        allFile = sr.ReadToEnd().Replace(username + ";" + oldPassword, username + ";" + password);
                        sr.Close();
                    }

                }

                StreamWriter sw = new StreamWriter("users.txt", false);

                using (sw)
                {
                    if (oldPassword == null)
                        sw.WriteLine(username + ";" + password);
                    else
                        sw.Write(allFile);

                    sw.Close();
                }
            }
        }


        /* ---------------------------------------------------------------- */
        /* ------------ FOLDERS FILE -------------------------------------- */
        /* ---------------------------------------------------------------- */

        /********************************************************************/
        private void FoldersFileCheck()
        {
            _LocalFolders = new ObservableCollection<LocalFolder>();
            lock (this)
            {
                FileStream fs = new FileStream("folders.txt", FileMode.Open, FileAccess.Read);

                using (fs)
                    fs.Close();
            }

        }


        /********************************************************************/
        private ObservableCollection<LocalFolder> setLocalFolders()
        {
            Console.WriteLine("localfolders setter waiting for lock");

            lock (_LocalFolders)
            {
                Console.WriteLine("localfolders setter acquired lock");
                if (isUserInitialized)
                    return _LocalFolders;

                _LocalFolders.Clear();
                List<LocalFolder> allLocalFolders = getAllLocalFolders();

                foreach (LocalFolder lf in allLocalFolders)
                    if (lf.Username.Equals(User.Username))
                        _LocalFolders.Add(lf);

                Console.WriteLine("setlocalFolders completed");
                return _LocalFolders;
            }
        }


        /********************************************************************/
        public void addLocalFolder(Folder folder, string path)
        {

            List<LocalFolder> allLocalFolders = getAllLocalFolders();
            LocalFolder lf = new LocalFolder(User.Username, folder.FolderName, path);
            allLocalFolders.Add(lf);

            lock (this)
            {
                StreamWriter sw = new StreamWriter("folders.txt", false);
                using (sw)
                {
                    string output = JsonConvert.SerializeObject(allLocalFolders);
                    sw.WriteLine(output);
                    sw.Close();
                }
            }


            _LocalFolders.Add(lf);

            FolderWatcher fw = new FolderWatcher(folder, lf);
            FolderWatchers.Add(fw);
            fw.watch();
        }


        /*****************************************************************/
        public void addLocalFolder(Folder folder, LocalFolder lf)
        {

            List<LocalFolder> allLocalFolders = getAllLocalFolders();
            allLocalFolders.Add(lf);

            lock (this)
            {
                StreamWriter sw = new StreamWriter("folders.txt", false);
                using (sw)
                {
                    string output = JsonConvert.SerializeObject(allLocalFolders);
                    sw.WriteLine(output);
                    sw.Close();
                }
            }

            _LocalFolders.Add(lf);

            FolderWatcher fw = new FolderWatcher(folder, lf);
            FolderWatchers.Add(fw);
            fw.watch();
        }


        /********************************************************************/
        private List<LocalFolder> getAllLocalFolders()
        {
            List<LocalFolder> allLocalFolders = new List<LocalFolder>();
            string fileContent = null;

            lock (this)
            {
                using (StreamReader sr = new StreamReader("folders.txt"))
                {
                    fileContent = sr.ReadToEnd();
                    sr.Close();
                }
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


        /* ---------------------------------------------------------------- */
        /* ------------ FOLDER WATCHERS ----------------------------------- */
        /* ---------------------------------------------------------------- */


        public void stopWatching(LocalFolder localFolder)
        {
            foreach (FolderWatcher fw in FolderWatchers)
                if (fw.LocalFolder.Name.Equals(localFolder.Name) && fw.IsWatching)
                {
                    fw.stopWatching();
                    break;
                }
        }


        /******************************************************************/
        public void startWatching(LocalFolder localFolder)
        {
            foreach (FolderWatcher fw in FolderWatchers)
                if (fw.LocalFolder.Name.Equals(localFolder.Name) && !fw.IsWatching)
                {
                    fw.watch();
                    break;
                }
        }


        /******************************************************************/
        public void startWatching()
        {
            foreach (FolderWatcher fw in FolderWatchers)
                fw.watch();
        }
    }
}
