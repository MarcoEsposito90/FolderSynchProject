using FirstFloor.ModernUI.Windows.Controls;
using FolderSynchMUIClient.FolderSynchService;
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
    /// Interaction logic for RollbackConfirmDialog.xaml
    /// </summary>
    public partial class RollbackConfirmDialog : ModernDialog
    {

        public enum Option
        {
            DeleteCurrent,
            KeepCurrent,
            SimpleDownload
        }

        private static readonly string[] messages =
        {
            "If you choose yes, the current folder will be deleted and replaced with its previous version, and you will loose all changes after: ",
            "If you choose yes, the current folder will be replaced with its previous version, but you will still have a copy of files. However, you will loose changes after: ",
            "If you choose yes, you will download a copy of the previous version of the folder. Nothing will be deleted or replaced"
        };


        private Option option;
        private Update update;


        public RollbackConfirmDialog(Option option, Update update)
        {
            InitializeComponent();

            this.option = option;
            this.update = update;
            Owner = Application.Current.MainWindow;
            // define the dialog buttons
            this.Buttons = new Button[] { this.YesButton, this.NoButton};
        }

        private void ModernDialog_ContentRendered(object sender, EventArgs e)
        {

            switch (option)
            {
                // ----------------------------------------
                case Option.DeleteCurrent:

                    warningMessageTB.Text = messages[0] + update.Timestamp.ToString();
                    break;

                // ----------------------------------------
                case Option.KeepCurrent:

                    warningMessageTB.Text = messages[1] + update.Timestamp.ToString();
                    break;
                    
                // ----------------------------------------
                case Option.SimpleDownload:

                    warningMessageTB.Text = messages[2];
                    break;
            }

        }
    }
}
