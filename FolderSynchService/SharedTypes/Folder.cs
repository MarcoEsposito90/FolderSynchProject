using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [DataContract]
    public class Folder
    {

        public static readonly int DEFAULT_REFRESH_TIME_HOURS = 24;
        public static readonly int DEFAULT_DELETE_TIME_DAYS = 14;

        /* -------------- PROPERTIES -------------------------*/

        [DataMember]
        public string FolderName { get; set; }

        [DataMember]
        public string Username { get; private set; }

        [DataMember]
        public DateTime SynchDate { get; set; }

        [DataMember]
        public int AutoRefreshTime { get; set; }

        [DataMember]
        public int AutoDeleteTime { get; set; }


        /* -------------- CONSTRUCTORS -------------------------*/

        public Folder(string Name, string Username)
        {
            this.FolderName = Name;
            this.Username = Username;
        }


    }
}
