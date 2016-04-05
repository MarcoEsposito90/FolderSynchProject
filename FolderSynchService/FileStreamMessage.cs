using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchService
{
    [MessageContract]
    public class FileStreamMessage : IDisposable
    {

        [MessageHeader]
        public string baseFolder;

        [MessageHeader]
        public string localPath;

        [MessageBodyMember]
        public Stream data;

        public FileStreamMessage()
        {

        }

        public FileStreamMessage(string baseFolder, string localPath, FileStream data)
        {
            this.baseFolder = baseFolder;
            this.localPath = localPath;
            this.data = data;
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
