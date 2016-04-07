using ServicesProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject.HostClasses
{
    class TransactionsHandler
    {

        public enum Operations
        {
            New,
            Commit,
            Abort,
            NewFile,
            NewFolder,
            DeleteFile,
            DeleteFolder,
            UpdateFile
        }
        private static string LOG_FILE_RELATIVE_PATH = "\\TransactionsLog.txt";


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ PROPERTIES ------------------------------------------ */
        /* ------------------------------------------------------------------------------ */

        public Dictionary<String, UpdateTransaction> ActiveTransactions;

        public static string LogFilePath
        {
            get
            {
                return FolderSynchServer.Instance.MainDirectoryPath + LOG_FILE_RELATIVE_PATH;
            }
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ CONSTRUCTOR ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private static TransactionsHandler _instance = null;
        public static TransactionsHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TransactionsHandler();

                return _instance;
            }
        }

        private TransactionsHandler() {

            ActiveTransactions = new Dictionary<string, UpdateTransaction>();
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ METHODS --------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        /**********************************************************************************/
        public void CheckLogFile()
        {
            if (!File.Exists(FolderSynchServer.Instance.MainDirectoryPath + LOG_FILE_RELATIVE_PATH))
            {
                FileStream fs = new FileStream( LogFilePath,
                                                FileMode.Create);
                using (fs)
                    fs.Close();
            }
        }

        /**********************************************************************************/
        public void AddTransaction(UpdateTransaction transaction)
        {
            if (ActiveTransactions.Keys.Contains(transaction.TransactionID))
                throw new FaultException(new FaultReason("error: transaction ID not available"));

            ActiveTransactions.Add(transaction.TransactionID, transaction);
            WriteLogEntry(transaction, Operations.New, "");
        }


        /**********************************************************************************/
        public void AddOperation(UpdateTransaction transaction, Operations operation, string path)
        {

            if (!ActiveTransactions.ContainsValue(transaction))
                throw new FaultException(new FaultReason("no transaction active for this upload"));

            WriteLogEntry(transaction, operation, path);
        }


        /**********************************************************************************/
        public void CommitTransaction(UpdateTransaction transaction)
        {

            if(!ActiveTransactions.ContainsValue(transaction))
                throw new FaultException(new FaultReason("no transaction active for this upload"));

            WriteLogEntry(transaction, Operations.Commit, "");
            ActiveTransactions.Remove(transaction.TransactionID);
        }


        /**********************************************************************************/
        private void WriteLogEntry(UpdateTransaction transaction, Operations operation, string path)
        {
            lock (_instance)
            {
                FileStream fs = new FileStream( LogFilePath,
                                                FileMode.Append,
                                                FileAccess.Write);

                StreamWriter sw = new StreamWriter(fs);

                using (fs)
                using (sw)
                {
                    string logLine = transaction.TransactionID + ";" + operation + ";" + path;
                    Console.WriteLine("writing to log: " + logLine);

                    sw.WriteLine(logLine);

                    sw.Close();
                    fs.Close();
                }

            }
            
        }
    }
}
