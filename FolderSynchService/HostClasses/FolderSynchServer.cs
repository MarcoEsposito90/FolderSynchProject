using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.ServiceModel;

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
        public Dictionary<string, User> Users
        {
            get;
            private set;
        }


        public Dictionary<string, User> ConnectedUsers
        {
            get;
            private set;
        }

        // updates --------------------------------------------------------------------

        public Dictionary<string, UpdateTransaction> ActiveTransactions
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
            ConnectedUsers = new Dictionary<string, User>();
            Users = new Dictionary<string, User>();
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
                    Users.Add(username, newUser);
                    UsersFileHandler.Instance.WriteUsersList(new List<User>(Users.Values));
                    return newUser;
                }

                // check if username is still available ------------

                if (Users.ContainsKey(username))
                {
                    Console.WriteLine("registration failed; username not available: " + username);
                    throw new FaultException<RegistrationFault>(new RegistrationFault(RegistrationFault.USERNAME_UNAVAILABLE));
                }

                // add new user to list -----------------------------
                newUser = new User(username, password);
                Users.Add(username, newUser);
                UsersFileHandler.Instance.WriteUsersList(new List<User>(Users.Values));

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

                User u = null;
                if (!Users.TryGetValue(username, out u))
                    throw new FaultException(new FaultReason("no such user"));

                if (!u.Password.Equals(password))
                    throw new FaultException(new FaultReason(LoginFault.WRONG_USERNAME_OR_PASSWORD));

                LoginUser(u);
                return u;
            }

        }


        /*******************************************************************************************/
        public void logoutUser(User user)
        {
            lock (ConnectedUsers)
            {
                if (ConnectedUsers.ContainsKey(user.Username))
                    ConnectedUsers.Remove(user.Username);
                else
                    throw new FaultException(new FaultReason(LogoutFault.NO_SUCH_USER));

            }

        }


        /*******************************************************************************************/
        private void LoginUser(User u)
        {
            lock (ConnectedUsers)
            {
                if (ConnectedUsers.ContainsKey(u.Username))
                    throw new FaultException(new FaultReason(LoginFault.USER_ALREADY_IN));

                ConnectedUsers.Add(u.Username,u);
            }
        }


        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ FOLDERS METHODS ------------------------------------------------------------------ */
        /* ----------------------------------------------------------------------------------------------- */

        public void AddNewFolder(User user, Folder folder)
        {
            if (Directory.Exists(RemoteFoldersPath + "\\" + user.Username + "\\" + folder.Name))
                throw new FaultException<MyBaseFault>(new MyBaseFault("directory already on server"));

            if (!user.Folders.ContainsKey(folder.Name))
                user.Folders.Add(folder.Name, folder);

            UsersFileHandler.Instance.WriteUsersList(new List<User>(Users.Values));
            Directory.CreateDirectory(RemoteFoldersPath + "\\" + user.Username + "\\" + folder.Name);
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
            ConnectedUsers.TryGetValue(username, out user);

            if (user == null)
                throw new FaultException(new FaultReason(FileTransferFault.USER_NOT_CONNECTED));

            if (!user.Folders.ContainsKey(baseFolder))
                throw new FaultException(new FaultReason(FileTransferFault.UNKNOWN_BASE_FOLDER));

            UpdateTransaction transaction = null;

            if (!TransactionsHandler.Instance.ActiveTransactions.TryGetValue(transactionID, out transaction))
                throw new FaultException(new FaultReason(FileTransferFault.NO_TRANSACTION_ACTIVE));

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
                while ((readBytesCount = uploadStream.Read(buffer, 0, bufferSize)) > 0)
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
            if (!ConnectedUsers.ContainsKey(user.Username))
                throw new FaultException(new FaultReason(FileTransferFault.USER_NOT_CONNECTED));

            if (!TransactionsHandler.Instance.ActiveTransactions.ContainsValue(transaction))
                throw new FaultException(new FaultReason(FileTransferFault.NO_TRANSACTION_ACTIVE));


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
