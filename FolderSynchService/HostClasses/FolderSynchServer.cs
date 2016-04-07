using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.ServiceModel;
using ServicesProject.HostClasses;

namespace ServicesProject
{
    public class FolderSynchServer
    {
        /* ---------------------------------------------------------------- */
        /* ------------------ STATIC FIELDS ------------------------------- */
        /* ---------------------------------------------------------------- */

        // main directory path is relative to documents folder
        private static string MAIN_DIRECTORY_RELATIVE_PATH = "\\FolderSynchHost";

        // all others are relative to main folder
        private static string REMOTE_FOLDERS_RELATIVE_PATH = "\\RemoteFolders";


        /* ---------------------------------------------------------------- */
        /* ------------------ PROPERTIES ---------------------------------- */
        /* ---------------------------------------------------------------- */

        // files and directories --------------------------------------------------------------
        public String MainDirectoryPath
        {
            get
            {
                string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return docsFolder + MAIN_DIRECTORY_RELATIVE_PATH;
            }
        }

        public String RemoteFoldersPath
        {
            get
            {
                return MainDirectoryPath + REMOTE_FOLDERS_RELATIVE_PATH;
            }
        }


        // status variables --------------------------------------------------------------
        public bool IsInitialized
        {
            get;
            private set;
        }


        // users -----------------------------------------------------------------------
        public List<User> Users
        {
            get;
            private set;
        }


        public List<User> ConnectedUsers
        {
            get;
            private set;
        }

        // updates --------------------------------------------------------------------

        public Dictionary<string,UpdateTransaction> ActiveTransactions
        {
            get;
            private set;
        }

        /* ---------------------------------------------------------------- */
        /* ------------------ CONSTRUCTOR --------------------------------- */
        /* ---------------------------------------------------------------- */

        private static FolderSynchServer _instance = null;
        public static FolderSynchServer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FolderSynchServer();

