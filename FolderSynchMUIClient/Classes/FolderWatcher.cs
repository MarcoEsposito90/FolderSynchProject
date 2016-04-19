using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using FolderSynchMUIClient.FolderSynchService;
using FolderSynchMUIClient.Classes;
using System.ComponentModel;

namespace FolderSynchMUIClient
{
    public class FolderWatcher
    {
        Timer timer;

        public LocalFolder LocalFolder
        {
            get;
            private set;
        }

        public bool IsWatching
        {
            get;
            private set;
        }

        Folder folder;


        public FolderWatcher(Folder folder, LocalFolder localFolder)
        {
            Console.WriteLine("Watcher created for path " + localFolder.Path);
            this.folder = folder;
            this.LocalFolder = localFolder;
            timer = null;
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ TIMER MANAGEMENT ------------------------------------------ */
        /* ------------------------------------------------------------------------------ */

        public void watch()
        {
            Console.WriteLine("Start watching");
            IsWatching = true;

            // 1)   must detect if update is immediately necessary (for instance,
            //      the client may have been closed for days)
            double minutes = (DateTime.Now - LocalFolder.LastUpdate.Timestamp).TotalMinutes;
            Console.WriteLine("folder: " + LocalFolder.Name + " is not being updated for " + minutes + " minutes");
            

            // 2) setting timer
            timer = new Timer(  performUpdate, 
                                null, 
                                minutes > 1 ? TimeSpan.FromSeconds(2) : TimeSpan.FromMinutes(1),    // first time span is for next fire
                                TimeSpan.FromMinutes(1));                                           // second is for subsequent
        }

        public void stopWatching()
        {
            if (timer != null)
                timer.Dispose();
            timer = null;

            IsWatching = false;
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ TIMER CALLBACK -------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void performUpdate(Object state)
        {
            Console.WriteLine("proceeding with update at " + DateTime.Now);
            UploadBackgroundWorker uploader = null;

            if (LocalFolder.LastUpdate != null)
            {
                List<Item.Change> changes = LocalFolder.DetectChanges(LocalFolder.LastUpdate.Timestamp);

                if(changes.Count > 0)
                {
                    // we do not upload the whole folder, but just what's changed
                    uploader = new UploadBackgroundWorker(LocalFolder, changes);
                    uploader.RunWorkerAsync();
                }
            }
            else
            {
                // no previous update available. it is the first update or something went wrong in the past
                // for safety, we proceed uploading the whole folder
                uploader = new UploadBackgroundWorker(LocalFolder);
                uploader.RunWorkerAsync();
            }

            if (uploader != null)
            {
                uploader.WorkerSupportsCancellation = false;
                uploader.WorkerReportsProgress = true;
                uploader.ProgressChanged += UploadWork_ProgressChanged;
                uploader.RunWorkerCompleted += UploadWork_Completed;
            }
            else
                Console.WriteLine("no need for new updates");
            
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------ UPLOAD PROGRESS ------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void UploadWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("upload progress: " + e.ProgressPercentage);
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------ UPLOAD COMPLETE ------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void UploadWork_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            UploadBackgroundWorker.UploadWorkerResponse response = (UploadBackgroundWorker.UploadWorkerResponse)e.Result;
            Console.WriteLine(  "upload terminated with " +
                                (response.Success ? "success" : "error"));
        }

    }
}
