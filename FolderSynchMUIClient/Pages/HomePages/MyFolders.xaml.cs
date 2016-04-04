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

            ObservableCollection<Folder> FolderList = new ObservableCollection<Folder>();

            Folder f1 = new Folder() { Name = "Folder1", Path = "xxx" };
            Folder f2 = new Folder() { Name = "Folder2", Path = "yyy" };
            Folder f3 = new Folder() { Name = "Folder2", Path = "zzz" };

            FolderList.Add(f1);
            FolderList.Add(f2);
            FolderList.Add(f3);

            foldersButtonControl.ItemsSource = FolderList;
        }

        private void folderNameButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
