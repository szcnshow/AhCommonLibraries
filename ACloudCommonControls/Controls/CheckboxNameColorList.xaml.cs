using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// 显示文件列表，包括 颜色  选择项  字符串， 用于趋势图中显示成分列表 ，也用于光谱图形中显示文件列表
    /// </summary>
    public partial class CheckboxNameColorList : UserControl
    {
        string[] strColors =
            {
                "Black", "Red", "Blue", "Gold", "Brown", "DarkGreen", "Navy", "LawnGreen", 
                "DimGray", "DarkRed", "OrangeRed", "Olive",  "Teal", "SlateGray", "Gray", "MidnightBlue", 
                "Orange", "YellowGreen", "SeaGreen", "Aqua", "LightBlue", "Violet", "DarkGray",
                "Pink", "Yellow", "Lime", "Turquoise", "SkyBlue", "Plum", "LightGray", "Indigo",
                "LightPink", "Tan", "Lavender"
            };
        /// <summary>
        /// Max selection items
        /// </summary>
        public int maxSelectedItem = 50;    //最多50个选择项

        /// <summary>
        /// 用于ListBox绑定
        /// </summary>
        public class StringAndColorList : INotifyPropertyChanged    //用于ListBox绑定
        {
            private bool _isChecked;
            /// <summary>
            /// Is checked
            /// </summary>
            public bool IsChecked                           //是否选择
            {
                get { return _isChecked; }
                set
                {
                    _isChecked = value;
                    this.OnPropertyChanged("IsChecked");
                }
            }
            private SolidColorBrush _color;     //颜色
            /// <summary>
            /// Item color
            /// </summary>
            public SolidColorBrush Color 
            {
                get { return _color; }
                set
                {
                    _color = value;
                    OnPropertyChanged("Color");
                }
            }

            private string _name;               //显示的文字
            /// <summary>
            /// Item display name
            /// </summary>
            public string Name {
                get { return _name; }
                set
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }

            private object _tag;                //绑定的内容
            /// <summary>
            /// Attached datta
            /// </summary>
            public object Tag {
                get { return _tag; }
                set
                {
                    _tag = value;
                    OnPropertyChanged("Tag");
                }
            }                 

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="text">Display text</param>
            /// <param name="showcolor">Color</param>
            /// <param name="data">attached data</param>
            public StringAndColorList(string text, SolidColorBrush showcolor, object data=null)
            {
                IsChecked = false;
                Color = showcolor;
                this.Tag = data;
                this.Name = text;

                if (this.Tag == null)
                    this.Tag = Name;
            }

            #region INotifyPropertyChanged Members

            void OnPropertyChanged(string prop)
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }

            /// <summary>
            /// Property changed
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }
        /// <summary>
        /// 要显示的文件列表
        /// </summary>
        public ObservableCollection<StringAndColorList> FileListData = new ObservableCollection<StringAndColorList>();   //要显示的文件列表
        /// <summary>
        /// 项目选中消息
        /// </summary>
        public static readonly RoutedEvent ItemCheckedEvent = EventManager.RegisterRoutedEvent("ItemChecked",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CheckboxNameColorList));
        /// <summary>
        /// 项目选中消息
        /// </summary>
        public event RoutedEventHandler ItemChecked
        {
            add { AddHandler(ItemCheckedEvent, value); }
            remove { RemoveHandler(ItemCheckedEvent, value); }
        }

        SolidColorBrush[] AllColorList;     //颜色列表

        //private List<StringAndColorList> SelectedItems = new List<StringAndColorList>();     //当前选择的内容
        /// <summary>
        /// 构造函数
        /// </summary>
        public CheckboxNameColorList()
        {
            InitializeComponent();

            //获取系统定义的颜色
            System.Reflection.PropertyInfo[] brushprops= typeof(Brushes).GetProperties();
            AllColorList = new SolidColorBrush[strColors.Length];

            for (int i = 0; i < strColors.Length;i++ )
                AllColorList[i] = (SolidColorBrush)typeof(Brushes).GetProperty(strColors[i]).GetValue(null, null);

            ListFile.ItemsSource = FileListData;
        }

        /// <summary>
        /// 直接增加一项，这里肯定是增加光谱文件名
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="isSelected"></param>
        /// <param name="data"></param>
        public void AddItem(string itemName, bool isSelected, object data=null)
        {
            //查找是否已经加载了
            if (data == null)
                data = itemName;
            if (FileListData.FirstOrDefault(subitem => subitem.Tag == data) != null)
                return;

            int index = FileListData.Count;
            StringAndColorList item = new StringAndColorList(itemName, AllColorList[index % AllColorList.Length], data);
            FileListData.Add(item);
            ListFile.SelectedIndex = ListFile.Items.Count - 1;
            ListFile.ScrollIntoView(item);
            ListFile.Items.Refresh();

            if(isSelected)
            {
                item.IsChecked = isSelected;
            }
        }

        /// <summary>
        /// 增加多项，这里肯定是增加光谱文件名
        /// </summary>
        /// <param name="itemNames"></param>
        /// <param name="isSelected"></param>
        /// <param name="datas"></param>
        /// <param name="clear"></param>
        public void AddItem(List<string>itemNames, bool isSelected, List<object>datas, bool clear)
        {
            if (itemNames == null)
            {
                RemoveAllItems();
                return;
            }

            if (datas == null)
            {
                datas = new List<object>();
                foreach (string str in itemNames)
                    datas.Add(str);
            }
            if (datas.Count != itemNames.Count)
                return;

            //需要删除没有在itemNames列表中的项
            if (clear)
            {
                List<object> tempData = new List<object>();
                foreach(StringAndColorList item in FileListData)
                {
                    if (!datas.Exists(subdata => item.Tag == subdata))
                        tempData.Add(item.Tag);
                }
                RemoveItem(tempData);
            }

            for(int i=0; i<itemNames.Count; i++)
            {
                //查找是否已经加载了
                if (FileListData.FirstOrDefault(olditem => olditem.Tag == datas[i]) != null)
                    continue;

                int index = FileListData.Count;

                StringAndColorList newdata = new StringAndColorList(itemNames[i], AllColorList[index % AllColorList.Length], datas[i]);

                FileListData.Add(newdata);
            }
            ListFile.SelectedIndex = ListFile.Items.Count - 1;
            ListFile.Items.Refresh();
            RaiseCheckedEvent();
        }

        /// <summary>
        /// 清除所有选择项
        /// </summary>
        public void RemoveSelectedItems()
        {
            List<StringAndColorList> tempList = new List<StringAndColorList>();
            foreach (StringAndColorList item in FileListData)
            {
                if (item.IsChecked)
                    tempList.Add(item);
            }

            foreach (StringAndColorList item in tempList)
            {
                FileListData.Remove(item);
            }
            ListFile.Items.Refresh();
            RaiseCheckedEvent();
        }

        /// <summary>
        /// 清除一项
        /// </summary>
        /// <param name="seldata"></param>
        public void RemoveItem(object seldata)
        {
            StringAndColorList item = FileListData.FirstOrDefault(subitem => subitem.Tag == seldata);
            if(item != null)
            {
                FileListData.Remove(item);
                ListFile.Items.Refresh();
                RaiseCheckedEvent();
            }
        }
        
        /// <summary>
        /// 一次清除多项
        /// </summary>
        /// <param name="selDatas"></param>
        public void RemoveItem(List<object> selDatas)
        {
            foreach (object data in selDatas)
            {
                StringAndColorList item = FileListData.FirstOrDefault(subitem => subitem.Tag == data);
                if(item != null)
                {
                    FileListData.Remove(item);
                }
            }
            ListFile.Items.Refresh();
            RaiseCheckedEvent();
        }

        /// <summary>
        /// 清除所有内容
        /// </summary>
        public void RemoveAllItems()
        {
            FileListData.Clear();
            ListFile.Items.Refresh();
            RaiseCheckedEvent();
        }

        /// <summary>
        /// 替换项目
        /// </summary>
        /// <param name="oldTag"></param>
        /// <param name="newName"></param>
        /// <param name="newTag"></param>
        public void ReplaceItem(string oldTag, string newName, string newTag)
        {
            StringAndColorList listitem = FileListData.First(item => (string)item.Tag == oldTag);
            if (listitem != null)
            {
                listitem.Name = newName;
                listitem.Tag = newTag;
            }
        }

        /// <summary>
        /// 选择指定项目
        /// </summary>
        /// <param name="seldata"></param>
        public void SelectOneItem(object seldata)
        {
            StringAndColorList selitem = FileListData.FirstOrDefault(item => seldata == item.Tag);
            if(selitem != null)
            {
                foreach (StringAndColorList item in FileListData)
                    item.IsChecked = false;

                selitem.IsChecked = true;
                int index = FileListData.IndexOf(selitem);
                if(index>=0)
                    ListFile.SelectedIndex = index;

                RaiseCheckedEvent();
            }
        }

        private void RaiseCheckedEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = ItemCheckedEvent;
            args.Source = this;
            RaiseEvent(args);       
        }

        //获取当前鼠标的点击的选择项
        private StringAndColorList GetItemOnClick(Grid parentGrid)
        {
            TextBlock txtfile = parentGrid.FindName("specText") as TextBlock;
            if (txtfile == null)
                return null;

            return FileListData.FirstOrDefault(item => txtfile.Tag == item.Tag);
        }

        //选择
        private void specCheck_Checked(object sender, RoutedEventArgs e)
        {
            RaiseCheckedEvent();
        }

        //不选
        private void specCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            RaiseCheckedEvent();
        }

        private void ListItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            StringAndColorList selitem = GetItemOnClick(sender as Grid);
            selitem.IsChecked = true;

            //RaiseCheckedEvent();

        }

        private void ListFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListFile.SelectedIndex < 0)
                return;

            StringAndColorList selitem = FileListData[ListFile.SelectedIndex];
            selitem.IsChecked = true;
            //RaiseCheckedEvent();
        }

        /// <summary>
        /// 获取所有选择的项目
        /// </summary>
        /// <returns></returns>
        public List<StringAndColorList> GetSelectedFiles()
        {
            List<StringAndColorList> selFiles = new List<StringAndColorList>();
            foreach (StringAndColorList item in FileListData)
            {
                if (item.IsChecked)
                    selFiles.Add(item);
            }

            return selFiles;
        }

        /// <summary>
        /// 返回当前List鼠标热点所选中的项目
        /// </summary>
        /// <returns></returns>
        public object GetCurrentHotFile()
        {
            if (ListFile.SelectedItems.Count < 0)
                return null;
            return ((StringAndColorList)ListFile.SelectedItems[ListFile.SelectedItems.Count - 1]).Tag;     //选择集合的最后一个
        }

        /// <summary>
        /// 查找一个显示项 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public StringAndColorList FindListItem(object tag)
        {
            if (FileListData == null)
                return null;

            return FileListData.FirstOrDefault(item => item.Tag == tag);
        }
    }
}
