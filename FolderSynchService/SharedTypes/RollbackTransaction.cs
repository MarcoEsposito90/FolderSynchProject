using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [DataContract]
    public class RollbackTransaction : Transaction
    {

        [DataMember]
        public int UpdateNumber { get; private set; }

        public RollbackTransaction(User user, int updateNumber, string baseFolder, DateTime timestamp) : base(user, baseFolder, timestamp)
        {
            this.UpdateNumber = updateNumber;
        }
    }

}
