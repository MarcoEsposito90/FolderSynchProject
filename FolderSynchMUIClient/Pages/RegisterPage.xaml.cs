using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine("Registering user: " + TBRegisterUsername.Text.ToString());
            bool success = App.FolderSynchProxy.RegisterNewUser(TBRegisterUsername.Text.ToString(), TBRegisterPassword.Text.ToString());

            Console.WriteLine("registration answered: " + success);
        }
    }
}
