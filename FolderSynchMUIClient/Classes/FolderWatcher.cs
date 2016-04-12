using ServicesProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FolderSynchMUIClient
{
    public class FolderWatcher
    {
        FileSystemWatcher watcher;
        Timer timer;

        LocalFolder localFolder;
        Folder folder;

        List<string> changedFiles;
        List<string> newFiles;
        List<string> deletedFiles;
        List<string> newSubDirectories;
        List<string> deletedSubDirectories;

        public FolderWatcher(Folder folder, LocalFolder localFolder)
        {
            // attribute setting ---------------------------------------
            Console.WriteLine("Watcher created for path " + localFolder.LocalPath);
            this.folder = folder;
            this.localFolder = localFolder;

            // data structure initialization ---------------------------
            changedFiles = new List<string>();
            newFiles = new List<string>();
            deletedFiles = new List<string>();
            newSubDirectories = new List<string>();
            deletedSubDirectories = new List<string>();

            // creating watcher ---------------------------------------
            watcher = new FileSystemWatcher(localFolder.LocalPath);
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter =  NotifyFilters.Attributes | 
                                    NotifyFilters.DirectoryName | 
                                    NotifyFilters.FileName | 
                                    NotifyFilters.LastWrite | 
                                    NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnChanged;
            watcher.EnableRaisingEvents = true;

            timer = null;
        }

        public void watch()
        {
            Console.WriteLine("Start watching");

            // setting timer
            timer = new Timer(performUpdate, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        public void stopWatching()
        {
            if (timer != null)
                timer.Dispose();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //capire qui come gestire la creazione degli update 
            
            WatcherChangeTypes wct = e.ChangeType;
            changedFiles.Add(e.FullPath);
            Console.WriteLine("File {0} {1}", e.FullPath, wct.ToString());
            Console.WriteLine("changedFiles size = {0}", changedFiles.Count);
        }

        private void performUpdate(Object state)
        {
            Console.WriteLine("proceeding with update at " + DateTime.Now);
        }

    }
}
