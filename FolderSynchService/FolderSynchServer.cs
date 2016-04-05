﻿using System;
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

        // main directory path is relative to documents folder
        private static string MAIN_DIRECTORY_RELATIVE_PATH = "\\FolderSynchHost";

        // all others are relative to main folder
        private static string REMOTE_FOLDERS_RELATIVE_PATH = "\\RemoteFolders";


        /* ---------------------------------------------------------------- */
        /* ------------------ PROPERTIES ---------------------------------- */
        /* ---------------------------------------------------------------- */

        // files and directories --------------------------------------------------------------
        public String MainDirectoryPath
        {
            get
            {
                string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return docsFolder + MAIN_DIRECTORY_RELATIVE_PATH;
            }
        }

        public String RemoteFoldersPath
        {
            get
            {
                return MainDirectoryPath + REMOTE_FOLDERS_RELATIVE_PATH;
            }
        }


        // status variables --------------------------------------------------------------
        public bool IsInitialized
        {
            get;
            private set;
        }


        // users -----------------------------------------------------------------------
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


        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ USER METHODS --------------------------------------------------------------------- */
        /* ----------------------------------------------------------------------------------------------- */

        /*******************************************************************************************************/
        public void Startup()
        {

            // 1) initialize files and directories ---------------------

            string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!Directory.Exists(MainDirectoryPath))
                Directory.CreateDirectory(MainDirectoryPath);

            if (!Directory.Exists(RemoteFoldersPath))
                Directory.CreateDirectory(RemoteFoldersPath);

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

                Directory.CreateDirectory(RemoteFoldersPath + "\\" + newUser.Username);
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


        /*******************************************************************************************/
        private void LoginUser(User u)
        {
            lock (ConnectedUsers)
            {
                if (ConnectedUsers.Contains(u))
                    throw new FaultException<LoginFault>(new LoginFault(LoginFault.USER_ALREADY_IN));

                ConnectedUsers.Add(u);
            }
        }


        /* ----------------------------------------------------------------------------------------------- */
        /* ------------ FOLDERS METHODS ------------------------------------------------------------------ */
        /* ----------------------------------------------------------------------------------------------- */

        public void AddNewFolder(User user, string folderName)
        {
            if (Directory.Exists(RemoteFoldersPath + "\\" + user.Username + "\\" + folderName))
                throw new FaultException<MyBaseFault>(new MyBaseFault("directory already on server"));

            user.Folders.Add(folderName);
            Directory.CreateDirectory(RemoteFoldersPath + "\\" + user.Username + "\\" + folderName);
        }


        public void addNewFile(User user, string baseFolder, string localPath, FileStream data)
        {
            if (!user.Folders.Contains(baseFolder))
                throw new FaultException<FileTransferFault>(new FileTransferFault(FileTransferFault.UNKNOWN_BASE_FOLDER));

            FileStream inputStream = new FileStream(RemoteFoldersPath + "\\" + user.Username + "\\" + baseFolder + "\\" + localPath, FileMode.Create);
            data.CopyTo(inputStream);
        }
    }




}
