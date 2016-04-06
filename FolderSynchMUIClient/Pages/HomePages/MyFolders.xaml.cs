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
using FirstFloor.ModernUI.Presentation;

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
            Console.WriteLine("Prima cartella: " + FolderList[0].Name);         

            Update u = new Update(FolderList[0], DateTime.Now);
            Console.WriteLine("Creo update u");
            

            u.UpdateEntries.Add(new Update.UpdateEntry(new FileItem("File modificato", "path"), 0));
            u.UpdateEntries.Add(new Update.UpdateEntry(new FileItem("Altro file modificato", "path"), 1));
            Console.WriteLine("Aggiungo le update entries");

            FolderList[0].Updates.Add(u);
            Console.WriteLine("Aggiungo l'update a " + FolderList[0].Name);

            foldersButtonControl.ItemsSource = FolderList;
        }

        /*
        private void folderNameButton_Click(object sender, RoutedEventArgs e)
        {
            App application = (App)Application.Current;
            application.Folder = (Folder)(sender as Button).DataContext;
            Console.WriteLine("Selected folder: " + application.Folder.Name + ", path: " + application.Folder.Path);
            Console.WriteLine("Selected folder: " + application.Folder.Name + ", size: " + application.Folder.Size);
        }
        */

        private void foldersButtonControl_changed(object sender, SelectionChangedEventArgs e)
        {
            if (foldersButtonControl.SelectedItem != null)
            {
                App application = (App)Application.Current;
                application.Folder = (Folder)foldersButtonControl.SelectedItem;
                Console.WriteLine("item type: " + foldersButtonControl.SelectedItem.GetType()) ;
                //Console.WriteLine("Selected folder: " + application.Folder.Name + ", path: " + application.Folder.Path);

                FolderTab.DataContext = application.Folder;
            }
        }

       
    }
}
