using FolderSynchMUIClient.FolderSynchService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Management;
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
using System.ComponentModel;

namespace FolderSynchMUIClient.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        private static readonly string LOGIN_SUCCESS = "ok";

        public LoginPage()
        {
            InitializeComponent();
            App application = (App)Application.Current;
            TBLoginUsername.ItemsSource = application.KnownUsers.Keys;
        }

        /* ---------------------------------------------------------------- */
        /* ------------ LOGIN CLICK --------------------------------------- */
        /* ---------------------------------------------------------------- */

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += performLogin;
            bw.RunWorkerCompleted += loginCompleted;

            bw.WorkerSupportsCancellation = false;
            bw.WorkerReportsProgress = true;

            bw.RunWorkerAsync();
        }


        /*************************************************************************/
        private void performLogin(object sender, DoWorkEventArgs e)
        {
            try
            {

                // get device HW identifier
                string cpuSerialNumber = "";
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (ManagementObject mo in mos.Get())
                {
                    cpuSerialNumber = (string)mo["ProcessorID"];
                    break;
                }

                bool finished = false;
                // perform login ---------------------------------------------------------------------------
                this.Dispatcher.Invoke((Action)( () =>
                        {
                            App application = (App)Application.Current;
                            application.User = application.FolderSynchProxy.loginUser(  TBLoginUsername.Text.ToString(), 
                                                                                        TBLoginPassword.Password.ToString(), 
                                                                                        cpuSerialNumber);
                            //finished = true;
                        }
                    ));

                //while (!finished) { }
                e.Result = LOGIN_SUCCESS;
            }
            catch (FaultException f)
            {
                e.Result = f.Message;
            }
        }


        /****************************************************************************/
        private void loginCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Visibility = Visibility.Hidden;
            string result = (string)e.Result;

            if (result.Equals(LOGIN_SUCCESS))
            {
                App application = (App)Application.Current;
                Console.WriteLine("user object obtained. FOlders list: ");

                foreach (Folder f in application.User.Folders)
                    Console.WriteLine(f.FolderName + "; synched at: " + f.SynchDate);

                if (CheckBoxRemember.IsChecked.Value)
                    application.AddKnownUser(TBLoginUsername.Text, TBLoginPassword.Password);

                // change window -------------------------------------------------------
                SecondWindow sw = new SecondWindow();
                sw.Show();

                Application.Current.MainWindow.Close();
                Application.Current.MainWindow = sw;
            }
            else
            {
                responseLabel.Content = "error: " + result;
                responseLabel.Foreground = Brushes.Red;
            }
        }


        /* ---------------------------------------------------------------- */
        /* ------------ AUTOCOMPLETE -------------------------------------- */
        /* ---------------------------------------------------------------- */

        private void TBLoginUsername_TextChanged(object sender, RoutedEventArgs e)
        {
            App application = (App)Application.Current;
            if (application.KnownUsers.ContainsKey(TBLoginUsername.Text))
                TBLoginPassword.Password = application.KnownUsers[TBLoginUsername.Text];
            else
                TBLoginPassword.Password = "";

        }
        
    }
}
