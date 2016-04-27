﻿using FolderSynchMUIClient.FolderSynchService;
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
            App application = (App)Application.Current;
            TBLoginUsername.ItemsSource = application.KnownUsers.Keys;
        }


        /* ---------------------------------------------------------------- */
        /* ------------ LOGIN CLICK --------------------------------------- */
        /* ---------------------------------------------------------------- */

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // perform login ---------------------------------------------------------------------------
                App application = (App)Application.Current;
                FolderSynchServiceContractClient proxy = application.FolderSynchProxy;

                application.User = proxy.loginUser(TBLoginUsername.Text.ToString(), TBLoginPassword.Password.ToString());
                Console.WriteLine("user object obtained. FOlders list: ");

                foreach (Folder f in application.User.Folders)
                    Console.WriteLine(f.FolderName + "; synched at: " + f.SynchDate);

                if (CheckBoxRemember.IsChecked.Value)
                    application.AddKnownUser(TBLoginUsername.Text, TBLoginPassword.Password);

                // change window ---------------------------------------------------------------------------
                SecondWindow sw = new SecondWindow();
                sw.Show();

                Application.Current.MainWindow.Close();
                Application.Current.MainWindow = sw;

            }
            catch (FaultException f)
            {
                responseLabel.Content = "error: " + f.Message;
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
