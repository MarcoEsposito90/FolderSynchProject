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



        /******************************************************************************/
        private void initialize()
        {

            if (File.Exists(UpdatesFilePath))
                Updates = readFromFile();
            else
                writeToFile(new List<Update>());

            Updates.Sort();
            foreach (Update u in Updates)
                Console.WriteLine("update number " + u.Number);
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ UPDATE METHODS -------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public void createNewUpdate(UpdateTransaction transaction)
        {
            int number = 0;
            if (Updates.Count > 0)
                number = Updates.ElementAt(Updates.Count - 1).Number + 1;

            Console.WriteLine("creating update with numer " + number);
            Update u = new Update(BaseFolder, transaction.Timestamp, number, transaction.TransactionID);

            // create the update folder
            Directory.CreateDirectory(FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
                                        User.Username + "\\" +
                                        BaseFolder + "\\" +
                                        u.UpdateFolder);

            Updates.Add(u);
            Updates.Sort();
            writeToFile(Updates);
        }

        /********************************************************************************/
        public void AddFile(UpdateTransaction transaction, string localPath, int type, byte[] data)
        {
            Update update = checkUpdateObject(transaction);
            if (!checkMotherFolder(update, localPath))
                throw new FaultException(new FaultReason(FileTransferFault.MISSING_FOLDER));

            if (!(type == Update.UpdateEntry.MODIFIED_FILE || type == Update.UpdateEntry.NEW_FILE))
                throw new FaultException(new FaultReason(FileTransferFault.INCONSISTENT_UPDATE));



            string filePath = FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
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

            FileInfo fi = new FileInfo(filePath);
            update.UpdateEntries.Add(new Update.UpdateEntry(localPath, type, update.Timestamp, fi.Length, update.Number));
        }


        /***********************************************************************************/
        public void AddFileStreamed(UpdateTransaction transaction, string localPath, int type, Stream stream)
        {

            Update update = checkUpdateObject(transaction);
            if (!checkMotherFolder(update, localPath))
                throw new FaultException(new FaultReason(FileTransferFault.MISSING_FOLDER));

            if (!(type == Update.UpdateEntry.MODIFIED_FILE || type == Update.UpdateEntry.NEW_FILE))
                throw new FaultException(new FaultReason(FileTransferFault.INCONSISTENT_UPDATE));

            string filePath = FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
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

            FileInfo fi = new FileInfo(filePath);
            update.UpdateEntries.Add(new Update.UpdateEntry(localPath, type, update.Timestamp, fi.Length, update.Number));
        }



        /**********************************************************************************/
        public void deleteFile(UpdateTransaction transaction, string localPath)
        {
            Update update = checkUpdateObject(transaction);
            if (!checkMotherFolder(update, localPath))
                throw new FaultException(new FaultReason(FileTransferFault.MISSING_FOLDER));

            Console.WriteLine("handler is deleting file");
            update.UpdateEntries.Add(new Update.UpdateEntry(localPath, Update.UpdateEntry.DELETED_FILE, update.Timestamp, 0, update.Number));
        }



        /**********************************************************************************/
        public void addSubDirectory(UpdateTransaction transaction, string localPath)
        {

            Update update = checkUpdateObject(transaction);
            if (!checkMotherFolder(update, localPath))
                throw new FaultException(new FaultReason(FileTransferFault.MISSING_FOLDER));

            string path = FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
                            User.Username + "\\" +
                            update.BaseFolder + "\\" +
                            update.UpdateFolder + "\\" +
                            localPath;

            Directory.CreateDirectory(path);
            DirectoryInfo di = new DirectoryInfo(path);
            update.UpdateEntries.Add(new Update.UpdateEntry(localPath, Update.UpdateEntry.NEW_DIRECTORY, update.Timestamp, 0, update.Number));
        }



        /**********************************************************************************/
        public void deleteSubDirectory(UpdateTransaction transaction, string localPath)
        {
            Update update = checkUpdateObject(transaction);
            if (!checkMotherFolder(update, localPath))
                throw new FaultException(new FaultReason(FileTransferFault.MISSING_FOLDER));

            Console.WriteLine("handler is deleting subDirectory");
            update.UpdateEntries.Add(new Update.UpdateEntry(localPath, Update.UpdateEntry.DELETED_DIRECTORY, update.Timestamp, 0, update.Number));
        }



        /**********************************************************************************/
        public Update commit(UpdateTransaction transaction)
        {
            Update update = checkUpdateObject(transaction);
            writeToFile(Updates);
            return update;
        }



        /**********************************************************************************/
        private Update checkUpdateObject(UpdateTransaction transaction)
        {
            Update update = null;
            foreach (Update u in Updates)
                if (u.TransactionID.Equals(transaction.TransactionID))
                {
                    update = u;
                    break;
                }

            if (update == null)
                throw new FaultException(new FaultReason(FileTransferFault.NO_UPDATE_FOUND));

            return update;
        }


        /**********************************************************************************/
        private bool checkMotherFolder(Update update, string localPath)
        {

            string[] tokens = localPath.Split('\\');
            string motherFolderLocalPath = "";

            for (int i = 0; i < tokens.Length - 1; i++)
            {
                if (i != 0)
                    motherFolderLocalPath += "\\";
                motherFolderLocalPath += (tokens[i]);
            }

            string path = FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
                          User.Username + "\\" +
                          update.BaseFolder + "\\" +
                          update.UpdateFolder + "\\" +
                          motherFolderLocalPath;

            if (Directory.Exists(path))
                return true;

            if (update.Number == 0)
            {
                foreach (Update.UpdateEntry entry in update.UpdateEntries)
                {
                    if (entry.ItemLocalPath.Equals(motherFolderLocalPath) && entry.UpdateType == Update.UpdateEntry.NEW_DIRECTORY)
                        return true;
                }
            }
            else
            {
                for (int i = update.Number - 1; i >= 0; i--)
                {
                    Update u = Updates.ElementAt(i);

                    foreach (Update.UpdateEntry entry in u.UpdateEntries)
                        if (entry.ItemLocalPath.Equals(motherFolderLocalPath))
                        {
                            if (entry.UpdateType == Update.UpdateEntry.NEW_DIRECTORY)
                            {
                                Directory.CreateDirectory(path);
                                return true;
                            }
                            else if (entry.UpdateType == Update.UpdateEntry.DELETED_DIRECTORY)
                                return false;
                            else
                                throw new Exception("PANIC!! Detected inconsistent update entry");
                        }
                }
            }

            return false;
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ ROLLBACK METHODS ------------------------------------ */
        /* ------------------------------------------------------------------------------ */

        public void rollBack(string transactionID)
        {

            // 1) read updates file and find which update is related to this transaction
            Update targetUpdate = null;

            foreach (Update u in Updates)
            {
                if (u.TransactionID.Equals(transactionID))
                {
                    targetUpdate = u;
                    break;
                }
            }

            if (targetUpdate == null)
                return;

            // 2) proceed to delete whole update folder
            deleteDirectory(FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
                                User.Username + "\\" +
                                BaseFolder + "\\" +
                                targetUpdate.UpdateFolder);

            // 3) remove update object from list
            Updates.Remove(targetUpdate);
            Updates.Sort();
            writeToFile(Updates);
        }


        /*********************************************************************************/
        private void deleteDirectory(string path)
        {
            Console.WriteLine("deleting dir: " + path);
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
                File.Delete(file);

            string[] subFolders = Directory.GetDirectories(path);
            foreach (string dir in subFolders)
                deleteDirectory(dir);

            Directory.Delete(path);
            Console.WriteLine("completed deleting dir: " + path);
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ HISTORY METHODS ------------------------------------- */
        /* ------------------------------------------------------------------------------ */
        public List<Update> getHistory()
        {
            Console.WriteLine("Handler is returning updates list for " + User.Username + " - " + BaseFolder);

            foreach (Update u in Updates)
                u.printUpdate();
            return Updates;
        }


        public List<Update.UpdateEntry> getFileHistory(string localPath)
        {

            Console.WriteLine("Handler is returning updates list for " + User.Username + " - " + BaseFolder + "\\" + localPath);
            List<Update.UpdateEntry> entries = new List<Update.UpdateEntry>();
            foreach (Update u in Updates)
            {
                foreach (Update.UpdateEntry entry in u.UpdateEntries)
                {
                    if (entry.ItemLocalPath.Equals(localPath))
                    {
                        Console.WriteLine("-----------------------------------------------");
                        Console.WriteLine("ItemLocalPath: " + entry.ItemLocalPath);
                        Console.WriteLine("EntryTimestamp: " + entry.EntryTimestamp);
                        Console.WriteLine("UpdateType: " + entry.UpdateType);
                        entries.Add(entry);
                        break;
                    }
                }
            }

            entries.Sort();
            return entries;
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------------ DOWNLOAD METHODS ------------------------------------ */
        /* ------------------------------------------------------------------------------ */

        public List<Update.UpdateEntry> getUpdateFilesList(int updateNumber)
        {
            List<Update.UpdateEntry> entries = new List<Update.UpdateEntry>();
            Update update = null;

            foreach(Update u in Updates)
            {
                if(u.Number == updateNumber)
                {
                    update = u;
                    break;
                }
            }

            if (update == null)
                throw new FaultException(new FaultReason("no update with number " + updateNumber));

            List<string> ignores = new List<string>();
            List<string> added = new List<string>();
            for(int i = updateNumber; i >= 0; i--)
            {
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine("scanning update " + i);
                foreach(Update.UpdateEntry e in Updates.ElementAt(i).UpdateEntries)
                {
                    Console.WriteLine("**********************");
                    Console.WriteLine("checking update entry: " + e.ItemLocalPath + "; " + e.UpdateType);
                    if (ignores.Contains(e.ItemLocalPath))
                    {
                        Console.WriteLine("entry ignored");
                        continue;
                    }

                    if (e.UpdateType == Update.UpdateEntry.DELETED_FILE || e.UpdateType == Update.UpdateEntry.DELETED_DIRECTORY)
                    {
                        Console.WriteLine("adding entry to ignores");
                        ignores.Add(e.ItemLocalPath);
                        continue;
                    }

                    if (!added.Contains(e.ItemLocalPath))
                    {
                        Console.WriteLine("adding entry to list");
                        entries.Add(e);
                        added.Add(e.ItemLocalPath);
                    }
                    else
                    {
                        Console.WriteLine("found later update already");
                    }
                }
            }

            entries.Sort();
            return entries;
        }


        /***********************************************************************************/
        public byte[] getFile(string localPath, int updateNumber)
        {

            if (Updates.Count == 0)
                throw new FaultException(new FaultReason("No update available for this folder"));

            // 1) discover right update folder -----------------------------------
            string updateFolder = getUpdateFolder(updateNumber);

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



        /***********************************************************************************/
        public Stream getFileStreamed(string localPath, int updateNumber)
        {
            if (Updates.Count == 0)
                throw new FaultException(new FaultReason("No update available for this folder"));

            // 1) discover right update folder -----------------------------------
            string updateFolder = getUpdateFolder(updateNumber);


            // 2) verify file path ---------------------------------------------
            string path = FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
                            User.Username + "\\" +
                            BaseFolder + "\\" +
                            updateFolder + "\\" +
                            localPath;

            if (!File.Exists(path))
                throw new FaultException(new FaultReason("No such file for this update"));

            Stream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            return fs;
        }


        /***********************************************************************************/
        public void commitRollback(int updateNumber)
        {
            for(int i = updateNumber + 1; i < Updates.Count; i++)
            {
                deleteDirectory(FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
                                User.Username + "\\" +
                                BaseFolder + "\\" +
                                Updates.ElementAt(i).UpdateFolder);

            }

            Updates.RemoveRange(updateNumber + 1, (Updates.Count - updateNumber - 1));
            writeToFile(Updates);
        }


        /***********************************************************************************/
        private string getUpdateFolder(int updateNumber)
        {

            string updateFolder = null;

            foreach (Update u in Updates)
            {
                if (u.Number == updateNumber)
                {
                    updateFolder = u.UpdateFolder;
                    break;
                }
            }


            if (updateFolder == null)
                throw new FaultException(new FaultReason("Uknown or deleted update"));

            return updateFolder;
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ DELETE METHODS -------------------------------------- */
        /* ------------------------------------------------------------------------------ */
        public void removeFolder()
        {
            foreach(Update u in Updates)
            {
                string path = FolderSynchServer.Instance.RemoteFoldersPath + "\\" +
                              User.Username + "\\" +
                              BaseFolder;

                deleteDirectory(path);
                Console.WriteLine("Directory deleted");
                Updates = null;
            }
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
