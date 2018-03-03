using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using Ai.Hong.Common.Extenstion;

//转换接口

namespace Ai.Hong.Common.Convert
{
    /// <summary>
    /// Object!=NULL, 返回True, 否则:False
    /// </summary>
    [ValueConversion(typeof(bool), typeof(object))]
    public class DisableNullConvert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// True到False转换
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class NotTrueConvert : IValueConverter
    {
        /// <summary>
        /// Not true ==>false
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value != true;
        }

        /// <summary>
        /// Not true ==> false
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value != true;
        }
    }

    /// <summary>
    /// 获取DataGrid的Item所在的行,用于绑定行号
    /// </summary>
    [ValueConversion(typeof(int), typeof(System.Windows.Controls.DataGridRow))]
    public class RowNumberConverter : IValueConverter
    {
        /// <summary>
        /// Row number
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Controls.DataGridRow row = value as System.Windows.Controls.DataGridRow;
            if (row == null)
                return 0;

            return row.GetIndex() + 1;
        }

        /// <summary>
        /// ConvertBack
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 如果选择的列表值为设定值，返回Visble,否则返回Collapsed, values[0]=列表选择值, values[1]=比较值
    /// </summary>
    [ValueConversion(typeof(Visibility), typeof(int[]))]
    public class SelectedIndexVisibleConvert : IMultiValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)values[0] == (int)values[1] ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// BOOL值与0,1之间的转换: False=0, True=1
    /// </summary>
    [ValueConversion(typeof(int), typeof(bool))]
    public class BoolIntConvert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? 1 : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value == 0 ? false : true;
        }
    }

    /// <summary>
    /// 颜色和画笔之间的转换
    /// </summary>
    [ValueConversion(typeof(Color), typeof(Brush))]
    public class BrushToColorConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            if (value == null)
                return default(Color);
            else
                return brush.Color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
            {
                return null;
            }
            // For a more sophisticated converter, check also the targetType and react accordingly..
            if (value is Color)
            {
                Color color = (Color)value;
                return new SolidColorBrush(color);
            }
            else
                return null;
        }
    }

    /// <summary>
    /// 预测值与参考值的偏差，如果parameter=true,返回绝对偏差，否则返回相对偏差
    /// </summary>
    [ValueConversion(typeof(double), typeof(double[]))]
    public class DifferentValueConvert : IMultiValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)parameter)
                return (double)values[1] - (double)values[0];
            else
                return ((double)values[1] - (double)values[0]) / (double)values[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Nuallable BOOL值与0,1, 2之间的转换: False=0, True=1, NULL=2
    /// </summary>
    [ValueConversion(typeof(int), typeof(int))]
    public class ValueQueryConditionConvert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? state = (bool?)value;
            if (state == null)
                return 3;
            else if (state == true)
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int state = (int)value;
            if (state == 0)
                return false;
            else if (state == 1)
                return true;
            else
                return null;
        }
    }

    /// <summary>
    /// 颜色和画笔之间的转换
    /// </summary>
    [ValueConversion(typeof(Brush), typeof(Color))]
    public class ColorToBrushConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
            {
                return null;
            }
            // For a more sophisticated converter, check also the targetType and react accordingly..
            if (value is Color)
            {
                Color color = (Color)value;
                return new SolidColorBrush(color);
            }
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            if (value == null)
                return default(Color);
            else
                return brush.Color;
        }
    }

    /// <summary>
    /// double与显示间的转换(两位小数)
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleStringF2Convert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            double temp = (double)value;
            return double.IsNaN(temp) ? null : temp.ToString("F2");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace((string)value))
                return double.NaN;

            double data = double.NaN;
            if (double.TryParse((string)value, out data) == false)
                return double.NaN;
            else
                return data;
        }
    }

    /// <summary>
    /// float与显示间的转换(两位小数)
    /// </summary>
    [ValueConversion(typeof(float), typeof(string))]
    public class FloatStringF2Convert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            float temp = (float)value;
            return float.IsNaN(temp) ? null : temp.ToString("F2");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace((string)value))
                return float.NaN;

            float data = float.NaN;
            if (float.TryParse((string)value, out data) == false)
                return float.NaN;
            else
                return data;
        }
    }

    /// <summary>
    /// BOOL值与Y之间的转换: False=null, True='Y'
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToYesConvert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            return (bool)value ? "Y" : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Orientation值与VerticalAlignment之间的转换
    /// </summary>
    [ValueConversion(typeof(VerticalAlignment), typeof(Orientation))]
    public class VerticalConvert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Horizontal :居中, Vertical:上对齐
            return (Orientation)value == Orientation.Horizontal ? VerticalAlignment.Center : VerticalAlignment.Top;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Orientation.Horizontal;
        }
    }


    /// <summary>
    /// Orientation值与VerticalAlignment之间的转换
    /// </summary>
    [ValueConversion(typeof(HorizontalAlignment), typeof(Orientation))]
    public class HorizontalConvert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Horizontal :左对齐, Vertical:居中
            return (Orientation)value == Orientation.Horizontal ? HorizontalAlignment.Left : HorizontalAlignment.Center;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Orientation.Horizontal;
        }
    }

    /// <summary>
    /// 将String指定的字体名称转换为字体类
    /// </summary>
    [ValueConversion(typeof(string), typeof(FontFamily))]
    public class StringToFontFamily : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FontFamily fontfamily = (FontFamily)value;
            LanguageSpecificStringDictionary lsd = fontfamily.FamilyNames;

            if (lsd.ContainsKey(System.Windows.Markup.XmlLanguage.GetLanguage("zh-cn")))
            {
                string fontname = null;
                if (lsd.TryGetValue(System.Windows.Markup.XmlLanguage.GetLanguage("zh-cn"), out fontname))
                {
                    return fontname;
                }
            }
            else
            {
                string fontname = null;
                if (lsd.TryGetValue(System.Windows.Markup.XmlLanguage.GetLanguage("en-us"), out fontname))
                {
                    return fontname;
                }
            }
            return "Arial";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string fontname = (string)value;
            FontFamily fontfamily = new FontFamily(fontname);
            return fontfamily;
        }
    }
    /// <summary>
    /// Checked与Visibility之间的转换, True=Visible, False=Collapsed
    /// </summary>
    [ValueConversion(typeof(Visibility), typeof(bool))]
    public class TrueIsVisibilityConvert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value == true) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }


    /// <summary>
    /// False与Visibility之间的转换, False=Visible, True=Collapsed
    /// </summary>
    [ValueConversion(typeof(Visibility), typeof(bool))]
    public class FalseIsVisibilityConvert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value == true) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value != Visibility.Visible;
        }
    }

    /// <summary>
    /// 计算treeviewItem的层级,使用TreeViewItemExtensions
    /// </summary>
    [ValueConversion(typeof(TreeViewItem), typeof(Thickness))]
    public class IndentConverter : IValueConverter
    {
        /// <summary>
        /// Item的层级
        /// </summary>
        public double Indent { get; set; }

        /// <summary>
        /// 当前Item左边的左边的Margin
        /// </summary>
        public double MarginLeft { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var item = value as TreeViewItem;
            if (null == item)
                return new Thickness(0);
            return new Thickness(this.MarginLeft + this.Indent * item.GetDepth(), 0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 日期格式与字符串yyyy-MM-dd之间的转换
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateConverter : IValueConverter      //将日期转换到字符串yyyy-MM-dd
    {
        /// <summary>
        /// Datetime ==> string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// String ==> datetime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value as string;
            DateTime resultDateTime;
            if (DateTime.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// 时间格式与字符串HH:mm:ss之间的转换
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class TimeConverter : IValueConverter      //将时间转换到字符串HH:mm:ss
    {
        /// <summary>
        /// Datetime ==> string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.ToString("HH:mm:ss");
        }

        /// <summary>
        /// String ==> datetime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value as string;
            DateTime resultDateTime;
            if (DateTime.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
