using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{

    [DataContract]
    public class Installation
    {

        [DataMember]
        public string MachineName
        {
            get;
            private set;
        }

        public Installation(string machineName)
        {
            this.MachineName = machineName;
        }
    }
}
