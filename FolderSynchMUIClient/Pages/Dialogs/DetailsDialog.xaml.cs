using FirstFloor.ModernUI.Windows.Controls;
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

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for DetailsDialog.xaml
    /// </summary>
    public partial class DetailsDialog : ModernDialog
    {

        public DetailsDialog(LocalFolder lf, Item it)
        {
            InitializeComponent();

            tabDetails.DataContext = it;
            Console.WriteLine("DataContext: " + it.Name);
            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton};
        }
    }
}
