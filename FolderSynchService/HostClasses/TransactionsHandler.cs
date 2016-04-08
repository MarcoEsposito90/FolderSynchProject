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
        private static string LOG_CLEAN_START = "LOG_CLEAN_START";
        private static string LOG_CLEAN_CLEAN_COMPLETE = "LOG_CLEAN_COMPLETE";

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


        /*******************************************************************************/
        private void cleanLogFile()
        {
            lock (_instance)
            {

                List<String> lines = new List<string>();

                // 0) open the file in read mode
                FileStream fs = new FileStream(LogFilePath,
                                                FileMode.Open,
                                                FileAccess.Read);

                StreamReader sr = new StreamReader(fs);

                using (fs)
                using (sr)
                {
                    // 1) start reading all log file
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {

                        string[] tokens = line.Split(';');

                        // 2) if the entry is a committ or an abort, it is finished
                        if (tokens[2].Equals(Operations.Commit) || tokens[2].Equals(Operations.Abort))
                        {

                            // 3) remove all lines relative to that transaction
                            foreach (string s in lines)
                            {
                                string[] tokens2 = s.Split(';');
                                if (tokens2[1].Equals(tokens[1]))
                                    lines.Remove(s);
                            }
                        }
                    }

                    sr.Close();
                    fs.Close();
                }

                // 4) open the file in write mode
                fs = new FileStream(LogFilePath,
                                    FileMode.Open,
                                    FileAccess.Write);

                StreamWriter sw = new StreamWriter(fs);

                using (fs)
                using (sw)
                {
                    // 5) writing log file
                    foreach(string s in lines)
                        sw.WriteLine(s);

                    fs.Close();
                    sw.Close();
                }
            }
        }


        /*************************************************************************/
        private void checkForRecovery()
        {

            cleanLogFile();

            lock (_instance)
            {
                // 0) open the file in read mode
                FileStream fs = new FileStream(LogFilePath,
                                                FileMode.Open,
                                                FileAccess.Read);

                StreamReader sr = new StreamReader(fs);

                using (fs)
                using (sr)
                {
                    // 1) start reading all log file
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {

                        string[] tokens = line.Split(';');
                        
                        /*  TODO: find a way to bind the transaction to the relative update.
                            after finding the update, remove its folder completely, like it never
                            happened. Next time, client will have to repeat it from beginning
                        */
                    }

                    sr.Close();
                    fs.Close();
                }
            }
        }
    }
}
