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
using ServicesProject;

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
            /*
            ItemProvider itemProvider = new ItemProvider();

            App application = (App)Application.Current;
            if(application.Folder != null) { 
                 ObservableCollection<Item> ItemsList = itemProvider.GetItems(application.Folder.Path);
                trvFolders.ItemsSource = ItemsList;
                
            }*/
        }
    }
}