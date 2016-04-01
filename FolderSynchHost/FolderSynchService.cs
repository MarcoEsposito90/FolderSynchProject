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
    class FolderSynchService : IFolderSynchService
    {
        public bool RegisterNewUser(string username, string password)
        {
            string usersFilePath;

            try
            {
                usersFilePath = FolderSynchServer.Instance.UsersFile;

                using (StreamReader sr = new StreamReader(usersFilePath))
                {

                    string fileContent = sr.ReadToEnd();
                    Console.WriteLine(fileContent);
                    return true;
                }


            }
            catch (Exception)
            {
                return false;
            }


        }
    }
}
