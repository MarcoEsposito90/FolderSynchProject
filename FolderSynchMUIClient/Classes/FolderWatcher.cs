using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient.Classes
{
    public class FolderWatcher
    {
        FileSystemWatcher watcher;

        public void watch(string folderPath)
        {
            Console.WriteLine("Watcher created for path " + folderPath);
            watcher = new FileSystemWatcher(folderPath);
            //watcher.Path = folderPath;
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;
            watcher.Changed += OnChanged;
            watcher.Renamed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //capire qui come gestire la creazione degli update 

            Console.WriteLine("OnChanged method called");
            WatcherChangeTypes wct = e.ChangeType;
            Console.WriteLine("File {0} {1}", e.FullPath, wct.ToString());
        }

    }
}
