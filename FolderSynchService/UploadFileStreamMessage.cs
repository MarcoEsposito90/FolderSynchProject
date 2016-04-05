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
    public class UploadFileStreamMessage
    {

        [MessageHeader]
        public string baseFolder;

        [MessageHeader]
        public string localPath;

        [MessageBodyMember]
        public FileStream data;
    }
}
