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
    public abstract class Item
    {
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /* ---------------------------------------------------------------- */
        /* ------------ SERIALIZABLE PROPERTIES --------------------------- */
        /* ---------------------------------------------------------------- */

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public Update LastUpdate { get; set; }

        [DataMember]
        public DateTime SynchDate { get; set; }

        public ObservableCollection<Update.UpdateEntry> Updates
        {
            get;
            set;
        }


        public long CurrentSize
        {
            get
            {
                return CalculateSize();
            }
        }

        /* ---------------------------------------------------------------- */
        /* ------------ TEMPORARY PROPERTIES ------------------------------ */
        /* ---------------------------------------------------------------- */


        public string SizeInBytes
        {
            get
            {
                return SizeSuffix(CalculateSize());
            }
        }


        /* ---------------------------------------------------------------- */
        /* ------------ ABSTRACT METHODS ---------------------------------- */
        /* ---------------------------------------------------------------- */

        public abstract long CalculateSize();


        /* ---------------------------------------------------------------- */
        /* ------------ METHODS ------------------------------------------- */
        /* ---------------------------------------------------------------- */

        public string SizeSuffix(long value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }



        /* ---------------------------------------------------------------- */
        /* ------------ CHANGE OBJECT ------------------------------------- */
        /* ---------------------------------------------------------------- */

        public class Change : IComparable
        {
            /*  ATTENTION!!!!! 
                These statics must ABSOLUTELY be equal to the statics defined in the class
                Update.UpdateEntry of the service. Please don't touch them!!!
            */

            public static readonly int NEW_FILE = 0;
            public static readonly int CHANGED_FILE = 1;
            public static readonly int NEW_DIRECTORY = 2;
            public static readonly int DELETED_DIRECTORY = 3;
            public static readonly int DELETED_FILE = 4;


            public int Type { get; private set; }
            public string Path { get; private set; }

            public Change(int type, string path)
            {
                Type = type;
                Path = path;
            }

            public int CompareTo(object obj)
            {
                if (!obj.GetType().Equals(typeof(Change)))
                    throw new Exception("ivoking compareTo for another type object in Item.Change.CompareTo");

                Change c = (Change)obj;

                int dif = c.Type - Type;

                if (dif != 0)
                    return dif;

                // if the change type is the same, we must check who is higher in hierarchy
                int pathLengthDif = Path.Length - c.Path.Length;
                if (Type == NEW_DIRECTORY)
                    return pathLengthDif;
                else if (Type == DELETED_DIRECTORY)
                    return -pathLengthDif;

                return 0;
            }
        }
    }

    

}
