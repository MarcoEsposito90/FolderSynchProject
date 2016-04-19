using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    public class RollbackTransactionsHandler : TransactionsHandler
    {

        public override string LogFilePath
        {
            get
            {
                return FolderSynchServer.Instance.MainDirectoryPath + "\\RollbackTransactionsLog.txt";
            }
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------------ CONSTRUCTOR ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private static RollbackTransactionsHandler _instance = null;
        public static RollbackTransactionsHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RollbackTransactionsHandler();

                return _instance;
            }
        }

        protected RollbackTransactionsHandler() : base()
        {

        }
    }
}
