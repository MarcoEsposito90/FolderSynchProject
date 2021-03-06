﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for TabBrowseWindow.xaml
    /// </summary>
    public partial class TabBrowseWindow : UserControl
    {
        public TabBrowseWindow()
        {
            InitializeComponent();
        }

        private void treeViewItem_Click(object sender, RoutedEventArgs e)
        {
            //Showing dialog with selected folder/file details
            if(trvFolders.SelectedItem.GetType() == typeof(FileItem))
            {
                FileItem it = trvFolders.SelectedItem as FileItem;
                DetailsDialog detailsDialog = new DetailsDialog((LocalFolder)this.DataContext, it);
                detailsDialog.DataContext = it;
                detailsDialog.ShowDialog();
            }
            

        }

        
    }
}
