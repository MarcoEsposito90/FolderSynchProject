using FolderSynchMUIClient.FolderSynchService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{
    [DataContract]
    public class FileItem : Item
    {
        /* ---------------------------------------------------------------- */
        /* ------------ CONSTRUCTOR --------------------------------------- */
        /* ---------------------------------------------------------------- */

        public FileItem(string name, string relativePath)
        {
            this.Name = name;
            this.Path = relativePath;
        }


        /* ---------------------------------------------------------------- */
        /* ------------ OVERRIDE METHODS ---------------------------------- */
        /* ---------------------------------------------------------------- */

        public override long CalculateSize()
        {
            FileInfo fi = new FileInfo(Path);
            return fi.Length;
        }

    }
}
