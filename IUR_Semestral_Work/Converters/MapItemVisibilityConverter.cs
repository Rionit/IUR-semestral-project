using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IUR_Semestral_Work.Converters
{
    public class MapItemVisibilityConverter : IMultiValueConverter
    {
        enum Filter { KOV, PLAST, PNEU, ELEKTRO, SMĚS, SKLO, PAPÍR};

        private bool isTypeFiltered(string type, bool[] filters)
        {
            switch (type)
            {
                case "KOV": return filters[(int)Filter.KOV] ? true : false;
                case "PLAST": return filters[(int)Filter.PLAST] ? true : false;
                case "PNEU": return filters[(int)Filter.PNEU] ? true : false;
                case "ELEKTRO": return filters[(int)Filter.ELEKTRO] ? true : false;
                case "SMĚS": return filters[(int)Filter.SMĚS] ? true : false;
                case "SKLO": return filters[(int)Filter.SKLO] ? true : false;
                case "PAPÍR": return filters[(int)Filter.PAPÍR] ? true : false;
                default: return false;
            }
        }

        private bool isDateFiltered(DateTime date, DateTime filterStart, DateTime filterEnd)
        {
            if (date >= filterStart && date <= filterEnd) return true;
            else return false;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 14 && values[0] is bool isAddedTrashFiltered && values[1] is bool isRemovedTrashFiltered && values[2] is bool isPicked
                && values[3] is string pinType && values[4] is bool kovFilter && values[5] is bool plastFilter && values[6] is bool pneuFilter && values[7] is bool elektroFilter && values[8] is bool smesFilter && values[9] is bool glassFilter && values[10] is bool paperFilter)
            {
                bool[] filters = [kovFilter, plastFilter, pneuFilter, elektroFilter, smesFilter, glassFilter, paperFilter];

                DateTime dateFilterStart = values[11] == null ? DateTime.MinValue : ((DateTime)values[11]).Date; // Remove time cause we want the date at 00:00
                DateTime dateFilterEnd = values[12] == null ? DateTime.MaxValue : ((DateTime)values[12]).Date.AddDays(1).AddMilliseconds(-1); // Change time to 23:59
                DateTime pinDate = values[13] == null ? DateTime.Now : (DateTime)values[13];

                if(isTypeFiltered(pinType, filters) && isDateFiltered(pinDate, dateFilterStart, dateFilterEnd))
                {
                    if (isRemovedTrashFiltered && isPicked)
                    {
                        return Visibility.Visible;
                    }
                    else if (isAddedTrashFiltered && !isPicked)
                    {
                        return Visibility.Visible;
                    }
                }
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
