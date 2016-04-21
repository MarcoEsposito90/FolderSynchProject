using FirstFloor.ModernUI.Windows.Controls;
using FolderSynchMUIClient.FolderSynchService;
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

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for ChangePasswordDialog.xaml
    /// </summary>
    public partial class ChangePasswordDialog : ModernDialog
    {
        public ChangePasswordDialog()
        {
            InitializeComponent();

            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
            OkButton.Visibility = Visibility.Hidden;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            App application = (App)Application.Current;

            if (TBOldPassword.Password.Equals(application.User.Password)) { 
                if (TBNewPassword.Password.Equals(TBRepeatPassword.Password))
                {
                    string specialChars = "{}[]()|;\\";
                    string pwd = TBNewPassword.Password.ToString();

                    if (pwd.Contains(";") || pwd.Contains("|") || pwd.Contains("(") || pwd.Contains(")")
                        || pwd.Contains("[") || pwd.Contains("]") || pwd.Contains("{") || pwd.Contains("}") || pwd.Contains("\\"))
                    {
                        responseLabel.Content = "Charcaters: " + specialChars.ToString() + " not allowed.";
                        responseLabel.Foreground = Brushes.Red;
                        
                    }

                    else {
                        try
                        {
                            FolderSynchServiceContractClient proxy = application.FolderSynchProxy;
                            proxy.changeCredentials(TBOldPassword.Password, TBNewPassword.Password);
                            responseLabel.Content = "Password changed";
                            responseLabel.Foreground = Brushes.Green;
                            OkButton.Visibility = Visibility.Visible;
                            CancelButton.IsEnabled = false;
                            
                        }
                        catch (FaultException f)
                        {
                            ErrorDialog ed = new ErrorDialog();
                            ed.txtFaultTitle.Text = "Error:";
                            ed.txtFaultReason.Text = f.Message;
                            ed.ShowDialog();
                        }

                    }
                }
                else
                {
                    responseLabel.Content = "Mismatching password";
                    responseLabel.Foreground = Brushes.Red;
                }
            }
            else
            {
                responseLabel.Content = "Your old password is wrong";
                responseLabel.Foreground = Brushes.Red;
            }
        }
    }
}
