﻿using System;
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

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(" -------------------------- BOOT START ------------------------------------ ");
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine();
            Console.WriteLine();

            System.AppDomain.CurrentDomain.UnhandledException += UnhandledException_Handler;

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
            UpdateTransactionsHandler.Instance.CheckLogFile();
            UpdateTransactionsHandler.Instance.cleanLogFile();

            RollbackTransactionsHandler.Instance.CheckLogFile();
            RollbackTransactionsHandler.Instance.cleanLogFile();

            UpdateHandlers = new Dictionary<string, UpdatesFileHandler>();

            // 4) check if some rollback is necessary ----------------------
            checkForRollbacks();
            IsInitialized = true;
        }






        /*****************************************************************************************/
        private void checkForRollbacks()
        {
            Console.WriteLine("\n\n\n------------------------- STARTING RECOVERY --------------------------");

            List<string> transactionIDs = UpdateTransactionsHandler.Instance.checkForRecovery();
            foreach (string id in transactionIDs)
            {
                string[] tokens = id.Split('|');

                Console.WriteLine("uncommitted transaction: " + id + ". User: " + tokens[0] + "; folder: " + tokens[1]);

                User u = null;
                if (!Users.TryGetValue(tokens[0], out u))
                    throw new Exception("User does not exist");

                bool crashOnCreation = false;
                UpdatesFileHandler handler = getUpdateFileHandler(u, tokens[1]);
                handler.rollBack(id, out crashOnCreation);

                if (crashOnCreation)
                {
                    Console.WriteLine("crash on creating a remote folder. removing it from user's folder list");
                    int index = u.Folders.FindIndex(item => item.FolderName.Equals(tokens[1]));
                    if (index >= 0)
                        u.Folders.RemoveAt(index);

                    UsersFileHandler.Instance.WriteUsersList(Users.Values.ToList());
                }

                UpdateTransactionsHandler.Instance.cleanLogFile(id);
            }

            Console.WriteLine("----------------------- RECOVERY COMPLETED --------------------------");
        }

        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ USER METHODS --------------------------------------------------------------------- */
        /* ----------------------------------------------------------------------------------------------- */

        /******************************************************************************************************/
        public User registerNewUser(string username, string password, string machineName)
        {
            lock (Users)
            {
                User newUser;

                // first user --------------------------------
                if (Users.Count == 0)
                {
                    newUser = new User(username, password);
                    newUser.Installations.Add(new Installation(machineName));
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
                newUser.Installations.Add(new Installation(machineName));
                Users.Add(username, newUser);
                UsersFileHandler.Instance.WriteUsersList(new List<User>(Users.Values));

                Directory.CreateDirectory(RemoteFoldersPath + "\\" + newUser.Username);
                LoginUser(newUser, machineName);
                return newUser;
            }


        }

        /***************************************************************************************************/
        public User LoginUser(string username, string password, string machineName)
        {
            lock (Users)
            {

                User u = null;
                if (!Users.TryGetValue(username, out u))
                    throw new FaultException(new FaultReason("no such user"));

                if (!u.Password.Equals(password))
                    throw new FaultException(new FaultReason(LoginFault.WRONG_USERNAME_OR_PASSWORD));

                LoginUser(u, machineName);
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
        public void changeCredentials(User user, string oldPassword, string newPassword)
        {
            if (!oldPassword.Equals(user.Password))
                throw new FaultException(new FaultReason("Could not perform operation"));

            user.Password = newPassword;
            UsersFileHandler.Instance.WriteUsersList(Users.Values.ToList());
        }



        /*******************************************************************************************/
        private void LoginUser(User u, string machineName)
        {
            lock (ConnectedUsers)
            {
                if (ConnectedUsers.ContainsKey(u.Username))
                    throw new FaultException(new FaultReason(LoginFault.USER_ALREADY_IN));

                if (u.Installations.FindIndex(item => item.MachineName.Equals(machineName)) == -1)
                {
                    Console.WriteLine("new device for this user: " + machineName);
                    Installation installation = new Installation(machineName);
                    u.Installations.Add(installation);
                    u.LastAccessDevice = installation;
                    UsersFileHandler.Instance.WriteUsersList(Users.Values.ToList());
                }
                ConnectedUsers.Add(u.Username, u);
            }
        }



        /*******************************************************************************************/
        public void ChannelFault_Handler(User user)
        {
            Console.WriteLine("Server is handling communication fault");

            foreach (string key in UpdateTransactionsHandler.Instance.ActiveTransactions.Keys)
            {
                string[] tokens = key.Split('|');
                if (tokens[0].Equals(user.Username))
                {
                    Console.WriteLine("Proceed aborting update transaction for: " + tokens[1]);
                    Transaction tr = null;
                    UpdateTransactionsHandler.Instance.ActiveTransactions.TryGetValue(key, out tr);
                    UpdateTransaction ut = (UpdateTransaction)tr;
                    updateAbort(user, ut);
                }
            }

            logoutUser(user);
        }


        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ FOLDERS METHODS ------------------------------------------------------------------ */
        /* ----------------------------------------------------------------------------------------------- */

        public void AddNewFolder(User user, Folder folder)
        {
            int found = user.Folders.FindIndex(item => item.FolderName.Equals(folder.FolderName));
            if (Directory.Exists(RemoteFoldersPath + "\\" + user.Username + "\\" + folder.FolderName) || found >= 0)
                throw new FaultException(new FaultReason("directory already on server"));

            Console.WriteLine("folder to be created not found. ok");
            user.Folders.Add(folder);
            UsersFileHandler.Instance.WriteUsersList(new List<User>(Users.Values));
            Console.WriteLine("User changes saved");
            Directory.CreateDirectory(RemoteFoldersPath + "\\" + user.Username + "\\" + folder.FolderName);
            Console.WriteLine("new folder created");

        }



        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ UPDATE METHODS ------------------------------------------------------------------- */
        /* ----------------------------------------------------------------------------------------------- */


        /**************************************************************************************************/
        public void beginUpdate(UpdateTransaction transaction)
        {

            UpdatesFileHandler handler = getUpdateFileHandler(transaction.User, transaction.BaseFolder);
            UpdateTransactionsHandler.Instance.AddTransaction(transaction);
            Console.WriteLine("Starting update creation");
            handler.createNewUpdate(transaction);
            Console.WriteLine("Update created with success");
        }


        /**************************************************************************************************/
        public void uploadFileStreamed(string username, string transactionID, string baseFolder, string localPath, int type, Stream uploadStream)
        {
            // 1) check if everything ok ---------------------------------------------------
            User user = null;
            ConnectedUsers.TryGetValue(username, out user);

            if (user == null)
                throw new FaultException(new FaultReason(FileTransferFault.USER_NOT_CONNECTED));

            UpdateTransaction transaction = null;
            UpdatesFileHandler handler = null;
            fileTransferChecks(user, baseFolder, transactionID, out transaction, out handler);

            // 2) write to file ------------------------------------------------------------
            handler.AddFileStreamed(transaction, localPath, type, uploadStream);

            // 3) register transaction -----------------------------------------------------
            UpdateTransactionsHandler.Instance.AddOperation(
                transaction,
                type == Update.UpdateEntry.MODIFIED_FILE ? UpdateTransactionsHandler.Operations.UpdateFile : UpdateTransactionsHandler.Operations.NewFile,
                localPath);
        }



        /**************************************************************************************************/
        public void uploadFile(User user, UpdateTransaction transaction, string baseFolder, string localPath, int type, byte[] data)
        {

            // 1) check if everything ok
            if (!ConnectedUsers.ContainsKey(user.Username))
                throw new FaultException(new FaultReason(FileTransferFault.USER_NOT_CONNECTED));

            UpdateTransaction tr = null;
            UpdatesFileHandler handler = null;
            fileTransferChecks(user, baseFolder, transaction.TransactionID, out tr, out handler);

            // 2) write to file
            handler.AddFile(tr, localPath, type, data);

            // 3) register to log
            UpdateTransactionsHandler.Instance.AddOperation(
                tr,
                type == Update.UpdateEntry.MODIFIED_FILE ? UpdateTransactionsHandler.Operations.UpdateFile : UpdateTransactionsHandler.Operations.NewFile,
                localPath);
        }



        /**************************************************************************************************/
        public void deleteFile(User user, UpdateTransaction transaction, string baseFolder, string localPath)
        {
            // 1) check if everything ok
            if (!ConnectedUsers.ContainsKey(user.Username))
                throw new FaultException(new FaultReason(FileTransferFault.USER_NOT_CONNECTED));

            UpdateTransaction tr = null;
            UpdatesFileHandler handler = null;
            fileTransferChecks(user, baseFolder, transaction.TransactionID, out tr, out handler);

            Console.WriteLine("ask handler to delete file");
            handler.deleteFile(tr, localPath);
            UpdateTransactionsHandler.Instance.AddOperation(tr, UpdateTransactionsHandler.Operations.DeleteFile, localPath);
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
            UpdateTransactionsHandler.Instance.AddOperation(tr, UpdateTransactionsHandler.Operations.NewFolder, localPath);
        }



        /**************************************************************************************************/
        public void deleteSubDirectory(User user, UpdateTransaction transaction, string baseFolder, string localPath)
        {
            // 1) check if everything ok
            if (!ConnectedUsers.ContainsKey(user.Username))
                throw new FaultException(new FaultReason(FileTransferFault.USER_NOT_CONNECTED));

            UpdateTransaction tr = null;
            UpdatesFileHandler handler = null;
            fileTransferChecks(user, baseFolder, transaction.TransactionID, out tr, out handler);

            Console.WriteLine("ask handler to delete directory");
            handler.deleteSubDirectory(tr, localPath);
            UpdateTransactionsHandler.Instance.AddOperation(tr, UpdateTransactionsHandler.Operations.DeleteFolder, localPath);
        }


        /**************************************************************************************************/
        public Update updateCommit(UpdateTransaction transaction)
        {
            UpdatesFileHandler handler = null;
            if (!UpdateHandlers.TryGetValue(transaction.User.Username + "|" + transaction.BaseFolder, out handler))
                throw new FaultException(new FaultReason("Transaction handler not found"));

            Update result = handler.commit(transaction);
            Console.WriteLine("update cimmit 1");
            UpdateTransactionsHandler.Instance.CommitTransaction(transaction);
            Console.WriteLine("update commit 2");
            return result;
        }


        /**************************************************************************************************/
        public void updateAbort(User user, UpdateTransaction transaction)
        {
            UpdatesFileHandler handler = null;
            if (!UpdateHandlers.TryGetValue(transaction.User.Username + "|" + transaction.BaseFolder, out handler))
                throw new FaultException(new FaultReason("Transaction handler not found"));

            bool crashOnCreation = false;
            handler.rollBack(transaction.TransactionID, out crashOnCreation);

            if (crashOnCreation)
            {
                Console.WriteLine("crash on creating a remote folder. removing it from user's folder list");
                int index = user.Folders.FindIndex(item => item.FolderName.Equals(transaction.BaseFolder));
                if (index >= 0)
                    user.Folders.RemoveAt(index);

                UsersFileHandler.Instance.WriteUsersList(Users.Values.ToList());
            }

            UpdateTransactionsHandler.Instance.AddOperation(transaction, TransactionsHandler.Operations.Abort, "");
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
            return getUpdateFileHandler(user, baseFolder).getFileHistory(localPath);
        }


        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ DOWNLOAD METHODS ----------------------------------------------------------------- */
        /* ----------------------------------------------------------------------------------------------- */

        public List<Update.UpdateEntry> getUpdateFilesList(User user, Update update)
        {
            return getUpdateFileHandler(user, update.BaseFolder).getUpdateFilesList(update.Number);
        }


        /*****************************************************************************************************/
        public void beginRollback(RollbackTransaction transaction)
        {
            RollbackTransactionsHandler.Instance.AddTransaction(transaction);
        }



        /*****************************************************************************************************/
        public byte[] downloadFile(User user, string baseFolder, string localPath, int updateNumber)
        {
            bool exists = false;
            foreach (Folder f in user.Folders)
                if (f.FolderName.Equals(baseFolder))
                {
                    exists = true;
                    break;
                }

            if (!exists)
                throw new FaultException(new FaultReason(FileTransferFault.UNKNOWN_BASE_FOLDER));

            return getUpdateFileHandler(user, baseFolder).getFile(localPath, updateNumber);
        }



        /*****************************************************************************************************/
        public Stream downloadFileStreamed(string username, string baseFolder, string localPath, int updateNumber)
        {
            User user = null;
            if (!ConnectedUsers.TryGetValue(username, out user))
                throw new FaultException(new FaultReason(FileTransferFault.USER_NOT_CONNECTED));

            bool exists = false;
            foreach (Folder f in user.Folders)
                if (f.FolderName.Equals(baseFolder))
                {
                    exists = true;
                    break;
                }

            if (!exists)
                throw new FaultException(new FaultReason(FileTransferFault.UNKNOWN_BASE_FOLDER));

            return getUpdateFileHandler(user, baseFolder).getFileStreamed(localPath, updateNumber);
        }


        /*****************************************************************************************************/
        public void commitRollback(RollbackTransaction transaction)
        {
            UpdatesFileHandler handler = getUpdateFileHandler(transaction.User, transaction.BaseFolder);
            handler.commitRollback(transaction.UpdateNumber);
            RollbackTransactionsHandler.Instance.CommitTransaction(transaction);
        }


        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ OPTIONS METHODS ------------------------------------------------------------------ */
        /* ----------------------------------------------------------------------------------------------- */

        public void changeFolderOptions(User user, string folderName, Folder updatedFolder)
        {
            Folder oldFolder = checkFolder(user, folderName);

            if (oldFolder.AutoRefreshTime != updatedFolder.AutoRefreshTime)
            {
                Console.WriteLine(user.Username + " wants to change auto refresh");
                oldFolder.AutoRefreshTime = updatedFolder.AutoRefreshTime;
            }

            UsersFileHandler.Instance.WriteUsersList(Users.Values.ToList());
        }


        /*****************************************************************************************************/
        public void removeSynchronizedFolder(User user, string folderName)
        {
            Folder folder = checkFolder(user, folderName);
            getUpdateFileHandler(user, folderName).removeFolder();
            removeUpdateFileHandler(user, folderName);
            user.Folders.Remove(folder);
            UsersFileHandler.Instance.WriteUsersList(Users.Values.ToList());
        }



        /*****************************************************************************************************/
        private Folder checkFolder(User user, string folderName)
        {
            Folder folder = null;
            foreach (Folder f in user.Folders)
            {
                if (f.FolderName.Equals(folderName))
                    folder = f;
            }

            if (folder == null)
                throw new FaultException("No such folder for this user");

            return folder;
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
                if (f.FolderName.Equals(baseFolder))
                {
                    exists = true;
                    break;
                }

            if (!exists)
                throw new FaultException(new FaultReason(FileTransferFault.UNKNOWN_BASE_FOLDER));

            Transaction tr = null;
            if (!UpdateTransactionsHandler.Instance.ActiveTransactions.TryGetValue(transactionID, out tr))
                throw new FaultException(new FaultReason(FileTransferFault.NO_TRANSACTION_ACTIVE));

            if (!tr.GetType().Equals(typeof(UpdateTransaction)))
                throw new FaultException(new FaultReason("wrong transaction type"));

            transaction = (UpdateTransaction)tr;

            if (!UpdateHandlers.TryGetValue(user.Username + "|" + baseFolder, out handler))
                throw new FaultException(new FaultReason("Transaction handler not found"));

        }


        /**************************************************************************************************/
        private UpdatesFileHandler getUpdateFileHandler(User user, string baseFolder)
        {
            UpdatesFileHandler handler = null;

            if (!UpdateHandlers.TryGetValue(user.Username + "|" + baseFolder, out handler))
            {
                foreach (Folder f in user.Folders)
                    if (f.FolderName.Equals(baseFolder))
                    {
                        handler = new UpdatesFileHandler(user, f.FolderName);
                        break;
                    }

                if (handler == null)
                    throw new FaultException(new FaultReason("This user has no folder named " + baseFolder));

                UpdateHandlers.Add(handler.User.Username + "|" + handler.BaseFolder, handler);
            }

            return handler;
        }



        /**************************************************************************************************/
        private void removeUpdateFileHandler(User user, string baseFolder)
        {
            UpdatesFileHandler handler = null;
            UpdateHandlers.TryGetValue(user.Username + "|" + baseFolder, out handler);

            if (handler == null)
            {
                Console.WriteLine("handler to be removed not found");
                return;
            }

            Console.WriteLine("Proceed removing handler");
            UpdateHandlers.Remove(user.Username + "|" + baseFolder);
            handler = null;
        }


        /**************************************************************************************************/
        private void UnhandledException_Handler(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Unhandled exception!");
        }
    }




}
