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
using System.Text.RegularExpressions;
using System.Management;

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
                string specialChars = "{}[]()|;\\";
                string pwd = TBRegisterPassword.Password.ToString();

                if (pwd.Contains(";") || pwd.Contains("|") || pwd.Contains("(") || pwd.Contains(")") 
                    || pwd.Contains("[") || pwd.Contains("]") || pwd.Contains("{") || pwd.Contains("}") || pwd.Contains("\\"))
                {
                    ErrorDialog ed = new ErrorDialog("Invalid password\nYour password should not contain one of these special charcaters: " + 
                                                     specialChars.ToString());
                    ed.ShowDialog();
                }

                else {
                    try
                    {
                        App application = (App)Application.Current;
                        FolderSynchServiceContractClient proxy = application.FolderSynchProxy;

                        // get device HW identifier ----------------------------------------------------------------
                        string cpuSerialNumber = "";
                        ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                        foreach (ManagementObject mo in mos.Get())
                        {
                            cpuSerialNumber = (string)mo["ProcessorID"];
                            break;
                        }

                        // perform registration --------------------------------------------------------------------
                        application.User = proxy.RegisterNewUser(TBRegisterUsername.Text.ToString(), TBRegisterPassword.Password.ToString(), Environment.MachineName);
                        ResponseLabel.Content = "Registration successful";
                        ResponseLabel.Foreground = Brushes.Green;

                        // change window ---------------------------------------------------------------------------
                        SecondWindow sw = new SecondWindow();
                        sw.Show();

                        Application.Current.MainWindow.Close();
                        Application.Current.MainWindow = sw;

                    }
                    catch (FaultException f)
                    {
                        ResponseLabel.Content = "Error: " + f.Message;
                        ResponseLabel.Foreground = Brushes.Red;
                    }

                }
            }
            else
            {
                ResponseLabel.Content = "the passwords don't match";
            }


        }
    }
}
