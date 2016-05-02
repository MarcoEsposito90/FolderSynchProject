using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [ServiceBehavior (InstanceContextMode = InstanceContextMode.PerCall)]
    public class StreamedTransferImplementation : StreamedTransferContract
    {

        public void uploadFileStreamed(FileStreamMessage message)
        {
            Console.WriteLine(message.username + "wants to upload a file with stream");
            FolderSynchServer.Instance.uploadFileStreamed(  message.username, 
                                                            message.transactionID,  
                                                            message.baseFolder, 
                                                            message.localPath,
                                                            message.updateType, 
                                                            message.data);
        }

        public Stream downloadFileStreamed(string username, string baseFolder, string localPath, int updateNumber)
        {
            Console.WriteLine(username + " wants to download " + baseFolder + "\\" + localPath + " in streamed mode. (update " + updateNumber + ")");
            return FolderSynchServer.Instance.downloadFileStreamed(username, baseFolder, localPath, updateNumber);
        }


        public void heartBeat(string username)
        {
        }
    }
}
