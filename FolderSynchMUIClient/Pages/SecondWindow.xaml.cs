﻿using FirstFloor.ModernUI.Windows.Controls;
using FolderSynchMUIClient.FolderSynchService;
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
    /// Interaction logic for SecondWindow.xaml
    /// </summary>
    public partial class SecondWindow : ModernWindow
    {
        public SecondWindow()
        {
            InitializeComponent();

            App application = (App)Application.Current;

            // check if some synced folder is missing on this device -----------------------------------
            List<LocalFolder> localFolders = application.getLocalFolders().ToList();
            List<Folder> missingFolders = new List<Folder>();
            foreach (Folder f in application.User.Folders)
            {
                int found = localFolders.FindIndex(item => item.Name.Equals(f.FolderName));

                if (found == -1)
                    missingFolders.Add(f);
            }

            if (missingFolders.Count > 0)
            {
                LocalFoldersWarningDialog dialog = new LocalFoldersWarningDialog(missingFolders);
                dialog.ShowDialog();
            }
        }
    }
}
