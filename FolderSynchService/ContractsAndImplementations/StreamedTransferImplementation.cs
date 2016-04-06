using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{

    public class StreamedTransferImplementation : StreamedTransferContract
    {

        public void uploadFileStreamed(FileStreamMessage message)
        {
            Console.WriteLine(message.username + "wants to upload a file with stream");
            FolderSynchServer.Instance.addNewFileStreamed(message.username, message.baseFolder, message.localPath, message.data);
        }
    }
}
