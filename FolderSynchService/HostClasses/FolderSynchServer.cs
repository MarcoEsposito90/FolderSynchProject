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

        public Dictionary<string, UpdatesFileHandler> UpdateHandlers
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
        /* ------------------------------- BOOT PROCEDURE ------------------------------------------------ */
        /* ----------------------------------------------------------------------------------------------- */
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
            UpdateHandlers = new Dictionary<string, UpdatesFileHandler>();

            // 4) check if some rollback is necessary ----------------------
            checkForRollbacks();

            IsInitialized = true;
        }

        /*****************************************************************************************/
        private void checkForRollbacks()
        {
            foreach (string id in TransactionsHandler.Instance.checkForRecovery())
            {
                string[] tokens = id.Split('|');

                Console.WriteLine("uncommitted transaction: " + id + ". User: " + tokens[0] + "; folder: " + tokens[1]);

                User u = null;
                if (!Users.TryGetValue(tokens[0], out u))
                    throw new Exception("User does not exist");

                UpdatesFileHandler handler = getUpdateFileHandler(u, tokens[1]);
                handler.rollBack(id);

                TransactionsHandler.Instance.cleanLogFile(id);
            }
        }

        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ USER METHODS --------------------------------------------------------------------- */
        /* ----------------------------------------------------------------------------------------------- */

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
                throw new FaultException(new FaultReason("directory already on server"));

            user.Folders.Add(folder);

            UsersFileHandler.Instance.WriteUsersList(new List<User>(Users.Values));
            Directory.CreateDirectory(RemoteFoldersPath + "\\" + user.Username + "\\" + folder.Name);
        }

        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ UPDATE METHODS ------------------------------------------------------------------- */
        /* ----------------------------------------------------------------------------------------------- */


        /**************************************************************************************************/
        public void beginUpdate(UpdateTransaction transaction)
        {

            UpdatesFileHandler handler = getUpdateFileHandler(transaction.User, transaction.FolderName);
            TransactionsHandler.Instance.AddTransaction(transaction);
            handler.createNewUpdate(transaction);
        }


        /**************************************************************************************************/
        public void uploadFileStreamed(string username, string transactionID, string baseFolder, string localPath, Stream uploadStream)
        {
            // 1) check if everything ok ---------------------------------------------------
            User user = null;
            ConnectedUsers.TryGetValue(username+baseFolder, out user);

            if (user == null)
                throw new FaultException(new FaultReason(FileTransferFault.USER_NOT_CONNECTED));

            UpdateTransaction transaction = null;
            UpdatesFileHandler handler = null;
            fileTransferChecks(user, baseFolder, transactionID, out transaction, out handler);

            // 2) write to file ------------------------------------------------------------
            handler.AddFileStreamed(transaction, localPath, uploadStream);

            // 3) register transaction -----------------------------------------------------
            TransactionsHandler.Instance.AddOperation(transaction, TransactionsHandler.Operations.NewFile, localPath);
        }

        
        
        /**************************************************************************************************/
        public void uploadFile(User user, UpdateTransaction transaction, string baseFolder, string localPath, byte[] data)
        {

            // 1) check if everything ok
            if (!ConnectedUsers.ContainsKey(user.Username))
                throw new FaultException(new FaultReason(FileTransferFault.USER_NOT_CONNECTED));

            UpdateTransaction tr = null;
            UpdatesFileHandler handler = null;
            fileTransferChecks(user, baseFolder, transaction.TransactionID, out tr, out handler);

            // 2) write to file
            handler.AddFile(tr, localPath, data);

            // 3) register to log
            TransactionsHandler.Instance.AddOperation(tr, TransactionsHandler.Operations.NewFile, localPath);
        }



        /**************************************************************************************************/
        public void addSubDirectory(User user, UpdateTransaction transaction, string baseFolder, string localPath)
        {

            // 1) check if everything ok
            if (!ConnectedUsers.ContainsKey(user.Username))
                throw new FaultException(new FaultReason(FileTransferFault.USER_NOT_CONNECTED));

            UpdateTransaction tr = null;
            UpdatesFileHandler handler = null;
            fileTransferChecks(user, baseFolder, transaction.TransactionID, out tr, out handler);

            handler.addSubDirectory(tr, localPath);
            TransactionsHandler.Instance.AddOperation(tr, TransactionsHandler.Operations.NewFolder, localPath);
        }



        /**************************************************************************************************/
        public void updateCommit(UpdateTransaction transaction)
        {
            UpdatesFileHandler handler = null;
            if (!UpdateHandlers.TryGetValue(transaction.User.Username+transaction.FolderName, out handler))
                throw new FaultException(new FaultReason("Transaction handler not found"));

            handler.commit(transaction);
            TransactionsHandler.Instance.CommitTransaction(transaction);
        }


        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ HISTORY METHODS ------------------------------------------------------------------ */
        /* ----------------------------------------------------------------------------------------------- */

        public List<Update> getHistory(User user, string baseFolder)
        {
            return getUpdateFileHandler(user, baseFolder).getHistory();
        }


        /*****************************************************************************************************/
        public List<Update.UpdateEntry> getFileHistory(User user, string baseFolder, string localPath)
        {
            return getUpdateFileHandler(user,baseFolder).getFileHistory(localPath);
        }


        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ DOWNLOAD METHODS ----------------------------------------------------------------- */
        /* ----------------------------------------------------------------------------------------------- */

        public byte[] downloadFile(User user, string baseFolder, string localPath, int updateNumber)
        {
            bool exists = false;
            foreach (Folder f in user.Folders)
                if (f.Name.Equals(baseFolder))
                {
                    exists = true;
                    break;
                }

            if (!exists)
                throw new FaultException(new FaultReason(FileTransferFault.UNKNOWN_BASE_FOLDER));

            return getUpdateFileHandler(user, baseFolder).getFile(localPath, updateNumber);
        }

        public Stream downloadFileStreamed(string username, string baseFolder, string localPath, int updateNumber)
        {
            User user = null;
            if (!ConnectedUsers.TryGetValue(username, out user))
                throw new FaultException(new FaultReason(FileTransferFault.USER_NOT_CONNECTED));

            bool exists = false;
            foreach (Folder f in user.Folders)
                if (f.Name.Equals(baseFolder))
                {
                    exists = true;
                    break;
                }

            if (!exists)
                throw new FaultException(new FaultReason(FileTransferFault.UNKNOWN_BASE_FOLDER));

            return getUpdateFileHandler(user, baseFolder).getFileStreamed(localPath, updateNumber);
        }

        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ AUXILIARY METHODS ---------------------------------------------------------------- */
        /* ----------------------------------------------------------------------------------------------- */

        private void fileTransferChecks(User user, 
                                        string baseFolder, 
                                        string transactionID,
                                        out UpdateTransaction transaction, 
                                        out UpdatesFileHandler handler)
        {

            bool exists = false;
            foreach (Folder f in user.Folders)
                if (f.Name.Equals(baseFolder))
                {
                    exists = true;
                    break;
                }

            if (!exists)
                throw new FaultException(new FaultReason(FileTransferFault.UNKNOWN_BASE_FOLDER));

            if (!TransactionsHandler.Instance.ActiveTransactions.TryGetValue(transactionID, out transaction))
                throw new FaultException(new FaultReason(FileTransferFault.NO_TRANSACTION_ACTIVE));

            if (!UpdateHandlers.TryGetValue(user.Username + baseFolder, out handler))
                throw new FaultException(new FaultReason("Transaction handler not found"));

        }


        /**************************************************************************************************/
        private UpdatesFileHandler getUpdateFileHandler(User user, string baseFolder)
        {
            UpdatesFileHandler handler = null;

            if (!UpdateHandlers.TryGetValue(user.Username + baseFolder, out handler))
            {
                foreach (Folder f in user.Folders)
                    if (f.Name.Equals(baseFolder))
                    {
                        handler = new UpdatesFileHandler(user, f.Name);
                        break;
                    }

                if (handler == null)
                    throw new FaultException(new FaultReason("This user has no folder named " + baseFolder));

                UpdateHandlers.Add(handler.User.Username + handler.BaseFolder, handler);
            }

            return handler;
        }

    }




}
