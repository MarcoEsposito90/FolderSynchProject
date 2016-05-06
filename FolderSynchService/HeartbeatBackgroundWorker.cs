using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesProject
{
    class HeartbeatBackgroundWorker : BackgroundWorker
    {
        FolderSynchCallbackContract Callback;
        FolderSynchImplementation instanceContext;
        Timer timer;

        public HeartbeatBackgroundWorker(FolderSynchCallbackContract Callback, FolderSynchImplementation instanceContext) : base()
        {
            this.Callback = Callback;
            this.instanceContext = instanceContext;
        }


        public void startHeartbeat()
        {
            timer = new Timer(heartbeat, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(70));
        }

        public void stopHeartbeat()
        {
            timer.Dispose();
            timer = null;
        }

        private void heartbeat(object state)
        {
            Console.WriteLine("************ heartbeat *************** ");
            try
            {
                Callback.heartbeat();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                instanceContext.ChannelFault_Handler();
            }
        }
    }
}
