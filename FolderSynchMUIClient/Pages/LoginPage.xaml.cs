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

namespace FolderSynchMUIClient.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.currentUser = App.FolderSynchProxy.loginUser(TBLoginUsername.Text.ToString(), TBLoginPassword.Password.ToString());
                responseLabel.Content = "login successful";
            }
            catch(FaultException f)
            {
                responseLabel.Content = "error: " + f.Message;
            }
        }
    }
}
