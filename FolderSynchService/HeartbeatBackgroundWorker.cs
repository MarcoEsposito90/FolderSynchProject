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
        DateTime invocationTimestamp;

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

            invocationTimestamp = DateTime.Now;
            try
            {
                Callback.heartbeat();
                Console.WriteLine("***** Heartbeat response received ***** ");
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Over timeout");
                launchException();
            }
            catch(CommunicationException)
            {
                Console.WriteLine("Communication exception");
                launchException();
            }
        }


        private void launchException()
        {
            if (invocationTimestamp.CompareTo(instanceContext.LastCallTimestamp) > 0)
                instanceContext.ChannelFault_Handler();
            else
                Console.WriteLine("ignoring");
        }
    }
}
