using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [DataContract]
    public class UpdateTransaction
    {

        public enum TransactionState { Pending, Committed, Aborted }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ PROPERTIES ------------------------------------------ */
        /* ------------------------------------------------------------------------------ */

        [DataMember]
        public string TransactionID
        {
            get;
            private set;
        }

        [DataMember]
        public User User
        {
            get;
            private set;
        }

        [DataMember]
        public string FolderName
        {
            get;
            private set;
        }

        [DataMember]
        public DateTime Timestamp
        {
            get;
            private set;
        }

        public TransactionState State
        {
            get;
            set;
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ CONSTRUCTOR ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public UpdateTransaction(User user, string folderName, DateTime timestamp)
        {
            this.TransactionID = user.Username + folderName + timestamp.ToString();
            this.User = user;
            this.FolderName = folderName;
            this.Timestamp = timestamp;
            this.State = TransactionState.Pending;
        }
    }
}
