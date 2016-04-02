using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSynchService;
using Newtonsoft.Json;

namespace FolderSynchHost
{

    /* the purpose of this class is to provide a packed object which is in charge of
        doing most of the server work, and which can be used by any host application */
    class FolderSynchServer
    {
        /* ---------------------------------------------------------------- */
        /* ------------------ STATIC FIELDS ------------------------------- */
        /* ---------------------------------------------------------------- */

        private static string MAIN_DIRECTORY_RELATIVE_PATH = "\\FolderSynchHost";
        private static string REMOTE_FOLDERS_RELATIVE_PATH = MAIN_DIRECTORY_RELATIVE_PATH + "\\RemoteFolders";
        private static string USERS_FILE_RELATIVE_PATH = MAIN_DIRECTORY_RELATIVE_PATH + "\\users.txt";


        /* ---------------------------------------------------------------- */
        /* ------------------ PROPERTIES ---------------------------------- */
        /* ---------------------------------------------------------------- */

        public bool IsInitialized
        {
            get;
            private set;
        }

        public String UsersFile
        {
            get
            {
                if (!IsInitialized)
                    throw new Exception("The server is not initialized yet");

                string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return docsFolder + USERS_FILE_RELATIVE_PATH;
            }
            private set { }
        }

        /* ---------------------------------------------------------------- */
        /* ------------------ CONSTRUCTOR --------------------------------- */
        /* ---------------------------------------------------------------- */

        private static FolderSynchServer _instance = null;
        public static FolderSynchServer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FolderSynchServer();

                return _instance;
            }
        }

        private FolderSynchServer() { }


        /* ---------------------------------------------------------------- */
        /* ------------------ METHODS ------------------------------------- */
        /* ---------------------------------------------------------------- */

        /* startup is in charge to check if all host application files are available 
           and create them otherwise */
        public void Startup()
        {
            string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Console.WriteLine("documents folder path: " + docsFolder);

            if (!Directory.Exists((docsFolder + MAIN_DIRECTORY_RELATIVE_PATH)))
            {
                Directory.CreateDirectory(docsFolder + MAIN_DIRECTORY_RELATIVE_PATH);
            }

            if (!Directory.Exists(docsFolder + REMOTE_FOLDERS_RELATIVE_PATH))
            {
                Directory.CreateDirectory(docsFolder + REMOTE_FOLDERS_RELATIVE_PATH);
            }

            if (!File.Exists(docsFolder + USERS_FILE_RELATIVE_PATH))
            {

                // create file and write empty list
                StreamWriter sw = new StreamWriter(docsFolder + USERS_FILE_RELATIVE_PATH);
                List<User> users = new List<User>();
                string output = JsonConvert.SerializeObject(users);
                sw.WriteLine(output);
                sw.Close();
            }

            IsInitialized = true;
        }


    }
}
