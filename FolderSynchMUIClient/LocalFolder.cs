using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{
    public class LocalFolder
    {

        public string Username
        {
            get;
            private set;
        }

        public string FolderName
        {
            get;
            private set;
        }

        public string LocalPath
        {
            get;
            private set;
        }

        public LocalFolder(string username, string folderName, string localPath)
        {

            Username = username;
            FolderName = folderName;
            LocalPath = localPath;
        }
    }
}
