using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchService
{
    class UsersFileHandler
    {

        private static string USERS_FILE_RELATIVE_PATH = "\\users.txt";


        /* ---------------------------------------------------------------- */
        /* ------------------ CONSTRUCTOR --------------------------------- */
        /* ---------------------------------------------------------------- */

        private static UsersFileHandler _instance = null;
        public static UsersFileHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UsersFileHandler();

                return _instance;
            }
        }

        private UsersFileHandler() { }


        /* ---------------------------------------------------------------- */
        /* ------------------ METHODS ------------------------------------- */
        /* ---------------------------------------------------------------- */
        public void checkUsersFile()
        {
            if (!File.Exists(FolderSynchServer.Instance.MainDirectory + USERS_FILE_RELATIVE_PATH))
            {
                WriteUsersList(new List<User>());
            }
        }


        public void WriteUsersList(List<User> users)
        {
            lock (_instance)
            {

                StreamWriter sw = new StreamWriter(FolderSynchServer.Instance.MainDirectory + USERS_FILE_RELATIVE_PATH);
                string output = JsonConvert.SerializeObject(users);
                sw.WriteLine(output);
                sw.Close();
            }

        }

        public void ReadUsersFromFile(List<User> users)
        {

            lock (_instance)
            {
                // open users file
                StreamReader sr = new StreamReader(FolderSynchServer.Instance.MainDirectory + USERS_FILE_RELATIVE_PATH);
                string fileContent = sr.ReadToEnd();
                sr.Close();

                Object o = JsonConvert.DeserializeObject(fileContent);
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)o;

                // add users to list
                foreach (Object i in array)
                {

                    Newtonsoft.Json.Linq.JObject jo = (Newtonsoft.Json.Linq.JObject)i;
                    User u = (User)jo.ToObject(typeof(User));
                    users.Add(u);
                }
            }

        }
    }
}
