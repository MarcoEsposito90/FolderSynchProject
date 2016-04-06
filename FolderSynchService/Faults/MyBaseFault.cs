using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [DataContract]
    public class MyBaseFault
    {

        [DataMember]
        public string Message
        {
            get;
            private set;
        }

        public MyBaseFault(string message)
        {
            this.Message = message;
        }

    }
}
