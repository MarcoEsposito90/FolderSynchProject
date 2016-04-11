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
using ServicesProject;

namespace FolderSynchMUIClient.Classes
{
    public class UploadBackgroundWorker : BackgroundWorker
    {

        private string folderPath;
        private string folderName;
        private App application;
        private FolderSynchServiceContractClient proxy;
        private StreamedTransferContractClient streamProxy;

        public UploadBackgroundWorker(string folderPath) : base()
        {
            this.folderPath = folderPath;
            string[] directories = folderPath.Split('\\');
            this.folderName = directories[directories.Length - 1];

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

            // 2) compute local path -----------------------------------------------------
            string[] directories = folderPath.Split('\\');
            string folderName = directories[directories.Length - 1];

            try
            {
                if (application.User != null)
                {
                    // 3) begin update transaction --------------------------------------
                    DateTime timestamp = DateTime.Now;
                    UpdateTransaction transaction = proxy.beginUpdate(folderName, timestamp);

                    uploadDirectory(folderPath, transaction, worker);

                    proxy.updateCommit(transaction);
                }

            }
            catch (FaultException f)
            {
                Console.WriteLine("error: " + f.Reason);
                e.Result = new UploadWorkerResponse(false, UploadWorkerResponse.ERROR_MESSAGE);
            }

            e.Result = new UploadWorkerResponse(true, "");
        }



        /***********************************************************************************************/
        private void uploadDirectory(string path, UpdateTransaction transaction, BackgroundWorker worker)
        {
            Console.WriteLine("Uploading directory: " + path);
            if (!path.Equals(folderPath))
            {
                string l = path.Replace(folderPath + "\\", "");
                Console.WriteLine("creating subdir: " + l);
                proxy.addSubDirectory(transaction.TransactionID, folderName, l);
            }


            List<string> subDirectories = new List<string>(Directory.EnumerateDirectories(path));
            string[] files = Directory.GetFiles(path);
            int num = 0;

            foreach (string file in files)
            {
                Console.WriteLine("uploading " + file);

                // ***** compute local path *****
                string localPath = file.Replace(folderPath + "\\", "");
                
                // ***** start reading file *****
                using (Stream uploadStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    FileInfo fi = new FileInfo(file);

                    if (fi.Length > App.MAX_BUFFERED_TRANSFER_FILE_SIZE)
                    {
                        // ***** streamed transfer *****
                        streamProxy.uploadFileStreamed(folderName, localPath, transaction.TransactionID, application.User.Username, uploadStream);
                    }
                    else
                    {
                        // ***** one-time transfer (buffered) *****
                        byte[] buffer = new byte[App.MAX_BUFFERED_TRANSFER_FILE_SIZE];
                        uploadStream.Read(buffer, 0, App.MAX_BUFFERED_TRANSFER_FILE_SIZE);
                        proxy.uploadFile(transaction.TransactionID, folderName, localPath, buffer);
                    }
                }

                // ***** report progress to main thread *****

                int progress = (int)(((float)++num / files.Length) * 100);
                Console.WriteLine("num = " + num + "; progress = " + progress);
                worker.ReportProgress(progress);
            }

            foreach (string d in subDirectories)
                uploadDirectory(d, transaction, worker);

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
