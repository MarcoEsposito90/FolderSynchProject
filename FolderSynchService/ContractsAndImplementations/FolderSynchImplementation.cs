﻿using System;
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
        Dictionary<string,UpdateTransaction> ActiveTransactions = null;

        /* ---------------------------------------------------------------------- */
        /* ------------------------ USER ---------------------------------------- */
        /* ---------------------------------------------------------------------- */

        public User loginUser(string username, string password)
        {
            currentUser = FolderSynchServer.Instance.LoginUser(username, password);
            ActiveTransactions = new Dictionary<string, UpdateTransaction>();

            Console.WriteLine(currentUser.Username + " wants to login");
            Console.WriteLine(currentUser.Username + " Folders list: ");
            foreach (Folder f in currentUser.Folders)
                Console.WriteLine(f.FolderName);

            return currentUser;
        }

        public void logoutUser(User user)
        {
            Console.WriteLine(user.Username + "is logging out");
            FolderSynchServer.Instance.logoutUser(user);
            currentUser = null;
        }

        public User RegisterNewUser(string username, string password)
        {
            currentUser = FolderSynchServer.Instance.registerNewUser(username, password);
            return currentUser;
        }



        /* ------------------------------------------------------------------------------- */
        /* ------------------------ UPDATES ---------------------------------------------- */
        /* ------------------------------------------------------------------------------- */

        public UpdateTransaction beginUpdate(string baseFolder, DateTime timestamp)
        {
            Console.WriteLine(currentUser.Username + "wants to start a transaction");
            Console.WriteLine("timestamp = " + timestamp.ToString());
            Console.WriteLine("baseFodler = " + baseFolder);
            UpdateTransaction tr = new UpdateTransaction(currentUser, baseFolder, timestamp);
            FolderSynchServer.Instance.beginUpdate(tr);
            ActiveTransactions.Add(tr.TransactionID,tr);
            return tr;
        }


        public void uploadFile(string transactionID, string baseFolder, string localPath, byte[] data)
        {
            UpdateTransaction transaction = null;
            

            if (!ActiveTransactions.TryGetValue(transactionID, out transaction))
                throw new FaultException(new FaultReason("This transaction is not active for the user"));

            Console.WriteLine(currentUser.Username + "wants to add a new file");
            FolderSynchServer.Instance.uploadFile(currentUser, transaction, baseFolder, localPath, data);
        }

        public void addSubDirectory(string transactionID, string baseFolder, string localPath)
        {
            UpdateTransaction transaction = null;

            if (!ActiveTransactions.TryGetValue(transactionID, out transaction))
                throw new FaultException(new FaultReason("This transaction is not active for the user"));

            Console.WriteLine(currentUser.Username + "wants to add a subDirectory");
            FolderSynchServer.Instance.addSubDirectory(currentUser, transaction, baseFolder, localPath);
        }


        public Update updateCommit(UpdateTransaction transaction)
        {
            Console.WriteLine(currentUser.Username + "wants to commit an update");
            Update result = FolderSynchServer.Instance.updateCommit(transaction);
            ActiveTransactions.Remove(transaction.TransactionID);
            return result;
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ FOLDERS --------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public void addNewSynchronizedFolder(Folder folder)
        {
            Console.WriteLine(currentUser.Username + " wants to add a new folder: " + folder.FolderName);
            FolderSynchServer.Instance.AddNewFolder(currentUser, folder);
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ HISTORY --------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public List<Update.UpdateEntry> getFileHistory(string baseFolder, string localPath)
        {
            Console.WriteLine(currentUser.Username + " wants file history: " + baseFolder + "\\" + localPath);
            return FolderSynchServer.Instance.getFileHistory(currentUser, baseFolder, localPath);
        }

        public List<Update> getHistory(string baseFolder)
        {
            Console.WriteLine(currentUser.Username + " wants history of: " + baseFolder);
            return FolderSynchServer.Instance.getHistory(currentUser, baseFolder);
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ DOWNLOAD -------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public byte[] downloadFile(string baseFolder, string localPath, int updateNumber)
        {
            Console.WriteLine(currentUser.Username + " wants to download: " + baseFolder + "\\" + localPath + "; (update " + updateNumber + ")");
            return FolderSynchServer.Instance.downloadFile(currentUser, baseFolder, localPath, updateNumber);
        }
    }
}
