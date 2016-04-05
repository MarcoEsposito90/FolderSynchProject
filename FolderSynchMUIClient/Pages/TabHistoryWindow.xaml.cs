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

namespace FolderSynchMUIClient.Pages
{
    /// <summary>
    /// Interaction logic for TabHistoryWindow.xaml
    /// </summary>
    public partial class TabHistoryWindow : UserControl
    {
        public TabHistoryWindow()
        {
            InitializeComponent();
           
            ObservableCollection<Folder> FolderList = new ObservableCollection<Folder>();

            Folder f1 = new Folder("Folder1", "xxx");
            Folder f2 = new Folder("Folder2", "yyy");
            Folder f3 = new Folder("Folder3", "zzz");

            FolderList.Add(f1);
            FolderList.Add(f2);
            FolderList.Add(f3);

            folderEditDates.ItemsSource = FolderList;
        }
    }
}
