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

            DateTime invocationTimestamp = DateTime.Now;
            try
            {
                Callback.heartbeat();
            }
            catch (TimeoutException e)
            {
                Console.WriteLine("Over timeout");

                if (invocationTimestamp.CompareTo(instanceContext.LastCallTimestamp) > 0)
                    instanceContext.ChannelFault_Handler();
                else
                    Console.WriteLine("ignoring");
            }
            catch(CommunicationException f)
            {
                
                Console.WriteLine("Communication fault");
                instanceContext.ChannelFault_Handler();
            }
        }
    }
}
