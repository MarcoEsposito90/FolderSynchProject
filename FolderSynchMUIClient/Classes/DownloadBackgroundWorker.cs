﻿using FolderSynchMUIClient.FolderSynchService;
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
        bool DeleteCurrent;

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


        // whole folder downloader
        public DownloadBackgroundWorker(LocalFolder localFolder, Update update, string downloadFolderName, bool deleteCurrent) : base()
        {
            LocalFolder = localFolder;
            DownloadFolderName = downloadFolderName;
            Update = update;
            DeleteCurrent = deleteCurrent;

            filesNumber = 0;
            filesDownloaded = 0;

            DoWork += Download_BackgroundWork_Folder;

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

                    downloadFile(Entry, DownloadFileName);
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



        /*********************************************************************************/
        private void Download_BackgroundWork_Folder(object sender, DoWorkEventArgs e)
        {
            DateTime timestamp = DateTime.Now;
            string tempDir = null;
            RollbackTransaction transaction = null;
            application.stopWatching(LocalFolder);

            try
            {
                if(application.User != null)
                {
                    
                    // start transaction and move directory ------------------------------------------------
                    if (DeleteCurrent || DownloadFolderName.Equals(LocalFolder.Path))
                    {
                        transaction = proxy.beginRollback(Update, timestamp);
                        tempDir = moveDirectory(LocalFolder.Path);
                        Directory.CreateDirectory(LocalFolder.Path);
                    }

                    // downloading files -------------------------------------------------------------------
                    List<Update.UpdateEntry> entries = new List<Update.UpdateEntry>(proxy.getUpdateFileList(Update));

                    for (int i = 0; i < entries.Count; i++)
                    {
                        Update.UpdateEntry entry = entries.ElementAt(i);
                        Console.WriteLine("entry" + i + ": " + entry.ItemLocalPath + "; from update " + entry.UpdateNumber);
                        if (entry.UpdateType == Item.Change.NEW_DIRECTORY)
                        {
                            Directory.CreateDirectory(DownloadFolderName + "\\" + entry.ItemLocalPath);
                            continue;
                        }

                        downloadFile(entry, DownloadFolderName + "\\" + entry.ItemLocalPath);
                    }

                    // everything ok. commit and delete folder if necessary ----------------------------------
                    if (DeleteCurrent || DownloadFolderName.Equals(LocalFolder.Path))
                        proxy.commitRollback(transaction);

                    if (DeleteCurrent)
                        deleteFolder(tempDir);

                    // save new state
                    LocalFolder.setLatestUpdateItems();
                    LocalFolder.LastUpdate = Update;

                    e.Result = new DownloadWorkerResponse(true, "");
                }
                else
                {
                    Console.WriteLine("null user! cannot download folder");
                    e.Result = new DownloadWorkerResponse(false, "You are not connected!");
                }
            }
            catch(FaultException f)
            {
                Console.WriteLine("error: " + f.Message);
                e.Result = new DownloadWorkerResponse(false, f.Message);

                // delete what has been copied so far
                deleteFolder(DownloadFolderName);

                if (DeleteCurrent || DownloadFolderName.Equals(LocalFolder.Path))
                {
                    // return to current directory
                    Console.WriteLine("moving " + tempDir + " to " + LocalFolder.Path);
                    Directory.Move(tempDir, LocalFolder.Path);
                }
            }

            application.startWaching(LocalFolder);
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------------ AUXILIARY ------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void downloadFile(Update.UpdateEntry entry, string path)
        {
            long size = entry.ItemDimension;
            if (size > App.MAX_BUFFERED_TRANSFER_FILE_SIZE)
            {
                // streamed download --------------------------------------------------------------------
                Stream stream = streamProxy.downloadFileStreamed(application.User.Username,
                                                                    LocalFolder.Name,
                                                                    entry.ItemLocalPath,
                                                                    entry.UpdateNumber);

                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

                using (fs)
                {
                    int bufferSize = 10240;
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead;

                    while ((bytesRead = stream.Read(buffer, 0, bufferSize)) > 0)
                    {

                        fs.Write(buffer, 0, bytesRead);
                    }

                    fs.Close();
                    stream.Close();
                }
            }
            else
            {
                // buffered download -----------------------------------------------------------------------
                byte[] fileBuffer = proxy.downloadFile(LocalFolder.Name, entry.ItemLocalPath, entry.UpdateNumber);
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(fileBuffer, 0, fileBuffer.Length);
                    fs.Close();
                }
            }

        }


        /*********************************************************************************/
        private void deleteFolder(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string f in files)
                File.Delete(f);

            string[] subFolders = Directory.GetDirectories(path);
            foreach(string f in subFolders)
            {
                deleteFolder(f);
            }

            Directory.Delete(path);
        }


        /*********************************************************************************/
        private string moveDirectory(string path)
        {
            // do not delete folder, just temporary rename it
            int counter = 0;
            string newPath;

            while (true)
            {
                newPath = path + "_temp" + counter;

                if (!Directory.Exists(newPath))
                    break;
                counter++;
            }

            Directory.Move(path, newPath);
            return newPath;
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
