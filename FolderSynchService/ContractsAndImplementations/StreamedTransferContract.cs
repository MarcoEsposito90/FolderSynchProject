using System;
using System.Collections.Generic;
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
    }
}
