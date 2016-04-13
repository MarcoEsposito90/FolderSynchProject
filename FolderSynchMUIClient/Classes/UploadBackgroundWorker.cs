using FolderSynchMUIClient.FolderSynchService;
using FolderSynchMUIClient.StreamedTransferService;
using System;
using System.Collections.Generic;
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
        private int filesNumber;
        private int filesUploaded;
        private App application;
        private FolderSynchServiceContractClient proxy;
        private StreamedTransferContractClient streamProxy;

        public UploadBackgroundWorker(LocalFolder localFolder) : base()
        {
            this.LocalFolder = localFolder;
            filesNumber = 0;
            filesUploaded = 0;

            DoWork += Upload_BackgroundWork;

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
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.ReportProgress(0, State.Computing);
            Update newUpdate = null;

            // 2) compute size -----------------------------------------------------
            filesNumber = computeSize(LocalFolder.Path);
            worker.ReportProgress(0, State.Uploading);

            try
            {
                if (application.User != null)
                {
                    // 3) begin update transaction --------------------------------------
                    DateTime timestamp = DateTime.Now;
                    UpdateTransaction transaction = proxy.beginUpdate(LocalFolder.Name, timestamp);

                    uploadDirectory(LocalFolder.Path, transaction, worker);
                    newUpdate = proxy.updateCommit(transaction);
                }

            }
            catch (FaultException f)
            {
                Console.WriteLine("error: " + f.Reason);
                e.Result = new UploadWorkerResponse(false, UploadWorkerResponse.ERROR_MESSAGE);
            }

            e.Result = new UploadWorkerResponse(true, "");
            LocalFolder.LastUpdate = newUpdate;
            LocalFolder.setLatestUpdateItems();
        }



        /***********************************************************************************************/
        private void uploadDirectory(string path, UpdateTransaction transaction, BackgroundWorker worker)
        {
            Console.WriteLine("Uploading directory: " + path);
            if (!path.Equals(LocalFolder.Path))
            {
                string l = path.Replace(LocalFolder.Path + "\\", "");
                Console.WriteLine("creating subdir: " + l);
                proxy.addSubDirectory(transaction.TransactionID, LocalFolder.Name, l);
            }


            List<string> subDirectories = new List<string>(Directory.EnumerateDirectories(path));
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                Console.WriteLine("uploading " + file);

                // ***** compute local path *****
                string localPath = file.Replace(LocalFolder.Path + "\\", "");
                
                // ***** start reading file *****
                using (Stream uploadStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    FileInfo fi = new FileInfo(file);

                    if (fi.Length > App.MAX_BUFFERED_TRANSFER_FILE_SIZE)
                    {
                        // ***** streamed transfer *****
                        streamProxy.uploadFileStreamed(LocalFolder.Name, localPath, transaction.TransactionID, application.User.Username, uploadStream);
                    }
                    else
                    {
                        // ***** one-time transfer (buffered) *****
                        byte[] buffer = new byte[App.MAX_BUFFERED_TRANSFER_FILE_SIZE];
                        uploadStream.Read(buffer, 0, App.MAX_BUFFERED_TRANSFER_FILE_SIZE);
                        proxy.uploadFile(transaction.TransactionID, LocalFolder.Name, localPath, buffer);
                    }
                }

                // ***** report progress to main thread *****

                int percentage = (int)(((float)++filesUploaded / filesNumber) * 100);
                worker.ReportProgress(percentage, State.Uploading);
                
            }

            foreach (string d in subDirectories)
                uploadDirectory(d, transaction, worker);

        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ SIZE COMPUTING -------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private int computeSize(string path)
        {
            int num = 0;

            List<string> subDirectories = new List<string>(Directory.EnumerateDirectories(path));
            string[] files = Directory.GetFiles(path);

            num += files.Length;
            foreach (string dir in subDirectories)
                num += computeSize(dir);

            return num;
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ RESPONSE OBJECT ------------------------------------- */
        /* ------------------------------------------------------------------------------ */


        public class UploadWorkerResponse
        {

            public static readonly string ERROR_MESSAGE = "something went wrong";

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
