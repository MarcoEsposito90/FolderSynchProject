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

        public UploadBackgroundWorker(string folderPath) : base()
        {
            this.folderPath = folderPath;
            DoWork += Upload_BackgroundWork;
        }


        private void Upload_BackgroundWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // 1) get references to service ----------------------------------------------
            App application = (App)Application.Current;
            FolderSynchServiceContractClient proxy = application.FolderSynchProxy;
            StreamedTransferContractClient streamProxy = application.StreamTransferProxy;

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

                    // 4) enumerating files and directiories ----------------------------
                    string[] files = Directory.GetFiles(folderPath);
                    int num = 0;

                    // 3) upload each file ----------------------------------------------
                    foreach (string file in files)
                    {

                        // ***** compute local path *****
                        string[] path = file.Split('\\');
                        string localPath = "";

                        for (int i = path.Length - 1; i >= 0; i--)
                        {
                            if (path[i].Equals(folderName))
                                break;

                            localPath += path[i] + localPath;
                        }

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

                        int progress = (int)(((float)++num / files.Length)*100);
                        Console.WriteLine("num = " + num + "; progress = " + progress);
                        worker.ReportProgress(progress);
                    }

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
