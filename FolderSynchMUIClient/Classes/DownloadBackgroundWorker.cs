using FolderSynchMUIClient.FolderSynchService;
using FolderSynchMUIClient.StreamedTransferService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FolderSynchMUIClient
{
    class DownloadBackgroundWorker : BackgroundWorker
    {

        public enum State
        {
            Preparing,
            Downloading
        }


        private LocalFolder LocalFolder;
        private Update.UpdateEntry Entry;
        private Update Update;

        private int filesNumber;
        private int filesDownloaded;
        private string DownloadFileName;
        private string DownloadFolderName;

        private App application;
        private FolderSynchServiceContractClient proxy;
        private StreamedTransferContractClient streamProxy;


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ CONTRUCTORS ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        // single file downloader
        public DownloadBackgroundWorker(LocalFolder localFolder, Update.UpdateEntry entry, string downloadFileName) : base()
        {
            this.LocalFolder = localFolder;
            this.Entry = entry;
            this.DownloadFileName = downloadFileName;

            filesNumber = 0;
            filesDownloaded = 0;

            DoWork += Download_BackgroundWork_File;

            application = (App)Application.Current;
            proxy = application.FolderSynchProxy;
            streamProxy = application.StreamTransferProxy;
        }


        // incremental uploader
        public DownloadBackgroundWorker(LocalFolder localFolder, Update update, string downloadFolderName) : base()
        {
            LocalFolder = localFolder;
            DownloadFolderName = downloadFolderName;

            filesNumber = 0;
            filesDownloaded = 0;

            //DoWork += ;

            application = (App)Application.Current;
            proxy = application.FolderSynchProxy;
            streamProxy = application.StreamTransferProxy;
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ DO WORK --------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        /*********************************************************************************/
        private void Download_BackgroundWork_File(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.ReportProgress(0, State.Preparing);

            try
            {
                if (application.User != null)
                {

                    long size = Entry.ItemDimension;
                    if(size > App.MAX_BUFFERED_TRANSFER_FILE_SIZE)
                    {
                        // streamed download --------------------------------------------------------------------
                        Stream stream = streamProxy.downloadFileStreamed(   application.User.Username, 
                                                                            LocalFolder.Name, 
                                                                            Entry.ItemLocalPath, 
                                                                            Entry.UpdateNumber);

                        FileStream fs = new FileStream(DownloadFileName, FileMode.OpenOrCreate, FileAccess.Write);

                        using (fs)
                        {
                            int bufferSize = 10240;
                            byte[] buffer = new byte[bufferSize];
                            int bytesRead;

                            while ((bytesRead = stream.Read(buffer, 0, bufferSize)) > 0 ){

                                fs.Write(buffer, 0, bytesRead);
                            }

                            fs.Close();
                            stream.Close();
                        }
                    }
                    else
                    {
                        // buffered download -----------------------------------------------------------------------
                        byte[] fileBuffer = proxy.downloadFile(LocalFolder.Name, Entry.ItemLocalPath, Entry.UpdateNumber);
                        using (FileStream fs = new FileStream(DownloadFileName, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            fs.Write(fileBuffer, 0, fileBuffer.Length);
                            fs.Close();
                        }
                    }

                    e.Result = new DownloadWorkerResponse(true, "");
                }
                else
                {
                    e.Result = new DownloadWorkerResponse(false, DownloadWorkerResponse.USER_NOT_CONNECTED);
                }

            }
            catch (FaultException f)
            {
                Console.WriteLine("error: " + f.Reason);
                e.Result = new DownloadWorkerResponse(false, DownloadWorkerResponse.ERROR_MESSAGE);
            }

        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ RESPONSE OBJECT ------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public class DownloadWorkerResponse
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


            public DownloadWorkerResponse(bool success, string errorMessage)
            {
                Success = success;
                ErrorMessage = errorMessage;
            }
        }
    }
}
