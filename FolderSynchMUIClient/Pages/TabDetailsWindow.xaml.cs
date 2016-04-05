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
    /// Interaction logic for TabDetailsWindow.xaml
    /// </summary>
    public partial class TabDetailsWindow : UserControl
    {
        public TabDetailsWindow()
        {
            InitializeComponent();

            App application = (App)Application.Current;
            if (application.Folder != null)
            {
                TBFolderName.Text = application.Folder.Name;
                TBFolderDetails_Path.Text = application.Folder.Path;
                TBFolderDetails_Size.Text = application.Folder.SizeInBytes;
                TBFolderDetails_NumFiles.Text = (application.Folder.ContainedFiles.ToString() + " files, "+ application.Folder.ContainedFolders.ToString() + " folders");
            }
        }
    }
}
