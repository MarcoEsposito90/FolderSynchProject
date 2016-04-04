using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.ServiceModel;

namespace FolderSynchService
{
    public class FolderSynchServer
    {
        /* ---------------------------------------------------------------- */
        /* ------------------ STATIC FIELDS ------------------------------- */
        /* ---------------------------------------------------------------- */

        private static string MAIN_DIRECTORY_RELATIVE_PATH = "\\FolderSynchHost";
        private static string REMOTE_FOLDERS_RELATIVE_PATH = MAIN_DIRECTORY_RELATIVE_PATH + "\\RemoteFolders";


        /* ---------------------------------------------------------------- */
        /* ------------------ PROPERTIES ---------------------------------- */
        /* ---------------------------------------------------------------- */

        public bool IsInitialized
        {
            get;
            private set;
        }

        public String MainDirectory
        {
            get
            {
                string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return docsFolder + MAIN_DIRECTORY_RELATIVE_PATH;
            }
        }


        public List<User> Users
        {
            get;
            private set;
        }


        public List<User> ConnectedUsers
        {
            get;
            private set;
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
        /* ------------ PUBLIC METHODS ------------------------------------ */
        /* ---------------------------------------------------------------- */

        /*******************************************************************************************************/
        public void Startup()
        {

            // 1) initialize files and directories ---------------------

            string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!Directory.Exists((docsFolder + MAIN_DIRECTORY_RELATIVE_PATH)))
                Directory.CreateDirectory(docsFolder + MAIN_DIRECTORY_RELATIVE_PATH);

            if (!Directory.Exists(docsFolder + REMOTE_FOLDERS_RELATIVE_PATH))
                Directory.CreateDirectory(docsFolder + REMOTE_FOLDERS_RELATIVE_PATH);

            UsersFileHandler.Instance.checkUsersFile();

            // 2) initialize users data structures --------------------------
            ConnectedUsers = new List<User>();
            Users = new List<User>();
            UsersFileHandler.Instance.ReadUsersFromFile(Users);


            IsInitialized = true;
        }



        /******************************************************************************************************/
        public User registerNewUser(string username, string password)
        {
            // check if server has startupped ----------------
            if (!IsInitialized)
            {
                Console.WriteLine("Trying to register a user on an uninitalized server");
                throw new FaultException<RegistrationFault>(new RegistrationFault(RegistrationFault.SERVER_ERROR));
            }

            lock (Users)
            {
                User newUser;

                // first user --------------------------------
                if (Users.Count == 0)
                {
                    newUser = new User(username, password);
                    Users.Add(newUser);
                    UsersFileHandler.Instance.WriteUsersList(Users);
                    return newUser;
                }

                // check if username is still available ------------
                foreach (User u in Users)
                {

                    if (u.Username.Equals(username))
                    {
                        Console.WriteLine("registration failed; username not available: " + username);
                        throw new FaultException<RegistrationFault>(new RegistrationFault(RegistrationFault.USERNAME_UNAVAILABLE));
                    }

                }

                // add new user to list -----------------------------
                newUser = new User(username, password);
                Users.Add(newUser);
                UsersFileHandler.Instance.WriteUsersList(Users);

                LoginUser(newUser);
                return newUser;
            }


        }

        /***************************************************************************************************/
        public User LoginUser(string username, string password)
        {
            lock (Users)
            {
                foreach (User u in Users)
                {
                    if (u.Username.Equals(username))
                    {
                        if (u.Password.Equals(password))
                        {
                            LoginUser(u);
                            return u;
                        }

                        throw new FaultException<LoginFault>(new LoginFault(LoginFault.WRONG_USERNAME_OR_PASSWORD));
                    }
                }

                throw new FaultException<LoginFault>(new LoginFault(LoginFault.WRONG_USERNAME_OR_PASSWORD));
            }

        }


        /*******************************************************************************************/
        public void logoutUser(User user)
        {
            lock (ConnectedUsers)
            {
                if (ConnectedUsers.Contains(user))
                    ConnectedUsers.Remove(user);
            }
            
        }

        /* ---------------------------------------------------------------- */
        /* ------------ AUXILIARY METHODS --------------------------------- */
        /* ---------------------------------------------------------------- */
        private void LoginUser(User u)
        {
            lock (ConnectedUsers)
            {
                if (ConnectedUsers.Contains(u))
                    throw new FaultException<LoginFault>(new LoginFault(LoginFault.USER_ALREADY_IN));

                ConnectedUsers.Add(u);
            }
        }
    }




}
