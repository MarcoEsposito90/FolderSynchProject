using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSynchService;
using System.IO;
using Newtonsoft.Json;

namespace FolderSynchHost
{
    class FolderSynchServiceImplementation : FolderSynchServiceContract
    {
        public bool RegisterNewUser(string username, string password)
        {
            string usersFilePath;

            try
            {
                usersFilePath = FolderSynchServer.Instance.UsersFile;

                StreamReader sr = new StreamReader(usersFilePath);
                string fileContent = sr.ReadToEnd();
                sr.Close();

                Console.WriteLine(fileContent);

                Object o = JsonConvert.DeserializeObject(fileContent);
                Console.WriteLine("type = " + o.GetType());

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

                    StreamWriter sw = new StreamWriter(usersFilePath);
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

                StreamWriter sw2 = new StreamWriter(usersFilePath);
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
