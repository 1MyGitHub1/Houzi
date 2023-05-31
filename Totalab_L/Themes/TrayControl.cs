using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Totalab_L.Themes
{
    public class TrayControl : ItemsControl
    {
        static TrayControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TrayControl), new FrameworkPropertyMetadata(typeof(TrayControl)));
        }

        /// <summary>
        /// 数据模板中数据个数
        /// </summary>
        public int ItemsCount
        {
            get { return (int)GetValue(ItemsCountProperty); }
            set { SetValue(ItemsCountProperty, value); }
        }
        public static readonly DependencyProperty ItemsCountProperty = DependencyProperty.Register(
            nameof(ItemsCount), typeof(int), typeof(TrayControl), new PropertyMetadata(20));

        ///<summary>
        ///模板中行的Item个数
        ///</summary>
        public int XCount
        {
            get { return (int)GetValue(XCountProperty); }
            set { SetValue(XCountProperty, value); }
        }
        public static readonly DependencyProperty XCountProperty = DependencyProperty.Register(
            nameof(XCount), typeof(int), typeof(TrayControl), new PropertyMetadata(20));

        ///<模板中列的Item个数
        public int YCount
        {
            get { return (int)GetValue(YCountProperty); }
            set { SetValue(YCountProperty, value); }
        }
        public static readonly DependencyProperty YCountProperty = DependencyProperty.Register(
            nameof(YCount), typeof(int), typeof(TrayControl), new PropertyMetadata(20));


        public Size ItemsSize
        {
            get { return (Size)GetValue(ItemsSizeProperty); }
            set { SetValue(ItemsSizeProperty, value); }
        }
        public static readonly DependencyProperty ItemsSizeProperty = DependencyProperty.Register(
         nameof(ItemsSize), typeof(Size), typeof(TrayControl), new PropertyMetadata(new Size(18, 18)));
    }
}
