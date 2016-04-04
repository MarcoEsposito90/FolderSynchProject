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
            ItemProvider itemProvider = new ItemProvider();
            
            ObservableCollection<Folder> FolderList = itemProvider.GetFolders("C:\\Users\\Giulia Genta\\Desktop");

            foldersButtonControl.ItemsSource = FolderList;
        }

        private void folderNameButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
