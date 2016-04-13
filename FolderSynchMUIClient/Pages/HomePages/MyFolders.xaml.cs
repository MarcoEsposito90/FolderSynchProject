using System;
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
using FirstFloor.ModernUI.Presentation;
using FolderSynchMUIClient.FolderSynchService;

namespace FolderSynchMUIClient.Pages.HomePages
{
    /// <summary>
    /// Interaction logic for MyFolders.xaml
    /// </summary>
    public partial class MyFolders : UserControl
    {
        public MyFolders()
        {
            InitializeComponent();
            App application = (App)Application.Current;
            ObservableCollection<LocalFolder> FolderList = new ObservableCollection<LocalFolder>(application.getLocalFolders());
            Console.WriteLine("FolderList created, contains {0} elements", FolderList.Count);

            foldersButtonControl.ItemsSource = application.LocalFolders;

            if (application.LocalFolders.Count > 0)
                foldersButtonControl.SelectedItem = application.LocalFolders[0];
        }

        private void foldersButtonControl_changed(object sender, SelectionChangedEventArgs e)
        {
            if (foldersButtonControl.SelectedItem != null)
            {
                App application = (App)Application.Current;
                
                application.Folder = (LocalFolder)foldersButtonControl.SelectedItem;
                Console.WriteLine("item type: " + foldersButtonControl.SelectedItem.GetType()) ;
                //Console.WriteLine("Selected folder: " + application.Folder.Name + ", path: " + application.Folder.Path);

                FolderTab.DataContext = application.Folder;
            }
        }

       
    }
}
