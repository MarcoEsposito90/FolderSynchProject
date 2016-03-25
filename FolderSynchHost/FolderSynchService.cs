using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSynchService;

namespace FolderSynchHost
{
    class FolderSynchService : IFolderSynchService
    {
        public static int requestNumber = 0;
        public static bool boolValue = true;

        public string DoWork()
        {
            requestNumber++;
            Console.WriteLine("DoWork called");
            return "you asked to execute DoWork at: " + DateTime.Now.ToString();
        }

        public int DoWork2()
        {
            Console.WriteLine("DoWork2 called");
            return ++requestNumber;
        }

        public bool DoWork3()
        {
            boolValue = !boolValue;
            return boolValue;
        }
    }
}
