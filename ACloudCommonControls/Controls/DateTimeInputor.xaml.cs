using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Data;
using System.Windows.Data;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// DateTimeInputor.xaml 的交互逻辑
    /// </summary>
    public partial class DateTimeInputor : UserControl
    {
        /// <summary>
        /// 设置的时间
        /// </summary>
        public DateTime? dateTime
        {
            get { return (DateTime?)GetValue(dateTimeProperty); }
            set { SetValue(dateTimeProperty, value);}
        }
        
        ///时间依赖属性
        // Using a DependencyProperty as the backing store for dateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty dateTimeProperty =
            DependencyProperty.Register("dateTime", typeof(DateTime?), typeof(DateTimeInputor), 
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDateTimePropertyChanged)));

        private class datetimeInfo:INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void DoPropertyChang(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            private DateTime? _day;
            /// <summary>
            /// 日期
            /// </summary>
            public DateTime? day
            {
                get { return _day; }
                set
                {
                    _day = value;
                    DoPropertyChang("day");
                    DoPropertyChang("notnull");
                }
            }

            private int _hour;
            /// <summary>
            /// 小时
            /// </summary>
            public int hour
            {
                get { return _hour; }
                set
                {
                    if (value < 0)
                        _hour = 0;
                    else if (value >= 24)
                        _hour = 23;
                    else
                        _hour = value;
                    DoPropertyChang("hour");
                }
            }

            private int _minute;
            /// <summary>
            /// 分钟
            /// </summary>
            public int minute
            {
                get { return _minute; }
                set
                {
                    if (value < 0)
                        _minute = 0;
                    else if (value >= 59)
                        _minute = 59;
                    else
                        _minute = value;
                    DoPropertyChang("minute");
                }
            }

            /// <summary>
            /// 日期是否为NULL
            /// </summary>
            public bool notnull { get { return day != null; } }

            public bool fromSetDatetime = false;

            /// <summary>
            /// 设置日期时间
            /// </summary>
            /// <param name="datetime"></param>
            public void SetDateTime(DateTime? datetime)
            {
                fromSetDatetime = true;
                day = datetime;
                if (day == null)
                {
                    hour = 0;
                    minute = 0;
                }
                else
                {
                    hour = ((DateTime)day).Hour;
                    minute = ((DateTime)day).Minute;
                }
                fromSetDatetime = false;
            }

            public DateTime? GetDateTime()
            {
                if (day == null)
                    return null;

                DateTime tempday = (DateTime)day;
                return new DateTime(tempday.Year, tempday.Month, tempday.Day, hour, minute, 0);
            }
        }

        private datetimeInfo curDateTime { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DateTimeInputor()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(DateTimeInputor_Loaded);
        }

        void DateTimeInputor_Loaded(object sender, RoutedEventArgs e)
        {
            datePicker.DisplayDateStart = new DateTime(2000, 1, 1);
            datePicker.DisplayDateEnd = new DateTime(2049, 12, 31);
            datePicker.DisplayDate = DateTime.Now;
            curDateTime = new datetimeInfo();
            curDateTime.PropertyChanged += new PropertyChangedEventHandler(curDateTime_PropertyChanged);
            rootGrid.DataContext = curDateTime;

            for (int i = 0; i < 24; i++)
                txtHour.Items.Add(i);
            for (int i = 0; i < 60; i++)
                txtMinute.Items.Add(i);

            //日期和时间都只能输入两位
            var textBox = txtMinute.Template.FindName("PART_EditableTextBox", txtHour) as TextBox;
            if (textBox != null)
            {
                Binding bind = new Binding("hour");
                bind.Mode = BindingMode.TwoWay;
                textBox.SetBinding(TextBox.TextProperty, bind);
                textBox.MaxLength = 2;
            }

            textBox = txtMinute.Template.FindName("PART_EditableTextBox", txtMinute) as TextBox;
            if (textBox != null)
            {
                Binding bind = new Binding("minute");
                bind.Mode = BindingMode.TwoWay;
                textBox.SetBinding(TextBox.TextProperty, bind);
                textBox.MaxLength = 2;
            }
        }

        private static void OnDateTimePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DateTimeInputor inputor = sender as DateTimeInputor;
            if (inputor == null)
                return;

            if (e.NewValue != e.OldValue && inputor.curDateTime != null)
            {
                inputor.curDateTime.SetDateTime(e.NewValue as DateTime?);
            }
        }

        void curDateTime_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(sender is datetimeInfo && curDateTime.fromSetDatetime == false)
                SetValue(dateTimeProperty, curDateTime.GetDateTime()); 
        }
        
        /// <summary>
        /// 强制刷新数据
        /// </summary>
        public void Refresh()
        {
            var textBox = txtMinute.Template.FindName("PART_EditableTextBox", txtHour) as TextBox;
            if (textBox != null)
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            textBox = txtMinute.Template.FindName("PART_EditableTextBox", txtMinute) as TextBox;
            if (textBox != null)
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
