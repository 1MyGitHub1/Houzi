using LabTech.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Totalab_L.Common
{
    [ValueConversion(typeof(object), typeof(SolidColorBrush))]
    public class CountToDGRowBackConverter : IValueConverter
    {
        public string TrueColorVal { get; set; }
        public string FalseColorVal { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is int val)
                {
                    if (val % 2 == 0)
                    {
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(TrueColorVal));
                    }
                    else
                    {
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(FalseColorVal));
                    }
                }
            }
            catch (Exception e)
            {
                MainLogHelper.Instance.Error("CountToDGRowBackConverter [Convert]出错", e);
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(Colors.White);
        }
    }
}