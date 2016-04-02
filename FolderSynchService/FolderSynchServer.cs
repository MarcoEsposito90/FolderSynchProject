using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace FolderSynchService
{
    public class FolderSynchServer
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

        private String UsersFile
        {
            get
            {
                if (!IsInitialized)
                    throw new Exception("The server is not initialized yet");

                string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return docsFolder + USERS_FILE_RELATIVE_PATH;
            }
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


        /*  this method is in charge of registrating a new user */
        public bool registerNewUser(string username, string password)
        {

            if (!IsInitialized)
            {
                Console.WriteLine("Trying to register a user on an uninitalized server");
                return false;
            }

            try
            {
                StreamReader sr = new StreamReader(this.UsersFile);
                string fileContent = sr.ReadToEnd();
                sr.Close();

                Object o = JsonConvert.DeserializeObject(fileContent);

                if (!o.GetType().Equals(typeof(Newtonsoft.Json.Linq.JArray)))
                {
                    Console.WriteLine("wrong type: " + o.GetType());
                    return false;
                }

                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)o;
                List<User> users = new List<User>();

                if (array.Count == 0)
                {
                    // first user
                    users.Add(new User(username, password));

                    string output = JsonConvert.SerializeObject(users);

                    StreamWriter sw = new StreamWriter(this.UsersFile);
                    sw.WriteLine(output);
                    sw.Close();

                    return true;
                }

                foreach (Object i in array)
                {
                    if (!i.GetType().Equals(typeof(Newtonsoft.Json.Linq.JObject)))
                    {
                        Console.WriteLine("Wrong type: " + i.GetType());
                        return false;
                    }


                    Newtonsoft.Json.Linq.JObject jo = (Newtonsoft.Json.Linq.JObject)i;
                    User u = (User)jo.ToObject(typeof(User));

                    if (u.Username.Equals(username))
                    {
                        Console.WriteLine("registration failed: username not available: " + username);
                        return false;
                    }

                    users.Add(u);
                }

                users.Add(new User(username, password));
                string output2 = JsonConvert.SerializeObject(users);

                StreamWriter sw2 = new StreamWriter(this.UsersFile);
                sw2.WriteLine(output2);
                sw2.Close();

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error in registration: " + e.Message);
                return false;
            }
        }
    }
}
