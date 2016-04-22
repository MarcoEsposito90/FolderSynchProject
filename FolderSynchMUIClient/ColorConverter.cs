using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FolderSynchMUIClient
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intType = (int)value;
            Brush typeColor;
            switch (intType)
            {
                case 0:
                    typeColor = Brushes.Green;
                    break;
                case 1:
                    typeColor = Brushes.Orange;
                    break;
                case 2:
                    typeColor = Brushes.Green;
                    break;
                case 3:
                    typeColor = Brushes.Red;
                    break;
                case 4:
                    typeColor = Brushes.Red;
                    break;
                default:
                    typeColor = Brushes.Black;
                    break;
            }

            return typeColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
