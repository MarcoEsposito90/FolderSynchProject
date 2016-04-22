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
    /// Interaction logic for ModernDialog1.xaml
    /// </summary>
    public partial class ErrorDialog : ModernDialog
    {
        string errorMessage;
        public ErrorDialog(string errorMessage)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            this.errorMessage = errorMessage;
            this.Buttons = new Button[] { this.OkButton };
        }

        private void ModernDialog_ContentRendered(object sender, EventArgs e)
        {
            txtFaultReason.Text = errorMessage;
        }
    }
}
