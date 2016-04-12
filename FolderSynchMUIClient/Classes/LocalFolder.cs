using ServicesProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{
    [DataContract]
    public class LocalFolder
    {

        [DataMember]
        public string Username
        {
            get;
            private set;
        }

        [DataMember]
        public string FolderName
        {
            get;
            private set;
        }

        [DataMember]
        public string LocalPath
        {
            get;
            private set;
        }

        [DataMember]
        public Update LastUpdate
        {
            get;
            set;
        }

        public LocalFolder(string username, string folderName, string localPath)
        {

            Username = username;
            FolderName = folderName;
            LocalPath = localPath;
        }
    }
}
