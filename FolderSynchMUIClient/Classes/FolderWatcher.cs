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

        LocalFolder localFolder;
        Folder folder;


        public FolderWatcher(Folder folder, LocalFolder localFolder)
        {
            Console.WriteLine("Watcher created for path " + localFolder.Path);
            this.folder = folder;
            this.localFolder = localFolder;
            timer = null;
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ TIMER MANAGEMENT ------------------------------------------ */
        /* ------------------------------------------------------------------------------ */

        public void watch()
        {
            Console.WriteLine("Start watching");

            // 1)   must detect if update is immediately necessary (for instance,
            //      the client may have been closed for days)
            double minutes = (DateTime.Now - localFolder.LastUpdate.Timestamp).TotalMinutes;
            Console.WriteLine("folder: " + localFolder.Name + " is not being updated for " + minutes + " minutes");
            

            // 2) setting timer
            timer = new Timer(  performUpdate, 
                                null, 
                                minutes > 1 ? TimeSpan.FromSeconds(2) : TimeSpan.FromSeconds(10),    // first time span is for next fire
                                TimeSpan.FromSeconds(20));                                           // second is for subsequent
        }

        public void stopWatching()
        {
            if (timer != null)
                timer.Dispose();
            timer = null;
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ TIMER CALLBACK -------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void performUpdate(Object state)
        {
            Console.WriteLine("proceeding with update at " + DateTime.Now);
            UploadBackgroundWorker uploader = null;

            if (localFolder.LastUpdate != null)
            {
                List<Item.Change> changes = localFolder.DetectChanges(localFolder.LastUpdate.Timestamp);

                Console.WriteLine("detected " + changes.Count + " changes");
                foreach (Item.Change c in changes)
                    Console.WriteLine("changed: " + c.Path + " (" + c.Type + ")");

                if(changes.Count > 0)
                {
                    // we do not upload the whole folder, but just what's changed
                    uploader = new UploadBackgroundWorker(localFolder, changes);
                    uploader.RunWorkerAsync();
                }
            }
            else
            {
                // no previous update available. it is the first update or something went wrong in the past
                // for safety, we proceed uploading the whole folder
                uploader = new UploadBackgroundWorker(localFolder);
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
