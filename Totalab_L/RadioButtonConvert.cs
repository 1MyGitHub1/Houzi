using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Totalab_L
{
    public class RadioButtonConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;
            string cheackvalue = value.ToString();
            string targetvalue = parameter.ToString();
            bool r = cheackvalue.Equals(targetvalue, StringComparison.InvariantCultureIgnoreCase);
            return r;
            //return value == null ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }
}
