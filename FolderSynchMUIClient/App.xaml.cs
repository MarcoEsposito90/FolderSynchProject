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
using System.ServiceModel;

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


        /* ---------------------------------------------------------------------------------------------------------------------- */
        /* ------------ CALLBACKS ----------------------------------------------------------------------------------------------- */
        /* ---------------------------------------------------------------------------------------------------------------------- */

        /****************************************************************************************************************************/
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


        /****************************************************************************************************************************/
        private void Application_Exit(object sender, ExitEventArgs e)
        {

            if (User != null)
            {
                User = null;
            }
        }


        /****************************************************************************************************************************/
        private void Application_Login()
        {
            Console.WriteLine("Application_Login called");
            List<Folder> missingFolders = new List<Folder>();
            List<LocalFolder> disappearedFolders = new List<LocalFolder>();
            Dictionary<LocalFolder, Update> needResynch = new Dictionary<LocalFolder, Update>();

            foreach (Folder f in User.Folders)
            {
                Console.WriteLine("Checking for folder: " + f.FolderName);
                int index = _LocalFolders.ToList().FindIndex(item => item.Name.Equals(f.FolderName));
                if (index != -1)
                {
                    LocalFolder lf = _LocalFolders.ElementAt(index);

                    // check if the folder has been moved meanwhile --------------------------------------------
                    if (!checkLocalFolder(lf.Path))
                    {
                        Console.WriteLine("the folder has been moved wihout authorization");
                        disappearedFolders.Add(lf);
                        continue;
                    }

                    // check if local folder needs to be resynched ----------------------------------------------
                    Update lastLocalUpdate = lf.Updates.ElementAt(lf.Updates.Count - 1);
                    List<Update> remoteUpdates = new List<Update>(FolderSynchProxy.getHistory(lf.Name));
                    Update lastRemoteUpdate = remoteUpdates.ElementAt(remoteUpdates.Count - 1);

                    Console.WriteLine("******************************************************************************");
                    Console.WriteLine("Checking synchronization for folder: " + lf.Name);

                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("local update: ");
                    printUpdate(lastLocalUpdate);

                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("remote update: ");
                    printUpdate(lastRemoteUpdate);

                    if (!lastRemoteUpdate.TransactionID.Equals(lastLocalUpdate.TransactionID))
                    {
                        Console.WriteLine(lf.Name + " is not synchronized! localUpdate: " +
                                            lf.Updates.ElementAt(lf.Updates.Count - 1).TransactionID +
                                            "; remoteUpdate: " + remoteUpdates.ElementAt(remoteUpdates.Count - 1).TransactionID);

                        needResynch.Add(lf, lastRemoteUpdate);
                    }

                    FolderWatcher fw = new FolderWatcher(f, _LocalFolders.ElementAt(index));
                    FolderWatchers.Add(fw);
                }
                else
                {
                    Console.WriteLine(f.FolderName + " is missing");
                    missingFolders.Add(f);
                }
                
            }

            // show dialogs if necessary ---------------------------------------------------------------
            if (missingFolders.Count > 0)
            {
                LocalFoldersWarningDialog dialog1 = new LocalFoldersWarningDialog(missingFolders);
                dialog1.ShowDialog();
            }

            if (needResynch.Count > 0)
            {
                FolderDesynchUpdateDialog dialog2 = new FolderDesynchUpdateDialog(needResynch);
                dialog2.ShowDialog();
            }

            if(disappearedFolders.Count > 0)
            {
                string message = "The Folders:\n";
                foreach (LocalFolder lf in disappearedFolders)
                    message += lf.Name + "\n";
                message += "Have been moved from their path and will loose synchronization.\n";
                message += "If you want them to be resynched close the application and move them back to their original path";

                ErrorDialog ed = new ErrorDialog(message);
                ed.ShowDialog();
            }

            // can now start monitoring ---------------------------------------------------------------
            startWatching();
        }


        /****************************************************************************************************************/
        private void Application_Logout()
        {
            // 1) stop watching folder changes ------------------
            foreach (FolderWatcher fw in FolderWatchers)
                fw.stopWatching();

            // 2) save unsaved changes --------------------------
            List<LocalFolder> allLocalFolders = getAllLocalFolders();
            foreach(LocalFolder lf in _LocalFolders)
            {
                int found = allLocalFolders.FindIndex(item => item.Name.Equals(lf.Name));
                if (found >= 0)
                {
                    allLocalFolders.RemoveAt(found);
                    allLocalFolders.Insert(found, lf);
                }
                else
                {
                    allLocalFolders.Add(lf);
                }
            }
            writeAllLocalFolders(allLocalFolders);

            // 2) clean everything ------------------------------
            _LocalFolders.Clear();
            FolderWatchers.Clear();
            FolderSynchProxy = new FolderSynchServiceContractClient();
        }



        /* ------------------------------------------------------------------------------------------------------------------------ */
        /* ------------ USERS FILE ------------------------------------------------------------------------------------------------ */
        /* ------------------------------------------------------------------------------------------------------------------------ */

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
        /* ------------ FOLDERS ------------------------------------------- */
        /* ---------------------------------------------------------------- */

        public void addLocalFolder(Folder folder, string path)
        {
            LocalFolder lf = new LocalFolder(User.Username, folder.FolderName, path);

            List<LocalFolder> list = getAllLocalFolders();
            list.Add(lf);
            writeAllLocalFolders(list);

            _LocalFolders.Add(lf);
            FolderWatcher fw = new FolderWatcher(folder, lf);
            FolderWatchers.Add(fw);
            fw.watch();
        }


        /*****************************************************************/
        public void addLocalFolder(Folder folder, LocalFolder lf)
        {
            List<LocalFolder> list = getAllLocalFolders();
            list.Add(lf);
            writeAllLocalFolders(list);

            _LocalFolders.Add(lf);
            FolderWatcher fw = new FolderWatcher(folder, lf);
            FolderWatchers.Add(fw);
            fw.watch();
        }


        /*****************************************************************/
        public void removeLocalFolder(LocalFolder localFolder)
        {
            Console.WriteLine("removing local folder: " + localFolder.Name);

            List<LocalFolder> list = getAllLocalFolders();
            int index = list.FindIndex(i => i.Name.Equals(localFolder.Name));
            list.RemoveAt(index);
            writeAllLocalFolders(list);

            stopWatching(localFolder);
            _LocalFolders.Remove(localFolder);
            Console.WriteLine("local folder removed");
        }

        /* ---------------------------------------------------------------- */
        /* ------------ FOLDERS AUXILIARY --------------------------------- */
        /* ---------------------------------------------------------------- */

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
        private void writeAllLocalFolders(List<LocalFolder> folders)
        {
            lock (this)
            {
                StreamWriter sw = new StreamWriter("folders.txt", false);
                using (sw)
                {
                    string output = JsonConvert.SerializeObject(folders);
                    sw.WriteLine(output);
                    sw.Close();
                }
            }
        }


        /********************************************************************/
        private bool checkLocalFolder(string path)
        {
            if (!Directory.Exists(path))
                return false;

            return true;
        }


        /********************************************************************/
        private void printUpdate(Update update)
        {
            Console.WriteLine("Update number: " + update.Number);
            Console.WriteLine("timestamp: " + update.Timestamp);
            Console.WriteLine("transaction: " + update.TransactionID);
            Console.WriteLine("number of changes: " + update.UpdateEntries.Length);
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



        /* ---------------------------------------------------------------- */
        /* ------------ UNHANDLED EXCEPTIONS ------------------------------ */
        /* ---------------------------------------------------------------- */

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine("application must handle a " + e.Exception.GetType());

            /* ----------------------------------------------------------------------------------- */
            if (e.Exception.GetType().Equals(typeof(CommunicationObjectFaultedException)) ||
                e.Exception.GetType().Equals(typeof(CommunicationException)) ||
                e.Exception.GetType().Equals(typeof(EndpointNotFoundException)) ||
                e.Exception.GetType().Equals(typeof(ChannelTerminatedException)) ||
                e.Exception.GetType().Equals(typeof(CommunicationObjectAbortedException)) ||
                e.Exception.GetType().Equals(typeof(FaultException)) ||
                e.Exception.GetType().Equals(typeof(ServerTooBusyException)) ||
                e.Exception.GetType().Equals(typeof(ServiceActivationException)))
            {

                Console.WriteLine("Communication with server faulted");
                forceLogout();
                e.Handled = true;
            }
            /* ----------------------------------------------------------------------------------- */
            else
            {
                Console.WriteLine("Panic! unknown exception...");
                Application_Logout();
                _User = null;
                isUserInitialized = false;

                ErrorDialog ed = new ErrorDialog("Due to an unknown error, the application will be closed");
                ed.ShowDialog();
            }

            
        }



        /**************************************************************************************/
        private void forceLogout()
        {
            Application_Logout();
            _User = null;
            isUserInitialized = false;

            ErrorDialog ed = new ErrorDialog("Communication with server faulted. Impossible to access the service");

            if (ed.ShowDialog() == true)
            {
                // change window -------------------------
                MainWindow mw = new MainWindow();
                mw.Show();
                Application.Current.MainWindow = mw;

                Application.Current.Windows[0].Close();
            }

        }
    }
}
