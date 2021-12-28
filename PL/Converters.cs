using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PL
{
    public class AddConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int sum = 0;
            Double val;
            foreach (var value in values)
            {
                bool isNum = Double.TryParse(value.ToString(), out val) && !Double.IsNaN(val);
                sum += (int)val;
            }

            return sum.ToString(culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
