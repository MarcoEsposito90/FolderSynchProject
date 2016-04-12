using Newtonsoft.Json;
using ServicesProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    public class UpdatesFileHandler
    {

        public static readonly int LATEST_UPDATE = -1;

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ PROPERTIES ------------------------------------------ */
        /* ------------------------------------------------------------------------------ */

        public User User
        {
            get;
            private set;
        }

        public string BaseFolder
        {
            get;
            private set;
        }

        public List<Update> Updates
        {
            get;
            private set;
        }

        public string UpdatesFilePath
        {
            get
            {
                return FolderSynchServer.Instance.RemoteFoldersPath + "\\" + User.Username + "\\" + BaseFolder + "\\" + "updates.txt";
            }
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ CONSTRUCTORS ---------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public UpdatesFileHandler(User user, string folder)
        {
            User = user;
            BaseFolder = folder;
            Updates = new List<Update>();
            initialize();
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ UPDATE METHODS -------------------------------------- */
        /* ------------------------------------------------------------------------------ */
        private void initialize()
        {

            if (File.Exists(UpdatesFilePath))
                Updates = readFromFile();
            else
                writeToFile(new List<Update>());
        }

        /******************************************************************************/
        public void createNewUpdate(UpdateTransaction transaction)
        {
            int number = 0;
            if(Updates.Count > 0)
                number = Updates.ElementAt(Updates.Count-1).Number + 1;

            Update u = new Update(BaseFolder, transaction.Timestamp, number, transaction.TransactionID);

            // create the update folder
            Directory.CreateDirectory(  FolderSynchServer.Instance.RemoteFoldersPath + "\\" + 
                                        User.Username + "\\" + 
                                        BaseFolder + "\\" + 
                                        u.UpdateFolder);

            Updates.Add(u);
            writeToFile(Updates);
        }

        /********************************************************************************/
        public void AddFile(UpdateTransaction transaction, string localPath, byte[] data)
        {
            Update update = null;
            foreach(Update u in Updates)
                if (u.TransactionID.Equals(transaction.TransactionID))
                {
                    update = u;
                    break;
                }

            if (update == null)
                throw new FaultException(new FaultReason("Update object not found"));

            string filePath =   FolderSynchServer.Instance.RemoteFoldersPath + "\\" + 
                                User.Username + "\\" + 
                                update.BaseFolder + "\\" +
                                update.UpdateFolder + "\\" +
                                localPath;

            FileStream inputStream = new FileStream(filePath,
                                                    FileMode.Create,
                                                    FileAccess.Write,
                                                    FileShare.None);

            using (inputStream)
            {
                inputStream.Write(data, 0, data.Length);
                inputStream.Close();
            }

            update.UpdateEntries.Add(new Update.UpdateEntry(localPath, Update.UpdateEntry.NEW));
        }


        /***********************************************************************************/
        public void AddFileStreamed(UpdateTransaction transaction, string localPath, Stream stream)
        {

            Update update = null;
            foreach (Update u in Updates)
                if (u.TransactionID.Equals(transaction.TransactionID))
                {
                    update = u;
                    break;
                }

            if (update == null)
                throw new FaultException(new FaultReason("Update object not found"));

            string filePath =   FolderSynchServer.Instance.RemoteFoldersPath + "\\" + 
                                User.Username + "\\" + 
                                update.BaseFolder + "\\" + 
                                update.UpdateFolder + "\\" +
                                localPath;

            FileStream inputStream = new FileStream(filePath,
                                                    FileMode.Create,
                                                    FileAccess.Write,
                                                    FileShare.None);

            using (inputStream)
            {
                // allocate a buffer
                const int bufferSize = 10240;
                byte[] buffer = new byte[bufferSize];
                Console.WriteLine("Buffer allocated");

                // first they are moved to the buffer, then to the destination file
                int readBytesCount = 0;
                while ((readBytesCount = stream.Read(buffer, 0, bufferSize)) > 0)
                {

                    inputStream.Write(buffer, 0, readBytesCount);
                }

                inputStream.Close();
                stream.Close();
            }

            update.UpdateEntries.Add(new Update.UpdateEntry(localPath, Update.UpdateEntry.NEW));
        }

        /**********************************************************************************/
        public void addSubDirectory(UpdateTransaction transaction, string localPath)
        {

            Update update = null;
            foreach (Update u in Updates)
                if (u.TransactionID.Equals(transaction.TransactionID))
                {
                    update = u;
                    break;
                }

            if (update == null)
                throw new FaultException(new FaultReason("Update object not found"));

            string path =   FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
                            User.Username + "\\" +
                            update.BaseFolder + "\\" +
                            update.UpdateFolder + "\\" +
                            localPath;

            Directory.CreateDirectory(path);
            update.UpdateEntries.Add(new Update.UpdateEntry(localPath, Update.UpdateEntry.NEW));
        }


        /**********************************************************************************/
        public void commit(UpdateTransaction transaction)
        {
            Update update = null;
            foreach (Update u in Updates)
                if (u.TransactionID.Equals(transaction.TransactionID))
                {
                    update = u;
                    break;
                }

            if (update == null)
                throw new FaultException(new FaultReason("Update object not found"));

            writeToFile(Updates);
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ ROLLBACK METHODS ------------------------------------ */
        /* ------------------------------------------------------------------------------ */

        public void rollBack(string transactionID)
        {

            // 1) read updates file and find which update is related to this transaction
            Update targetUpdate = null;

            foreach(Update u in Updates)
            {
                if (u.TransactionID.Equals(transactionID))
                {
                    targetUpdate = u;
                    break;
                }
            }

            if (targetUpdate == null)
                throw new Exception("PANIC: uncommitted update not found");

            // 2) proceed to delete whole update folder
            deleteDirectory(    FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
                                User.Username + "\\" +
                                BaseFolder + "\\" +
                                targetUpdate.UpdateFolder);

            // 3) remove update object from list
            Updates.Remove(targetUpdate);
            writeToFile(Updates);
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ HISTORY METHODS ------------------------------------- */
        /* ------------------------------------------------------------------------------ */
        public List<Update> getHistory()
        {
            return Updates;
        }


        public List<Update.UpdateEntry> getFileHistory(string localPath)
        {

            List<Update.UpdateEntry> entries = new List<Update.UpdateEntry>();
            foreach(Update u in Updates)
            {
                foreach(Update.UpdateEntry entry in u.UpdateEntries)
                {
                    if (entry.ItemName.Equals(localPath))
                    {
                        entries.Add(entry);
                        break;
                    }
                }
            }

            return entries;
        }



        /*********************************************************************************/
        private void deleteDirectory(string path)
        {

            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
                File.Delete(file);

            string[] subFolders = Directory.GetDirectories(path);
            foreach (string dir in subFolders)
                deleteDirectory(dir);

            Directory.Delete(path);
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ DOWNLOAD METHODS ------------------------------------ */
        /* ------------------------------------------------------------------------------ */
        public byte[] getFile(string localPath, int updateNumber)
        {

            if (Updates.Count == 0)
                throw new FaultException(new FaultReason("No update available for this folder"));

            // 1) discover right update folder -----------------------------------
            string updateFolder = null;

            if (updateNumber == LATEST_UPDATE)
            {
                updateFolder = Updates.ElementAt(Updates.Count - 1).UpdateFolder;
            }
            else
            {
                foreach (Update u in Updates)
                {
                    if (u.Number == updateNumber)
                    {
                        updateFolder = u.UpdateFolder;
                        break;
                    }
                }
            }
            

            if (updateFolder == null)
                throw new FaultException(new FaultReason("Uknown or deleted update"));

            // 2) verify file path ---------------------------------------------
            string path = FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
                            User.Username + "\\" +
                            BaseFolder + "\\" +
                            updateFolder + "\\" +
                            localPath;

            if (!File.Exists(path))
                throw new FaultException(new FaultReason("No such file for this update"));


            // 2) copy file to a buffer ----------------------------------------
            FileInfo fi = new FileInfo(path);
            byte[] file = new byte[fi.Length];

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            using (fs)
            {
                fs.Read(file, 0, (int)fi.Length);
                fs.Close();
            }

            return file;
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ FILE METHODS ---------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private List<Update> readFromFile()
        {
            List<Update> updates = new List<Update>();

            lock (this)
            {
                FileStream fs = new FileStream(UpdatesFilePath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);

                string fileContent = null;

                using (fs)
                using (sr)
                {

                    fileContent = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                }

                Object o = JsonConvert.DeserializeObject(fileContent);
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)o;

                // add users to list
                foreach (Object i in array)
                {
                    Newtonsoft.Json.Linq.JObject jo = (Newtonsoft.Json.Linq.JObject)i;
                    Update u = (Update)jo.ToObject(typeof(Update));
                    updates.Add(u);
                }
            }

            return updates;

        }


        /**********************************************************************************/
        private void writeToFile(List<Update> updates)
        {
            lock (this)
            {
                StreamWriter sw = new StreamWriter(UpdatesFilePath, false);
                using (sw)
                {
                    string output = JsonConvert.SerializeObject(updates);
                    sw.WriteLine(output);

                    sw.Close();
                }
            }
        }
    }
}
