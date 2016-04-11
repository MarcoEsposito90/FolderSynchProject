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


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ METHODS --------------------------------------------- */
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
            FileStream fs = new FileStream(UpdatesFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            using (fs)
            using (sw)
            {
                string output = JsonConvert.SerializeObject(updates);
                sw.WriteLine(output);

                sw.Close();
                fs.Close();
            }
        }
    }
}
