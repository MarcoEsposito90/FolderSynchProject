using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using FolderSynchMUIClient.FolderSynchService;

namespace FolderSynchMUIClient
{
    public class FolderWatcher
    {
        Timer timer;

        LocalFolder localFolder;
        Folder folder;

        public FolderWatcher(Folder folder, LocalFolder localFolder)
        {
            // attribute setting ---------------------------------------
            Console.WriteLine("Watcher created for path " + localFolder.Path);
            this.folder = folder;
            this.localFolder = localFolder;

            timer = null;
        }

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
                                TimeSpan.FromMinutes(1));                                           // second is for subsequent
        }

        public void stopWatching()
        {
            if (timer != null)
                timer.Dispose();
        }


        private void performUpdate(Object state)
        {
            Console.WriteLine("proceeding with update at " + DateTime.Now);
            List<Item.Change> changes = localFolder.DetectChanges(localFolder.LastUpdate.Timestamp);

            foreach(Item.Change c in changes)
                Console.WriteLine("cahnged: " + c.Path + " (" + c.Type + ")");

            localFolder.setLatestUpdateItems();
        }

    }
}
