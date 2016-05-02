using FolderSynchMUIClient.FolderSynchService;
using FolderSynchMUIClient.StreamedTransferService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FolderSynchMUIClient.Classes
{
    public class UploadBackgroundWorker : BackgroundWorker
    {
        public enum State
        {
            Computing,
            Uploading
        }


        private LocalFolder LocalFolder;
        private List<Item.Change> Changes;

        private int filesNumber;
        private int filesUploaded;
        private App application;
        private FolderSynchServiceContractClient proxy;
        private StreamedTransferContractClient streamProxy;


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ CONTRUCTORS ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        // whole folder uploader
        public UploadBackgroundWorker(LocalFolder localFolder) : base()
        {
            this.LocalFolder = localFolder;
            Changes = null;

            filesNumber = 0;
            filesUploaded = 0;

            DoWork += Upload_BackgroundWork;

            application = (App)Application.Current;
            proxy = application.FolderSynchProxy;
            streamProxy = application.StreamTransferProxy;
        }


        // incremental uploader
        public UploadBackgroundWorker(LocalFolder localFolder, List<Item.Change> changes) : base()
        {
            LocalFolder = localFolder;
            Changes = changes;
            Changes.Sort();

            filesNumber = 0;
            filesUploaded = 0;

            DoWork += Upload_BackgroundWork_Incremental;

            application = (App)Application.Current;
            proxy = application.FolderSynchProxy;
            streamProxy = application.StreamTransferProxy;
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ DO WORK --------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        /*********************************************************************************/
        private void Upload_BackgroundWork(object sender, DoWorkEventArgs e)
        {
            // 1) temporary stop watching the folder -------------------------------
            application.stopWatching(LocalFolder);

            BackgroundWorker worker = sender as BackgroundWorker;
            worker.ReportProgress(0, State.Computing);
            Update newUpdate = null;


            // 2) compute size -----------------------------------------------------
            filesNumber = computeFileNumber(LocalFolder);
            worker.ReportProgress(0, State.Uploading);

            try
            {
                if (application.User != null)
                {
                    // 3) begin update transaction --------------------------------------
                    DateTime timestamp = DateTime.Now;
                    UpdateTransaction transaction = proxy.beginUpdate(LocalFolder.Name, timestamp);

                    uploadDirectory(LocalFolder, transaction, worker);
                    newUpdate = proxy.updateCommit(transaction);

                    e.Result = new UploadWorkerResponse(true, "");
                    LocalFolder.LastUpdateCheck = DateTime.Now;
                    LocalFolder.Updates.Add(newUpdate);
                    LocalFolder.setLatestUpdateItems();
                }
                else
                {
                    e.Result = new UploadWorkerResponse(false, UploadWorkerResponse.USER_NOT_CONNECTED);
                }

            }
            catch (FaultException f)
            {
                Console.WriteLine("error: " + f.Reason);
                e.Result = new UploadWorkerResponse(false, UploadWorkerResponse.ERROR_MESSAGE);

            }
            catch (TimeoutException exception)
            {
                Console.WriteLine("error: " + exception.Message);
                e.Result = new UploadWorkerResponse(false, UploadWorkerResponse.ERROR_MESSAGE);
            }

            application.startWatching(LocalFolder);
        }



        /***********************************************************************************************/
        private void uploadDirectory(FolderItem folder, UpdateTransaction transaction, BackgroundWorker worker)
        {
            Console.WriteLine("Uploading directory: " + folder.Path);
            if (!folder.Path.Equals(LocalFolder.Path))
            {
                string l = folder.Path.Replace(LocalFolder.Path + "\\", "");
                Console.WriteLine("creating subdir: " + l);
                proxy.addSubDirectory(transaction.TransactionID, LocalFolder.Name, l);
            }


            List<FolderItem> subDirectories = folder.LatestUpdateFolderItems;
            List<FileItem> files = folder.LatestUpdateFileItems;

            foreach (FileItem file in files)
            {
                Console.WriteLine("uploading " + file);

                // ***** compute local path *****
                string localPath = file.Path.Replace(LocalFolder.Path + "\\", "");

                // ***** start reading file *****
                using (Stream uploadStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
                {
                    FileInfo fi = new FileInfo(file.Path);

                    if (fi.Length > App.MAX_BUFFERED_TRANSFER_FILE_SIZE)
                    {
                        // ***** streamed transfer *****
                        streamProxy.uploadFileStreamed(LocalFolder.Name,
                                                        localPath,
                                                        transaction.TransactionID,
                                                        Item.Change.NEW_FILE,
                                                        application.User.Username,
                                                        uploadStream);

                    }
                    else
                    {
                        // ***** one-time transfer (buffered) *****
                        byte[] buffer = new byte[fi.Length];
                        uploadStream.Read(buffer, 0, buffer.Length);
                        proxy.uploadFile(transaction.TransactionID, LocalFolder.Name, localPath, Item.Change.NEW_FILE, buffer);
                    }
                }

                // ***** report progress to main thread *****

                int percentage = (int)(((float)++filesUploaded / filesNumber) * 100);
                worker.ReportProgress(percentage, State.Uploading);

            }

            foreach (FolderItem d in subDirectories)
                uploadDirectory(d, transaction, worker);

        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ DO WORK INCREMENTAL --------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void Upload_BackgroundWork_Incremental(object sender, DoWorkEventArgs e)
        {
            // 1) stop watching the folder -----------------------------------------------
            application.stopWatching(LocalFolder);

            if (application.User == null)
            {
                e.Result = new UploadWorkerResponse(false, UploadWorkerResponse.USER_NOT_CONNECTED);
                return;
            }

            // 2) preparation ------------------------------------------------------------
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.ReportProgress(0, State.Computing);
            Update newUpdate = null;
            filesNumber = Changes.Count;
            worker.ReportProgress(0, State.Uploading);

            try
            {
                // 3) begin update transaction --------------------------------------
                DateTime timestamp = DateTime.Now;
                UpdateTransaction transaction = proxy.beginUpdate(LocalFolder.Name, timestamp);

                // 4) upload items --------------------------------------------------
                foreach (Item.Change change in Changes)
                {
                    uploadItem(change, transaction);
                    int percentage = (int)(((float)++filesUploaded / filesNumber) * 100);
                    worker.ReportProgress(percentage, State.Uploading);
                }

                // 5) saving result
                newUpdate = proxy.updateCommit(transaction);
                e.Result = new UploadWorkerResponse(true, "");
                LocalFolder.LastUpdateCheck = DateTime.Now;
                LocalFolder.Updates = new ObservableCollection<Update>(proxy.getHistory(LocalFolder.Name));
                LocalFolder.setLatestUpdateItems();
            }
            catch (FaultException f)
            {
                Console.WriteLine("error: " + f.Message);
                e.Result = new UploadWorkerResponse(false, UploadWorkerResponse.ERROR_MESSAGE);
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine("error: " + exception.Message);
                e.Result = new UploadWorkerResponse(false, UploadWorkerResponse.ERROR_MESSAGE);
            }

            application.startWatching(LocalFolder);
        }


        /***********************************************************************************/
        private void uploadItem(Item.Change change, UpdateTransaction transaction)
        {
            Console.WriteLine("proceed to upload: " + change.Path);

            string localPath = change.Path.Replace(LocalFolder.Path + "\\", "");

            if (change.Type == Item.Change.NEW_DIRECTORY)
                proxy.addSubDirectory(transaction.TransactionID, LocalFolder.Name, localPath);

            else if (change.Type == Item.Change.DELETED_DIRECTORY)
                proxy.deleteSubDirectory(transaction.TransactionID, LocalFolder.Name, localPath);

            else if (change.Type == Item.Change.DELETED_FILE)
                proxy.deleteFile(transaction.TransactionID, LocalFolder.Name, localPath);

            else
            {
                
                using (Stream uploadStream = new FileStream(change.Path, FileMode.Open, FileAccess.Read))
                {
                    FileInfo fi = new FileInfo(change.Path);

                    if (fi.Length > App.MAX_BUFFERED_TRANSFER_FILE_SIZE)
                    {
                        // ***** streamed transfer *****
                        streamProxy.uploadFileStreamed(LocalFolder.Name,
                                                        localPath,
                                                        transaction.TransactionID,
                                                        change.Type,
                                                        application.User.Username,
                                                        uploadStream);

                    }
                    else
                    {
                        // ***** one-time transfer (buffered) *****
                        byte[] buffer = new byte[fi.Length];
                        Console.WriteLine("buf len = " + buffer.Length);
                        uploadStream.Read(buffer, 0, buffer.Length);
                        proxy.uploadFile(transaction.TransactionID, LocalFolder.Name, localPath, change.Type, buffer);
                    }
                }
            }
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ SIZE COMPUTING -------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private int computeFileNumber(FolderItem folder)
        {
            int num = 0;

            List<FolderItem> subDirectories = folder.LatestUpdateFolderItems;
            List<FileItem> files = folder.LatestUpdateFileItems;

            num += files.Count;
            foreach (FolderItem dir in subDirectories)
                num += computeFileNumber(dir);

            return num;
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ RESPONSE OBJECT ------------------------------------- */
        /* ------------------------------------------------------------------------------ */


        public class UploadWorkerResponse
        {

            public static readonly string ERROR_MESSAGE = "something went wrong";
            public static readonly string USER_NOT_CONNECTED = "cannot upload if user is not connected";

            public bool Success
            {
                get;
                private set;
            }

            public String ErrorMessage
            {
                get;
                private set;
            }


            public UploadWorkerResponse(bool success, string errorMessage)
            {
                Success = success;
                ErrorMessage = errorMessage;
            }
        }
    }
}
