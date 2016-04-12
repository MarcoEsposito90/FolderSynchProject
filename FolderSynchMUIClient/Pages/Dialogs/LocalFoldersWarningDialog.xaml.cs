﻿using FirstFloor.ModernUI.Windows.Controls;
using ServicesProject;
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
    /// Interaction logic for LocalFoldersWarningDialog.xaml
    /// </summary>
    public partial class LocalFoldersWarningDialog : ModernDialog
    {


        public LocalFoldersWarningDialog(List<Folder> folders)
        {
            InitializeComponent();

            TBMessage.Text = "The folders you are missing on this device:\n";
            foreach (Folder f in folders)
                TBMessage.Text += f.Name + "\n";

            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton };
        }
    }
}