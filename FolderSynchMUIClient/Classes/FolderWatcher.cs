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
        List<string> changedFiles;

        public void watch(string folderPath)
        {
            Console.WriteLine("Watcher created for path " + folderPath);
            changedFiles = new List<string>();
            watcher = new FileSystemWatcher(folderPath);
            watcher.IncludeSubdirectories = true;
            //watcher.Path = folderPath;
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnChanged;
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //capire qui come gestire la creazione degli update 
            
            WatcherChangeTypes wct = e.ChangeType;
            changedFiles.Add(e.FullPath);
            Console.WriteLine("File {0} {1}", e.FullPath, wct.ToString());
            Console.WriteLine("changedFiles size = {0}", changedFiles.Count);
        }

    }
}
