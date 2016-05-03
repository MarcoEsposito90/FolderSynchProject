using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [ServiceBehavior(   InstanceContextMode = InstanceContextMode.PerSession,
                        ConcurrencyMode = ConcurrencyMode.Single)]
    public class FolderSynchImplementation : FolderSynchServiceContract
    {
        User currentUser = null;
        Dictionary<string,UpdateTransaction> ActiveUpdateTransactions = null;
        Dictionary<string, RollbackTransaction> ActiveRollbackTransactions = null;

        /* ---------------------------------------------------------------------- */
        /* ------------------------ USER ---------------------------------------- */
        /* ---------------------------------------------------------------------- */

        public User loginUser(string username, string password)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");

            currentUser = FolderSynchServer.Instance.LoginUser(username, password);
            ActiveUpdateTransactions = new Dictionary<string, UpdateTransaction>();
            ActiveRollbackTransactions = new Dictionary<string, RollbackTransaction>();

            Console.WriteLine(currentUser.Username + " wants to login");
            Console.WriteLine(currentUser.Username + " Folders list: ");
            foreach (Folder f in currentUser.Folders)
                Console.WriteLine(f.FolderName);

            Console.WriteLine(" -------------------------------------------------------------------------- ");
            return currentUser;
        }



        /***************************************************************************************************/
        public void logoutUser(User user)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(user.Username + "is logging out");
            FolderSynchServer.Instance.logoutUser(user);
            currentUser = null;
            Console.WriteLine(" -------------------------------------------------------------------------- ");
        }



        /***************************************************************************************************/
        public User RegisterNewUser(string username, string password)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            currentUser = FolderSynchServer.Instance.registerNewUser(username, password);
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            return currentUser;
        }



        /***************************************************************************************************/
        public void changeCredentials(string oldPassword, string newPassword)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + " wants to change password");
            FolderSynchServer.Instance.changeCredentials(currentUser, oldPassword, newPassword);
            Console.WriteLine(" -------------------------------------------------------------------------- ");
        }



        /* ------------------------------------------------------------------------------- */
        /* ------------------------ UPDATES ---------------------------------------------- */
        /* ------------------------------------------------------------------------------- */

        public UpdateTransaction beginUpdate(string baseFolder, DateTime timestamp)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + "wants to start a transaction");
            Console.WriteLine("timestamp = " + timestamp.ToString());
            Console.WriteLine("baseFodler = " + baseFolder);
            UpdateTransaction tr = new UpdateTransaction(currentUser, baseFolder, timestamp);
            FolderSynchServer.Instance.beginUpdate(tr);
            ActiveUpdateTransactions.Add(tr.TransactionID,tr);
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            return tr;
        }

        /***************************************************************************************************/
        public void uploadFile(string transactionID, string baseFolder, string localPath, int type, byte[] data)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            UpdateTransaction transaction = null;
            

            if (!ActiveUpdateTransactions.TryGetValue(transactionID, out transaction))
                throw new FaultException(new FaultReason("This transaction is not active for the user"));

            Console.WriteLine(currentUser.Username + "wants to add a new file");
            FolderSynchServer.Instance.uploadFile(currentUser, transaction, baseFolder, localPath, type, data);
            Console.WriteLine(" -------------------------------------------------------------------------- ");
        }


        /***************************************************************************************************/
        public void addSubDirectory(string transactionID, string baseFolder, string localPath)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            UpdateTransaction transaction = null;

            if (!ActiveUpdateTransactions.TryGetValue(transactionID, out transaction))
                throw new FaultException(new FaultReason("This transaction is not active for the user"));

            Console.WriteLine(currentUser.Username + "wants to add a subDirectory");
            FolderSynchServer.Instance.addSubDirectory(currentUser, transaction, baseFolder, localPath);
            Console.WriteLine(" -------------------------------------------------------------------------- ");

        }


        /***************************************************************************************************/
        public void deleteFile(string transactionID, string baseFolder, string localPath)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");

            UpdateTransaction transaction = null;

            if (!ActiveUpdateTransactions.TryGetValue(transactionID, out transaction))
                throw new FaultException(new FaultReason("This transaction is not active for the user"));

            Console.WriteLine(currentUser.Username + "wants to delete a file");
            FolderSynchServer.Instance.deleteFile(currentUser, transaction, baseFolder, localPath);
            Console.WriteLine("Delete file return");
            Console.WriteLine(" -------------------------------------------------------------------------- ");

        }


        /***************************************************************************************************/
        public void deleteSubDirectory(string transactionID, string baseFolder, string localPath)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            UpdateTransaction transaction = null;

            if (!ActiveUpdateTransactions.TryGetValue(transactionID, out transaction))
                throw new FaultException(new FaultReason("This transaction is not active for the user"));

            Console.WriteLine(currentUser.Username + "wants to delete a subDirectory");
            FolderSynchServer.Instance.deleteSubDirectory(currentUser, transaction, baseFolder, localPath);
            Console.WriteLine("Delete subDirectory return");
            Console.WriteLine(" -------------------------------------------------------------------------- ");
        }


        /***************************************************************************************************/
        public Update updateCommit(UpdateTransaction transaction)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + "wants to commit an update");
            Update result = FolderSynchServer.Instance.updateCommit(transaction);
            ActiveUpdateTransactions.Remove(transaction.TransactionID);
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            return result;
        }


        /***************************************************************************************************/
        public void updateAbort(UpdateTransaction transaction)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + "must abort an update");
            FolderSynchServer.Instance.updateAbort(currentUser, transaction);
            ActiveUpdateTransactions.Remove(transaction.TransactionID);
            Console.WriteLine(" -------------------------------------------------------------------------- ");
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ FOLDERS --------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public void addNewSynchronizedFolder(Folder folder)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + " wants to add a new folder: " + folder.FolderName);
            FolderSynchServer.Instance.AddNewFolder(currentUser, folder);
            Console.WriteLine(" -------------------------------------------------------------------------- ");
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ HISTORY --------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public List<Update.UpdateEntry> getFileHistory(string baseFolder, string localPath)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + " wants file history: " + baseFolder + "\\" + localPath);
            return FolderSynchServer.Instance.getFileHistory(currentUser, baseFolder, localPath);
        }

        public List<Update> getHistory(string baseFolder)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + " wants history of: " + baseFolder);
            return FolderSynchServer.Instance.getHistory(currentUser, baseFolder);
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ DOWNLOAD -------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public byte[] downloadFile(string baseFolder, string localPath, int updateNumber)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + " wants to download: " + baseFolder + "\\" + localPath + "; (update " + updateNumber + ")");
            return FolderSynchServer.Instance.downloadFile(currentUser, baseFolder, localPath, updateNumber);
        }


        /***************************************************************************************************/
        public List<Update.UpdateEntry> getUpdateFileList(Update update)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + " wants the list of file relative to update " + update.Number);
            return FolderSynchServer.Instance.getUpdateFilesList(currentUser, update);
        }


        /***************************************************************************************************/
        public RollbackTransaction beginRollback(Update update, DateTime timestamp)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + " wants to begin a rollback");
            Console.WriteLine("Update: " + update.Number);
            Console.WriteLine("time: " + timestamp.ToString());

            RollbackTransaction transaction = new RollbackTransaction(currentUser, update.Number, update.BaseFolder, timestamp);
            FolderSynchServer.Instance.beginRollback(transaction);
            ActiveRollbackTransactions.Add(transaction.TransactionID, transaction);
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            return transaction;
        }


        /***************************************************************************************************/
        public void commitRollback(RollbackTransaction transaction)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + " wants to commit a rollback");

            RollbackTransaction tr = null;
            if (!ActiveRollbackTransactions.TryGetValue(transaction.TransactionID, out tr))
                throw new FaultException(new FaultReason("No active transaction for this rollback"));

            FolderSynchServer.Instance.commitRollback(tr);
            ActiveRollbackTransactions.Remove(tr.TransactionID);
            Console.WriteLine(" -------------------------------------------------------------------------- ");
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ OPTIONS --------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public void changeFolderOptions(string folderName, Folder updatedFolder)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + " wants to change options for: " + folderName);
            FolderSynchServer.Instance.changeFolderOptions(currentUser, folderName, updatedFolder);
            Console.WriteLine(" -------------------------------------------------------------------------- ");
        }

        /***************************************************************************************************/
        public void removeSynchronizedFolder(string folderName)
        {
            Console.WriteLine(" -------------------------------------------------------------------------- ");
            Console.WriteLine(currentUser.Username + " wants to remove a folder: " + folderName);
            FolderSynchServer.Instance.removeSynchronizedFolder(currentUser, folderName);
            Console.WriteLine(currentUser.Username + " succesfully removed the folder");
            Console.WriteLine(" -------------------------------------------------------------------------- ");
        }

        
    }
}
