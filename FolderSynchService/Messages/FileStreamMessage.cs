using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [MessageContract]
    public class FileStreamMessage : IDisposable
    {

        [MessageHeader]
        public string baseFolder;

        [MessageHeader]
        public string localPath;

        [MessageHeader]
        public string username;

        [MessageBodyMember]
        public Stream data;

        public FileStreamMessage()
        {

        }

        public void Dispose()
        {
            if(data != null)
            {
                data.Close();
                data = null;
            }
        }
    }
}
