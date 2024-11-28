using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ButtonCustomControl.Converters
{
    internal class TrashTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string trashType)
            {
                switch (trashType)
                {
                    case "KOV":
                        return "Gray";
                    case "PLAST":
                        return "Yellow";
                    case "PAPÍR":
                        return "Blue";
                    case "ELEKTRO":
                        return "Purple";
                    case "PNEU":
                        return "Black";
                    case "SMĚS":
                        return "Pink";
                    case "SKLO":
                        return "White";
                    default:
                        return "Red";
                }
            }

            return "White";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
