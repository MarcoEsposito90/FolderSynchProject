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
using FolderSynchService;

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
           List<Item> ItemsList = new List<Item>();

           Folder folder1 = new Folder() { Name = "Folder1", Path="C://" };
           folder1.Items.Add(new Folder() { Name = "Folder1a", Path = "C://Folder1" });
           Folder folder2a = new Folder() { Name = "Folder2a", Path = "C://Folder1" };
           folder2a.Items.Add(new FileItem() { Name = "File2", Path = "C://Folder1/Folder2a" });
           folder1.Items.Add(folder2a);
           folder1.Items.Add(new FileItem() { Name = "File1", Path = "C://Folder1" });
           ItemsList.Add(folder1);

           Folder folder2 = new Folder() { Name = "Folder2", Path = "C://" };
           folder1.Items.Add(new Folder() { Name = "Folder2a", Path = "C://Folder1" });
           folder1.Items.Add(new FileItem() { Name = "File2", Path = "C://Folder1" });
           ItemsList.Add(folder2);

           trvFolders.ItemsSource = ItemsList;*/

            ItemProvider itemProvider = new ItemProvider();

            App application = (App)Application.Current;
            if(application.Folder != null) { 
                 ObservableCollection<Item> ItemsList = itemProvider.GetItems(application.Folder.Path);
                trvFolders.ItemsSource = ItemsList;
            }
            //DataContext = ItemsList;
            
            
        }
    }
}
