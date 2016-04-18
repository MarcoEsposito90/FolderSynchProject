using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FolderSynchMUIClient
{
    // Custom class implements the IValueConverter interface.
    public class TypeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intType = (int)value;
            string strType;
            switch (intType)
            {
                case 0:
                    strType = "New file created: ";
                    break;
                case 1:
                    strType = "Modified file ";
                    break;
                case 2:
                    strType = "Created directory ";
                    break;
                case 3:
                    strType = "Deleted directory ";
                    break;
                case 4:
                    strType = "Deleted file ";
                    break;
                default:
                    strType = "";
                    break;
            }
            
            return strType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //No need for implementation 
            throw new NotImplementedException();
        }
        
        
    }
}
