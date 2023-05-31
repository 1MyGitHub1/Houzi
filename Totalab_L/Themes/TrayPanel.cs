using LabTech.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Totalab_L.Themes
{
    public class TrayPanel:Panel
    {
        /// <summary>
        /// 数据模板中元素个数
        /// </summary>
        public int ItemsCount
        {
            get { return (int)GetValue(ItemCountProperty); }
            set { SetValue(ItemCountProperty, value); }
        }
        public static readonly DependencyProperty ItemCountProperty = DependencyProperty.Register(
            nameof(ItemsCount), typeof(int), typeof(TrayPanel), new PropertyMetadata(20, OnItemCountChanged));

        private static void OnItemCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrayPanel sampleInforPanel && sampleInforPanel != null)
                sampleInforPanel.InvalidateArrange();
        }


        /// <summary>
        /// 模板中元素的大小
        /// </summary>
        public Size ItemsSize
        {
            get { return (Size)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }

        }
        public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register(
         nameof(ItemsSize), typeof(Size), typeof(TrayPanel), new PropertyMetadata(new Size(32, 32), OnItemSizeChanged));

        private static void OnItemSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrayPanel sampleInforPanel && sampleInforPanel != null)
                sampleInforPanel.InvalidateArrange();
        }


        ///<summary>
        ///模板中行的Item个数
        ///</summary>
        public int XCount
        {
            get { return (int)GetValue(XCountProperty); }
            set { SetValue(XCountProperty, value); }
        }
        public static readonly DependencyProperty XCountProperty = DependencyProperty.Register(
            nameof(XCount), typeof(int), typeof(TrayPanel), new PropertyMetadata(12, OnXCountChanged));

        private static void OnXCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrayPanel sampleInforPanel && sampleInforPanel != null)
                sampleInforPanel.InvalidateArrange();
        }

        ///<模板中列的Item个数
        public int YCount
        {
            get { return (int)GetValue(YCountProperty); }
            set { SetValue(YCountProperty, value); }
        }
        public static readonly DependencyProperty YCountProperty = DependencyProperty.Register(
            nameof(YCount), typeof(int), typeof(TrayPanel), new PropertyMetadata(5, OnYCountChanged));

        private static void OnYCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrayPanel sampleInforPanel && sampleInforPanel != null)
                sampleInforPanel.InvalidateArrange();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i] is UIElement child && child != null)
                {
                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            try
            {
                if (Children != null && Children.Count > 0)
                {
                    double rowWidth = finalSize.Width;
                    double rowHeight = finalSize.Height / YCount;
                    double cellWidth = finalSize.Width / XCount;
                    double cellHeight = rowHeight;
                    //ItemsSize = new Size((int)Math.Min(cellWidth, cellHeight), (int)Math.Min(cellWidth, cellHeight));

                    for (int childIndex = 0; childIndex < Children.Count; childIndex++)
                    {

                        if (Children[childIndex] is ContentPresenter child && child != null)
                        {
                            Point childPosition = new Point(0, 0);
                            childPosition = new Point(cellWidth * (childIndex % XCount) + (cellWidth / 2 - ItemsSize.Width / 2), rowHeight * (childIndex / XCount) + (cellHeight - ItemsSize.Height) / 2);
                            //if (childIndex / XCount % 2 == 0)///偶数行,从左到右
                            //{
                            //    childPosition = new Point(cellWidth * (childIndex % XCount) + (cellWidth / 2 - ItemsSize.Width / 2), rowHeight * (childIndex / XCount) + (cellHeight - ItemsSize.Height)/2);
                            //}
                            //if (childIndex / XCount % 2 == 1)
                            //{
                            //    childPosition = new Point((cellWidth * ((XCount-1) - childIndex % XCount) + (cellWidth / 2 - ItemsSize.Width / 2)), rowHeight * (childIndex / XCount) + (cellHeight - ItemsSize.Height)/2);
                            //}
                            child.Arrange(new Rect(childPosition, ItemsSize));
                        }
                    }
                    }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("TrayPanel [ArrangeOverride]：", ex);
            }
            return finalSize;
        }
    }
}
