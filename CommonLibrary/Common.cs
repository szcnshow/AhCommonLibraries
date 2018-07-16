using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Data;

namespace Ai.Hong.Common
{
    /// <summary>
    /// 选择消息参数结构
    /// </summary>
    public class SelectChangedArgs : RoutedEventArgs
    {
        /// <summary>
        /// 内容
        /// </summary>
        public object item { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="item"></param>
        public SelectChangedArgs(RoutedEvent routedEvent, object item)
        {
            this.RoutedEvent = routedEvent;
            this.item = item;
        }
    }

    /// <summary>
    /// Checked与Visibility之间的转换, True=Visible, False=Collapsed
    /// </summary>
    [ValueConversion(typeof(Visibility), typeof(bool))]
    public class CheckedVisibilityConvert : IValueConverter
    {
        /// <summary>
        /// Checked to visibility
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool?)value == true) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// visibility to checked
        /// </summary>
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
        /// False to visibility
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool?)value == false) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Visibility to false
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
    /// False与Visibility之间的转换, False=Visible, True=Collapsed
    /// </summary>
    [ValueConversion(typeof(bool), typeof(int))]
    public class BoolToIntConvert : IValueConverter
    {
        /// <summary>
        /// True/False ==>1/0
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value == false) ? 0 : 1;
        }

        /// <summary>
        /// 1/0 ==> True/False
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value != 0;
        }
    }

    /// <summary>
    /// 文件名与BitmapImage之间的转换
    /// </summary>
    [ValueConversion(typeof(string), typeof(System.Windows.Media.Imaging.BitmapImage))]
    public class ImageConvert : IValueConverter     //文件名转换为图像类型
    {
        /// <summary>
        /// Filename ==> image
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

            string filename = (string)value;
            return new System.Windows.Media.Imaging.BitmapImage(new Uri(filename));
        }
        /// <summary>
        /// Image ==> filename
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            System.Windows.Media.Imaging.BitmapImage img = value as System.Windows.Media.Imaging.BitmapImage;
            return Path.GetFileName(img.UriSource.AbsoluteUri);
        }
    }

    /// <summary>
    /// 时间格式与字符串yyyy-MM-dd HH:mm:ss之间的转换
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateTimeConverter : IValueConverter      //将时间转换到字符串yyyy-MM-dd HH:mm:ss
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
            return date.ToString("yyyy-MM-dd HH:mm:ss");
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
    /// 数值与字符串之间的转换，保持2位小数，如果数值小于等于0，显示为N
    /// </summary>
    [ValueConversion(typeof(double), typeof(String))]
    public class PositiveValueConverter : IValueConverter
    {
        /// <summary>
        /// Positive value ==> N
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double number = (double)value;
            if (number <= 0)
                return "N";

            number = Math.Round(number * 100) / 100;    //这里可以保证2位小数，或者没有小数
            return number.ToString();
        }

        /// <summary>
        /// N ==> positive value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value as string;
            double result;
            if (double.TryParse(strValue, out result) == true)
                return result;

            return 0;  //可能是'N'，表示小于0
        }
    }

    /// <summary>
    /// 列表修改消息参数结构
    /// </summary>
    public class CollectionChangedArgs : RoutedEventArgs
    {
        /// <summary>
        /// 列表变动的方式
        /// </summary>
        public System.Collections.Specialized.NotifyCollectionChangedAction action;

        /// <summary>
        /// 列表数据
        /// </summary>
        public object item { get; set; }

        /// <summary>
        /// CollectionChangedArgs
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="action"></param>
        /// <param name="item"></param>
        public CollectionChangedArgs(RoutedEvent routedEvent, System.Collections.Specialized.NotifyCollectionChangedAction action, object item)
        {
            this.RoutedEvent = routedEvent;
            this.action = action;
            this.item = item;
        }
    }

    /// <summary>
    /// 语言类型枚举
    /// </summary>
    public enum EnumLanguage
    {
        /// <summary>
        /// Chinese
        /// </summary>
        Chinese = 0,
        /// <summary>
        /// English
        /// </summary>
        English = 1,
    }


}
