using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.IO;

namespace FolderSynchHost
{
    class Program
    {

        static void Main(string[] args)
        {
            /* ---------------------------------------------------------------- */
            /* ------------------ STARTUP ------------------------------------- */
            /* ---------------------------------------------------------------- */

            FolderSynchServiceNamespace.FolderSynchServer server = FolderSynchServiceNamespace.FolderSynchServer.Instance;
            server.Startup();


            /*  NOTE:
                the "using" keyword ensures correct disposal for those objects which implement 
                the IDisposable interface, such as ServiceHost 
            */

            // first, create an instance of the service which will be hosted inside this process
            try
            {
                using (ServiceHost host = new ServiceHost(typeof(FolderSynchServiceNamespace.FolderSynchServiceImplementation)))
                {

                    // open the service to make it available. After that, it will react to clients' requests
                    Console.WriteLine("Opening host...");
                    host.Open();
                    Console.WriteLine("Host running and service available");
                    Console.WriteLine("press enter to stop");

                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {

                System.Console.WriteLine("excepion caught: " + e.GetType());
                Console.ReadLine();
            }


        }
    }
}
