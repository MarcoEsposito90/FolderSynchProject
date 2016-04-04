using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FolderSynchMUIClient.FolderSynchService;

namespace FolderSynchMUIClient.Pages
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : UserControl
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            if (TBRegisterPassword.Password.Equals(TBRegisterConfirmPassword.Password))
            {
                Console.WriteLine("Registering user: " + TBRegisterUsername.Text.ToString());

                try
                {
                    App application = (App)Application.Current;
                    FolderSynchServiceContractClient proxy = application.FolderSynchProxy;

                    application.User = proxy.RegisterNewUser(TBRegisterUsername.Text.ToString(), TBRegisterPassword.Password.ToString());
                    ResponseLabel.Content = "Registration successful";
                }
                catch(FaultException f)
                {

                    ResponseLabel.Content = "Error: " + f.Message;
                }

            }
            else
            {
                ResponseLabel.Content = "the passwords don't match";
            }


        }
    }
}
