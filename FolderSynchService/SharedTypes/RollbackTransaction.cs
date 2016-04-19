using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [DataContract]
    public class RollbackTransaction
    {

        [DataMember]
        public string TransactionID { get; private set; }

        [DataMember]
        public User User { get; private set; }

        [DataMember]
        public int UpdateNumber { get; private set; }

        [DataMember]
        public string BaseFolder { get; private set; }

        [DataMember]
        public DateTime Timestamp { get; private set; }


        public RollbackTransaction(User user, int updateNumber, string baseFolder, DateTime timestamp)
        {
            this.User = user;
            this.UpdateNumber = updateNumber;
            this.BaseFolder = baseFolder;
            this.Timestamp = timestamp;
            this.TransactionID = user.Username + "|" + baseFolder + "|" + timestamp.ToString();
        }
    }

}
