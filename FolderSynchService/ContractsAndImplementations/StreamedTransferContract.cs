using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [ServiceContract]
    public interface StreamedTransferContract
    {

        [OperationContract]
        void uploadFileStreamed(FileStreamMessage message);

        [OperationContract]
        Stream downloadFileStreamed(string username, string baseFolder, string localPath, int updateNumber);

        [OperationContract]
        void heartBeat(string username);
    }
}
