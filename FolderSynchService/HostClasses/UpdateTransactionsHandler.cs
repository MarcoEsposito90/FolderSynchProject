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
    class UpdateTransactionsHandler : TransactionsHandler
    {

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ PROPERTIES ------------------------------------------ */
        /* ------------------------------------------------------------------------------ */

        public override string LogFilePath
        {
            get
            {
                return FolderSynchServer.Instance.MainDirectoryPath + "\\UpdateTransactionsLog.txt";
            }
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ CONSTRUCTOR ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private static UpdateTransactionsHandler _instance = null;
        public static UpdateTransactionsHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UpdateTransactionsHandler();

                return _instance;
            }
        }

        private UpdateTransactionsHandler() : base() {

        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ METHODS --------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        /**********************************************************************************/
        public void AddOperation(UpdateTransaction transaction, Operations operation, string path)
        {

            if (!ActiveTransactions.ContainsValue(transaction))
                throw new FaultException(new FaultReason("no transaction active for this upload"));

            WriteLogEntry(transaction, operation, path);
        }

    }
}
