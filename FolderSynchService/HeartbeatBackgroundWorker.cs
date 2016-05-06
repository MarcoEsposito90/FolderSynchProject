using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesProject
{
    class HeartbeatBackgroundWorker : BackgroundWorker
    {
        FolderSynchImplementation instanceContext;
        Timer timer;

        public HeartbeatBackgroundWorker(FolderSynchImplementation instanceContext) : base()
        {
            this.instanceContext = instanceContext;
        }


        public void startHeartbeat()
        {
            timer = new Timer(heartbeat, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        public void stopHeartbeat()
        {
            timer.Dispose();
        }

        private void heartbeat(object state)
        {
            instanceContext.Callback.heartbeat();
        }
    }
}
