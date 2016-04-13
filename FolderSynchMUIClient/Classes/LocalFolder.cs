using FolderSynchMUIClient.FolderSynchService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace FolderSynchMUIClient
{
    [DataContract]
    public class LocalFolder : FolderItem
    {
        /* ---------------------------------------------------------------- */
        /* ------------ SERIALIZABLE PROPERTIES --------------------------- */
        /* ---------------------------------------------------------------- */

        [DataMember]
        public string Username { get; private set; }
        
        private Update _LatestUpdate;

        [DataMember]
        public Update LastUpdate {
            get
            {
                return _LatestUpdate;
            }
            set
            {
                _LatestUpdate = value;
                setLatestUpdateItems();
            }
        }

        [DataMember]
        public int AutoRefreshTime { get; set; }

        [DataMember]
        public int AutoDeleteTime { get; set; }

        [DataMember]
        public DateTime SynchDate { get; set; }


        /* ---------------------------------------------------------------- */
        /* ------------ READONLY PROPERTIES ------------------------------- */
        /* ---------------------------------------------------------------- */

        public ObservableCollection<Update> Updates
        {
            get;
            set;
        }

        public int ContainedFiles
        {
            get
            {
                return Directory.GetFiles(this.Path, "*", SearchOption.AllDirectories).Length;
            }
        }

        public int ContainedFolders
        {
            get
            {
                return Directory.GetDirectories(this.Path, "*", SearchOption.AllDirectories).Length;
            }
        }
        

        /* ---------------------------------------------------------------- */
        /* ------------ CONSTRUCTORS -------------------------------------- */
        /* ---------------------------------------------------------------- */

        public LocalFolder(string username, string folderName, string localPath) : base(folderName, localPath)
        {
            this.Updates = new ObservableCollection<Update>();
            this.Username = username;
        }

        
        /* ---------------------------------------------------------------- */
        /* ------------ OVERRIDE METHODS ---------------------------------- */
        /* ---------------------------------------------------------------- */

        public override long CalculateSize()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Path);

            long size = 0;

            foreach (Item i in Items)
                size += i.CalculateSize();

            return size;
        }

    }
}