                return _instance;
            }
        }

        private FolderSynchServer() { }


        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ USER METHODS --------------------------------------------------------------------- */
        /* ----------------------------------------------------------------------------------------------- */

        /*******************************************************************************************************/
        public void Startup()
        {

            // 1) initialize files and directories ---------------------

            string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!Directory.Exists(MainDirectoryPath))
                Directory.CreateDirectory(MainDirectoryPath);

            if (!Directory.Exists(RemoteFoldersPath))
                Directory.CreateDirectory(RemoteFoldersPath);

            UsersFileHandler.Instance.checkUsersFile();

            // 2) initialize users data structures --------------------------
            ConnectedUsers = new List<User>();
            Users = new List<User>();
            UsersFileHandler.Instance.ReadUsersFromFile(Users);

            // 3) initialize transactions data structures -------------------
            TransactionsHandler.Instance.CheckLogFile();

            IsInitialized = true;
        }



        /******************************************************************************************************/
        public User registerNewUser(string username, string password)
        {
            // check if server has startupped ----------------
            if (!IsInitialized)
            {
                Console.WriteLine("Trying to register a user on an uninitalized server");
                throw new FaultException<RegistrationFault>(new RegistrationFault(RegistrationFault.SERVER_ERROR));
            }

            lock (Users)
            {
                User newUser;

                // first user --------------------------------
                if (Users.Count == 0)
                {
                    newUser = new User(username, password);
                    Users.Add(newUser);
                    UsersFileHandler.Instance.WriteUsersList(Users);
                    return newUser;
                }

                // check if username is still available ------------
                foreach (User u in Users)
                {

                    if (u.Username.Equals(username))
                    {
                        Console.WriteLine("registration failed; username not available: " + username);
                        throw new FaultException<RegistrationFault>(new RegistrationFault(RegistrationFault.USERNAME_UNAVAILABLE));
                    }

                }

                // add new user to list -----------------------------
                newUser = new User(username, password);
                Users.Add(newUser);
                UsersFileHandler.Instance.WriteUsersList(Users);

                Directory.CreateDirectory(RemoteFoldersPath + "\\" + newUser.Username);
                LoginUser(newUser);
                return newUser;
            }


        }

        /***************************************************************************************************/
        public User LoginUser(string username, string password)
        {
            lock (Users)
            {
                foreach (User u in Users)
                {
                    if (u.Username.Equals(username))
                    {
                        if (u.Password.Equals(password))
                        {
                            LoginUser(u);
                            return u;
                        }

                        throw new FaultException<LoginFault>(new LoginFault(LoginFault.WRONG_USERNAME_OR_PASSWORD));
                    }
                }

                throw new FaultException<LoginFault>(new LoginFault(LoginFault.WRONG_USERNAME_OR_PASSWORD));
            }

        }


        /*******************************************************************************************/
        public void logoutUser(User user)
        {
            lock (ConnectedUsers)
            {
                if (ConnectedUsers.Contains(user))
                    ConnectedUsers.Remove(user);
            }
            
        }


        /*******************************************************************************************/
        private void LoginUser(User u)
        {
            lock (ConnectedUsers)
            {
                if (ConnectedUsers.Contains(u))
                    throw new FaultException<LoginFault>(new LoginFault(LoginFault.USER_ALREADY_IN));

                ConnectedUsers.Add(u);
            }
        }


        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ FOLDERS METHODS ------------------------------------------------------------------ */
        /* ----------------------------------------------------------------------------------------------- */

        public void AddNewFolder(User user, string folderName)
        {
            if (Directory.Exists(RemoteFoldersPath + "\\" + user.Username + "\\" + folderName))
                throw new FaultException<MyBaseFault>(new MyBaseFault("directory already on server"));

            if(!user.Folders.Contains(folderName))
                user.Folders.Add(folderName);

            UsersFileHandler.Instance.WriteUsersList(Users);
            Directory.CreateDirectory(RemoteFoldersPath + "\\" + user.Username + "\\" + folderName);
        }

        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ UPDATE METHODS ------------------------------------------------------------------- */
        /* ----------------------------------------------------------------------------------------------- */

        public void beginUpdate(UpdateTransaction transaction)
        {
            TransactionsHandler.Instance.AddTransaction(transaction);
        }


        public void uploadFileStreamed(string username, string transactionID, string baseFolder, string localPath, Stream uploadStream)
        {
            User user = null;

            foreach (User u in ConnectedUsers)
                if (u.Username.Equals(username))
                    user = u;

            if (user == null)
                throw new FaultException(new FaultReason("this user is not connected"));

            if (!user.Folders.Contains(baseFolder))
                throw new FaultException<FileTransferFault>(new FileTransferFault(FileTransferFault.UNKNOWN_BASE_FOLDER));

            UpdateTransaction transaction = null;

            if (!TransactionsHandler.Instance.ActiveTransactions.TryGetValue(transactionID, out transaction))
                throw new FaultException(new FaultReason("no transaction active for this upload"));

            string filePath = RemoteFoldersPath + "\\" + user.Username + "\\" + baseFolder + "\\" + localPath;
            FileStream inputStream = new FileStream(filePath, 
                                                    FileMode.Create,
                                                    FileAccess.Write,
                                                    FileShare.None);

            using (inputStream)
            {
                // allocate a 1 KB buffer
                const int bufferSize = 1024;
                byte[] buffer = new byte[bufferSize];
                Console.WriteLine("Buffer allocated");

                // read 1KB per iteration: first they are moved to the buffer, then to the destination file
                int readBytesCount = 0;
                while((readBytesCount = uploadStream.Read(buffer, 0, bufferSize)) > 0)
                {

                    Console.WriteLine("copying... Press enter to go on");
                    //string command = Console.ReadLine();

                    //if (command.Equals("stop"))
                    //{
                    //    throw new FaultException("server no more available");
                    //}

                    inputStream.Write(buffer, 0, readBytesCount);
                }

                inputStream.Close();
                uploadStream.Close();
            }

            TransactionsHandler.Instance.ActiveTransactions.TryGetValue(transactionID, out transaction);
            TransactionsHandler.Instance.AddOperation(transaction, TransactionsHandler.Operations.NewFile, filePath);
        }


        public void uploadFile(User user, UpdateTransaction transaction, string baseFolder, string localPath, byte[] data)
        {

            // 1) check if everything ok
            if (!ConnectedUsers.Contains(user))
                throw new FaultException(new FaultReason("this user is not connected"));

            if (!TransactionsHandler.Instance.ActiveTransactions.ContainsValue(transaction))
                throw new FaultException(new FaultReason("no transaction active for this upload"));


            // 2) write to file
            string filePath = RemoteFoldersPath + "\\" + user.Username + "\\" + baseFolder + "\\" + localPath;
            FileStream inputStream = new FileStream(filePath,
                                                    FileMode.Create,
                                                    FileAccess.Write,
                                                    FileShare.None);

            using (inputStream)
            {
                inputStream.Write(data, 0, data.Length);
                inputStream.Close();
            }

            // 3) register to log
            TransactionsHandler.Instance.AddOperation(transaction, TransactionsHandler.Operations.NewFile, filePath);
        }


        public void updateCommit(UpdateTransaction transaction)
        {
            TransactionsHandler.Instance.CommitTransaction(transaction);
        }
    }




}
