using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchService
{
    [ServiceBehavior(   InstanceContextMode = InstanceContextMode.PerSession,
                        ConcurrencyMode = ConcurrencyMode.Single)]

    public class FolderSynchServiceImplementation : FolderSynchServiceContract
    {

        public User loginUser(string username, string password)
        {
            Console.WriteLine("login called on service instance = " + this.ToString());
            return FolderSynchServer.Instance.LoginUser(username, password);
        }

        public void logoutUser(User user)
        {
            Console.WriteLine("logout called on service instance = " + this.ToString());
            FolderSynchServer.Instance.logoutUser(user);
        }

        public void RegisterNewUser(string username, string password)
        {
            Console.WriteLine("register called on service instance = " + this.ToString());
            FolderSynchServer.Instance.registerNewUser(username, password);
        }

    }
}
