using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
//using System.Xml.Serialization;
using Ai.Hong.Controls.Common;
using Ai.Hong.Charts;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// 附带颜色、选择的列表
    /// </summary>
    public partial class SelectColorList : UserControl
    {
        /// <summary>
        /// 列表选择消息
        /// </summary>
        public static readonly RoutedEvent ItemSelectedEvent = EventManager.RegisterRoutedEvent("ItemSelected",
                RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SelectColorList));
        /// <summary>
        /// 列表选择消息
        /// </summary>
        public event SelectionChangedEventHandler ItemSelected
        {
            add { AddHandler(ItemSelectedEvent, value); }
            remove { RemoveHandler(ItemSelectedEvent, value); }
        }

        /// <summary>
        /// 列表移除消息
        /// </summary>
        public static readonly RoutedEvent ItemRemovedEvent = EventManager.RegisterRoutedEvent("ItemRemoved",
                RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SelectColorList));
        /// <summary>
        /// 列表移除消息
        /// </summary>
        public event SelectionChangedEventHandler ItemRemoved
        {
            add { AddHandler(ItemRemovedEvent, value); }
            remove { RemoveHandler(ItemRemovedEvent, value); }
        }

        /// <summary>
        /// 列表移除消息
        /// </summary>
        public static readonly RoutedEvent AllRemovedEvent = EventManager.RegisterRoutedEvent("AllRemoved",
                RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SelectColorList));
        /// <summary>
        /// 列表移除消息
        /// </summary>
        public event SelectionChangedEventHandler AllRemoved
        {
            add { AddHandler(AllRemovedEvent, value); }
            remove { RemoveHandler(AllRemovedEvent, value); }
        }

        /// <summary>
        /// 显示内容列表
        /// </summary>
        public ObservableCollection<ColorChartDisplayInfo> spectrumFileList = null;

        SpectrumChart newGraphicChart = null;

        /// <summary>
        /// 从列表头来的颜色变化消息
        /// </summary>
        private bool colorChangeFromHeader = false;

        /// <summary>
        /// 从图形显示控件来的选中消息
        /// </summary>
        private bool selectChangeFromChart = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectColorList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 文件列表变化消息
        /// </summary>
        void spectrumFileList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<ColorChartDisplayInfo> filelist = sender as ObservableCollection<ColorChartDisplayInfo>;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            ColorChartDisplayInfo info = item as ColorChartDisplayInfo;
                            if (info.color == null)
                                info.color = ColorChartDisplayInfo.GetDisplayColor(filelist.IndexOf(info));
                            info.PropertyChanged += spectrumData_PropertyChanged;

                            if (info.isChecked && info.xDatas != null && info.yDatas != null)
                            {
                                if (newGraphicChart != null)
                                    newGraphicChart.AddChart(info.xDatas, info.yDatas, info.color, info.key, info.name, info.dataDisplayFormat);
                            }
                        }
                        if (newGraphicChart != null && e.NewItems.Count > 0)    //需要刷新一下
                            newGraphicChart.Refresh();
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            ColorChartDisplayInfo info = item as ColorChartDisplayInfo;
                            if (newGraphicChart != null)
                                newGraphicChart.RemoveChart(info.key);
                        }
                        if (newGraphicChart != null && e.OldItems.Count > 0)    //需要刷新一下
                            newGraphicChart.Refresh();
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            ColorChartDisplayInfo info = item as ColorChartDisplayInfo;
                            if (newGraphicChart != null)
                                newGraphicChart.RemoveChart(info.key);
                        }
                    }
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            ColorChartDisplayInfo info = item as ColorChartDisplayInfo;
                            if (info.color == null)
                                info.color = ColorChartDisplayInfo.GetDisplayColor(filelist.IndexOf(info));
                            info.PropertyChanged += spectrumData_PropertyChanged;

                            if (info.isChecked && info.xDatas != null && info.yDatas != null)
                            {
                                if (newGraphicChart != null)
                                    newGraphicChart.AddChart(info.xDatas, info.yDatas, info.color, info.key, info.name, info.dataDisplayFormat);
                            }
                        }
                    }
                    if (newGraphicChart != null && (e.NewItems.Count > 0 || e.OldItems.Count>0))    //需要刷新一下
                        newGraphicChart.Refresh();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    if (newGraphicChart != null)
                    {
                        newGraphicChart.RemoveAllChart();
                        newGraphicChart.Refresh();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 设置列表显示内容
        /// </summary>
        public void SetGridData(ObservableCollection<ColorChartDisplayInfo> dataList)
        {
            spectrumFileList = dataList;
            spectrumFileList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(spectrumFileList_CollectionChanged);
            if (newGraphicChart != null)
            {
                newGraphicChart.RemoveAllChart();
                newGraphicChart.Refresh();
            }

            if(dataList != null)
            {
                foreach (ColorChartDisplayInfo item in spectrumFileList)
                {
                    item.PropertyChanged += spectrumData_PropertyChanged;
                    if (item.isChecked && item.xDatas != null && item.yDatas != null)
                    {
                        if (newGraphicChart != null)
                            newGraphicChart.AddChart(item.xDatas, item.yDatas, item.color, item.key, item.dataDisplayFormat);
                    }
                }
                if (newGraphicChart != null && (from p in spectrumFileList where p.isChecked select p).Count() > 0)     //需要刷新一下
                    newGraphicChart.Refresh();
            }
            gridSpectrum.ItemsSource = spectrumFileList;
        }

        /// <summary>
        /// 设置列表的光谱图形显示控件
        /// </summary>
        /// <param name="chart"></param>
        public void SetGraphicChart(SpectrumChart chart)
        {
            newGraphicChart = chart;

            newGraphicChart.ColorChanged += new RoutedPropertyChangedEventHandler<SolidColorBrush>(newGraphicChart_ColorChanged);
            newGraphicChart.ItemSelected += new SelectionChangedEventHandler(newGraphicChart_ItemSelected);
            newGraphicChart.ItemHidden += new SelectionChangedEventHandler(newGraphicChart_ItemHidden);
        }

        /// <summary>
        /// 光谱图形隐藏消息
        /// </summary>
        void newGraphicChart_ItemHidden(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                Guid key = (Guid)item;
                var info = spectrumFileList.FirstOrDefault(p => p.key == key);
                if (info != null)
                    info.isChecked = false;
            }
        }

        /// <summary>
        /// 光谱图形选中消息
        /// </summary>
        void newGraphicChart_ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            selectChangeFromChart = true;

            foreach (var item in spectrumFileList)
            {
                item.isSelected = newGraphicChart.SelectedItems.FirstOrDefault(p => p == item.key) != Guid.Empty;
            }

            selectChangeFromChart = false;
        }

        void newGraphicChart_ColorChanged(object sender, RoutedPropertyChangedEventArgs<SolidColorBrush> e)
        {
            foreach (var item in newGraphicChart.SelectedItems)
            {
                Guid key = (Guid)item;
                var info = spectrumFileList.FirstOrDefault(p => p.key == key);
                if (info != null)
                    info.color = e.NewValue;
            }
        }

        /// <summary>
        /// 设置一个新增项的属性变更通知消息
        /// </summary>
        /// <param name="item"></param>
        public void SetPropertyChangeEventtemp(ColorChartDisplayInfo item)
        {
            item.PropertyChanged += spectrumData_PropertyChanged;
        }

        /// <summary>
        /// 显示或者隐藏光谱图形
        /// </summary>
        void spectrumData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ColorChartDisplayInfo info = sender as ColorChartDisplayInfo;
            if (e.PropertyName == "isChecked")
            {
                if (newGraphicChart != null)
                {
                    //要有读入的光谱数据才能添加
                    if (info.isChecked && info.xDatas != null && info.yDatas != null)
                        newGraphicChart.AddChart(info.xDatas, info.yDatas, info.color, info.key, info.name, info.dataDisplayFormat);
                    else
                        newGraphicChart.RemoveChart(info.key);
                    newGraphicChart.Refresh();  //需要刷新一下
                }

                //headerVisibleChecker.Checked -= visibleChecker_Checked;
                //headerVisibleChecker.Unchecked -= visibleChecker_Checked;   // visibleChecker_Unchecked;
                //headerVisibleChecker.IsChecked = (from item in spectrumFileList where item.isChecked select item).Count() > 0;
                //headerVisibleChecker.Checked += visibleChecker_Checked;
                //headerVisibleChecker.Unchecked += visibleChecker_Checked;   //visibleChecker_Unchecked;
            }
        }

        /// <summary>
        /// 移除DataGrid中的选定项
        /// </summary>
        public static void RemoveDataGridItems(System.Collections.IList allItems, System.Collections.IList removeItems)
        {
            List<object> tempList = new List<object>();
            foreach (object item in removeItems)
                tempList.Add(item);

            foreach (object item in tempList)
            {
                allItems.Remove(item);
            }
        }

        /// <summary>
        /// 移除列表中选择的光谱文件
        /// </summary>
        public void RemoveSelected()
        {
            if (newGraphicChart != null)
            {
                foreach (ColorChartDisplayInfo info in gridSpectrum.SelectedItems)
                    newGraphicChart.RemoveChart(info.key);

                if (gridSpectrum.SelectedItems.Count > 0)    //需要刷新一下
                    newGraphicChart.Refresh();
            }

            RemoveDataGridItems(spectrumFileList, gridSpectrum.SelectedItems);
        }

        /// <summary>
        /// 增加列表中的列，并且所有的列都是只读的
        /// </summary>
        /// <param name="titles">列头的名称</param>
        /// <param name="values">列的绑定变量名称</param>
        /// <param name="isReadOnly">是否可以编辑</param>
        public void AddNewDataBinding(string[] titles, string[] values, bool isReadOnly=true)
        {
            if(titles.Length != values.Length)
                return;

            //当前显示的值
            List<string> curtitles = (from p in gridSpectrum.Columns
                                      where p.GetType() == typeof(DataGridTextColumn)
                                      select p.Header.ToString()).ToList();
            for (int i = 0; i < titles.Length; i++)
            {
                //Datagrid中已经有了，就不加入
                if (curtitles.FirstOrDefault(p => p == titles[i]) != null)
                    continue;

                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = titles[i];
                Binding bind = new Binding(values[i]);
                bind.Mode = isReadOnly ? BindingMode.OneWay :BindingMode.TwoWay;
                //if (titles[i].IndexOf("时间") >= 0 || titles[i].IndexOf("time", StringComparison.OrdinalIgnoreCase) >=0)
                //    bind.StringFormat = "yyyy-MM-dd HH:mm:ss";
                //else if (titles[i].IndexOf("日期") >= 0 || titles[i].IndexOf("date", StringComparison.OrdinalIgnoreCase) >=0)
                //    bind.StringFormat = "yyyy-MM-dd";
                col.Binding = bind;
                col.IsReadOnly = isReadOnly;
                gridSpectrum.Columns.Add(col);
            }
        }

        /// <summary>
        /// 移除绑定数据列
        /// </summary>
        /// <param name="titles">数据列名称</param>
        public void RemoveDataBinding(string[] titles)
        {
            if (titles == null || titles.Length == 0)
                return;

            List<DataGridColumn> needRemove = new List<DataGridColumn>();
            foreach (var col in gridSpectrum.Columns)
            {
                if ((col.Header is string) &&  titles.FirstOrDefault(p => (col.Header as string) == p) != null)
                {
                    needRemove.Add(col);
                }
            }

            foreach (var item in needRemove)
                gridSpectrum.Columns.Remove(item);

        }

        /// <summary>
        /// 插入一列数据
        /// </summary>
        /// <param name="column">列数据</param>
        /// <param name="index">插入位置,位置错误，将在最后添加</param>
        public void InsertDataColumn(DataGridColumn column, int index)
        {
            if (index < 0 || index >= gridSpectrum.Columns.Count)
                gridSpectrum.Columns.Add(column);
            else
                gridSpectrum.Columns.Insert(index, column);
        }

        /// <summary>
        /// 显示隐藏一列数据
        /// </summary>
        /// <param name="index">列号</param>
        /// <param name="visible">True=显示，False=隐藏</param>
        public void VisibleDataColumn(int index, bool visible)
        {
            if (index >= 0 && index < gridSpectrum.Columns.Count)
                gridSpectrum.Columns[index].Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 显示隐藏一列数据
        /// </summary>
        /// <param name="headerTitle">标题</param>
        /// <param name="visible">True=显示，False=隐藏</param>
        public void VisibleDataColumn(string headerTitle, bool visible)
        {
            var col = gridSpectrum.Columns.FirstOrDefault(p => (p.Header as string) == headerTitle);
            if (col == null)
                return;

            VisibleDataColumn(gridSpectrum.Columns.IndexOf(col), visible);
        }
        
        /// <summary>
        /// 设置绑定字段的显示格式
        /// </summary>
        /// <param name="titles">字段名称</param>
        /// <param name="stringFormats">字段格式</param>
        public void SetDataBindingStringFormat(string[] titles, string[] stringFormats)
        {
            if (titles.Length != stringFormats.Length)
                return;

            for (int i = 0; i < titles.Length; i++)
            {
                //Datagrid中已经有的才能设置
                DataGridColumn col = gridSpectrum.Columns.FirstOrDefault(p=>p.Header!=null && p.Header.ToString() == titles[i]);
                if (col != null && col is DataGridTextColumn)   //只设置文字格式的列
                {
                    var bind = (col as DataGridTextColumn).Binding;
                    if (bind != null)
                    {
                        if(stringFormats[i] != null)
                            bind.StringFormat = stringFormats[i];
                    }
                }
            }

        }

        /// <summary>
        /// 列表选择变更消息
        /// </summary>
        private void gridSpectrum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectChangeFromChart == false && newGraphicChart != null)
            {
                List<Guid> sels = new List<Guid>();
                foreach(var item in gridSpectrum.SelectedItems)
                {
                    ColorChartDisplayInfo info = item as ColorChartDisplayInfo;
                    if (info.isChecked)
                    {
                        sels.Add(info.key);
                    }
                }
                newGraphicChart.SelectedItems = sels;
            }

            SelectionChangedEventArgs arg = null;
            if(gridSpectrum.SelectedItem == null)
                arg = new SelectionChangedEventArgs(ItemSelectedEvent, new List<ColorChartDisplayInfo>(), new List<ColorChartDisplayInfo>());
            else
                arg = new SelectionChangedEventArgs(ItemSelectedEvent, new List<ColorChartDisplayInfo>(), new List<ColorChartDisplayInfo>() { (ColorChartDisplayInfo)gridSpectrum.SelectedItem });

            RaiseEvent(arg);
        }

        /// <summary>
        /// 用于设置特殊的列
        /// </summary>
        /// <returns></returns>
        public DataGrid GetDataGrid()
        {
            return gridSpectrum;
        }

        /// <summary>
        /// 选中一个Item,并自动将其滚动到可见区域
        /// </summary>
        /// <param name="item"></param>
        public void SelectItem(ColorChartDisplayInfo item)
        {
            gridSpectrum.SelectedItem = item;
            if (gridSpectrum.SelectedItem != null)
            {
                gridSpectrum.UpdateLayout();
                gridSpectrum.ScrollIntoView(gridSpectrum.SelectedItem);
            }
        }

        /// <summary>
        /// 获取列表中的选择项
        /// </summary>
        public List<ColorChartDisplayInfo> GetSelectedItems()
        {
            List<ColorChartDisplayInfo> retList = new List<ColorChartDisplayInfo>();
            foreach (ColorChartDisplayInfo item in gridSpectrum.SelectedItems)
                retList.Add(item);

            return retList;
        }

        /// <summary>
        /// 标题上的选择框消息
        /// </summary>
        private void visibleChecker_Checked(object sender, RoutedEventArgs e)
        {
            //如果选中的文件有一个的IsCheck=True，则隐藏所有选中文件，否则显示（考虑到刷新效率）
            bool show = true;
            foreach (ColorChartDisplayInfo item in gridSpectrum.SelectedItems)
            {
                if (item.isChecked == true)
                    show = false;
            }

            foreach (ColorChartDisplayInfo item in gridSpectrum.SelectedItems)
                item.isChecked = show;
        }

        ///// <summary>
        ///// 标题上的选择框消息
        ///// </summary>
        //private void visibleChecker_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    //如果选中的文件有一个的IsCheck=FALSE，则显示所有选中文件，否则隐藏
        //    bool show = false;
        //    foreach (ColorChartDisplayInfo item in gridSpectrum.SelectedItems)
        //    {
        //        if (item.isChecked == false)
        //            show = true;
        //    }

        //    foreach (ColorChartDisplayInfo item in gridSpectrum.SelectedItems)
        //        item.isChecked = show;
        //}

        /// <summary>
        /// 获取当前焦点项目
        /// </summary>
        /// <returns></returns>
        public ColorChartDisplayInfo GetFocusItem()
        {
            return gridSpectrum.SelectedItem as ColorChartDisplayInfo;
        }

        /// <summary>
        /// 选择新的颜色来设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorBorder_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CheckBox ctrl = sender as CheckBox;

            colorChangeFromHeader = false;
            ColorPicker colorPop = new ColorPicker();

            colorPop.Tag = ctrl.Tag;
            colorPop.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            colorPop.Closed += new EventHandler(colorPop_Closed);
            colorPop.IsOpen = true;
        }

        /// <summary>
        /// 修改显示颜色窗口关闭
        /// </summary>
        void colorPop_Closed(object sender, EventArgs e)
        {
            ColorPicker colorPop = sender as ColorPicker;
            if (colorPop == null || colorPop.selectedBursh == null)
                return;
             
            if (!colorChangeFromHeader)     //只改变一个文件,文件在 colorPop.Tag 中
            {
                ColorChartDisplayInfo item = colorPop.Tag as ColorChartDisplayInfo;
                if (item == null || spectrumFileList.IndexOf(item) < 0)
                    return;

                item.color = (SolidColorBrush)colorPop.selectedBursh;
                if (newGraphicChart != null)
                {
                    newGraphicChart.ChangeColor(item.key, item.color);
                    newGraphicChart.Refresh();  //需要刷新一下
                }
            }
            else    //改变选中文件
            {
                foreach (var item in gridSpectrum.SelectedItems)
                {
                    ColorChartDisplayInfo info = item as ColorChartDisplayInfo;
                    info.color = (SolidColorBrush)colorPop.selectedBursh;
                    if (newGraphicChart != null)
                        newGraphicChart.ChangeColor(info.key, info.color);
                }
                if (newGraphicChart != null && gridSpectrum.SelectedItems.Count > 0)
                    newGraphicChart.Refresh();
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            gridSpectrum.Items.Refresh();
        }

        /// <summary>
        /// 刷新一个项
        /// </summary>
        /// <param name="item"></param>
        /// <param name="refreshRightNow">是否立即刷新图形</param>
        public void RefreshItem(ColorChartDisplayInfo item, bool refreshRightNow = true)
        {
            if (spectrumFileList.IndexOf(item) == -1)
                return;

            newGraphicChart.RemoveChart(item.key);
            if (item.isChecked == false)
                return;

            newGraphicChart.AddChart(item.xDatas, item.yDatas, item.color, item.key, item.name, item.dataDisplayFormat);
            if (refreshRightNow)
                newGraphicChart.Refresh();
        }

        /// <summary>
        /// 刷新图形
        /// </summary>
        public void RefreshGraphic()
        {
            newGraphicChart.Refresh();
        }

        /// <summary>
        /// 移除列表中的选中项目
        /// </summary>
        private void headerRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            //发送项目移除消息
            List<ColorChartDisplayInfo> selectedItems = new List<ColorChartDisplayInfo>();
            foreach (ColorChartDisplayInfo item in gridSpectrum.SelectedItems)
                selectedItems.Add(item);

            SelectionChangedEventArgs arg = new SelectionChangedEventArgs(AllRemovedEvent, selectedItems, null);
            RaiseEvent(arg);

            CommonMethod.RemoveDataGridItems(spectrumFileList, gridSpectrum.SelectedItems);
        }

        /// <summary>
        /// 移除列表中的当前项目
        /// </summary>
        private void CellRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Tag != null && btn.Tag is ColorChartDisplayInfo)
            {
                ColorChartDisplayInfo item = btn.Tag as ColorChartDisplayInfo;

                //发送项目移除消息
                SelectionChangedEventArgs arg = new SelectionChangedEventArgs(ItemRemovedEvent, new List<ColorChartDisplayInfo>() { item }, null);
                RaiseEvent(arg);

                spectrumFileList.Remove(item);
            }
        }

        /// <summary>
        /// 获取底层的Datagrid
        /// </summary>
        /// <returns></returns>
        public DataGrid GetUnderDataGrid()
        {
            return gridSpectrum;
        }

        private void headerChangeColor_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            colorChangeFromHeader = true;
            ColorPicker colorPop = new ColorPicker();
            colorPop.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            colorPop.Closed += new EventHandler(colorPop_Closed);
            colorPop.IsOpen = true;
        }

        private void checkImage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //如果选中的文件有一个的IsCheck=True，则隐藏所有选中文件，否则显示（考虑到刷新效率）
            bool show = true;
            foreach (ColorChartDisplayInfo item in gridSpectrum.SelectedItems)
            {
                if (item.isChecked == true)
                    show = false;
            }

            int index = 0;
            foreach (ColorChartDisplayInfo item in gridSpectrum.SelectedItems)
            {
                //为了效率，只显示选中的前100个光谱
                index++;
                if (show == true && index > 100)
                    break;

                item.isChecked = show;
            }
        }

        /// <summary>
        /// 修改谱图的显示颜色
        /// </summary>
        /// <param name="color">目标颜色</param>
        /// <param name="items">需要修改的谱图列表，如果为NULL，表示修改当前选中谱图</param>
        public void SetItemColor(SolidColorBrush color, IList<ColorChartDisplayInfo> items = null)
        {
            if (items == null)
            {
                items = GetSelectedItems();
            }

            foreach (var item in items)
            {
                item.color = color;
                if (newGraphicChart != null)
                    newGraphicChart.ChangeColor(item.key, item.color);
            }

            //刷新图像
            if (newGraphicChart != null)
                newGraphicChart.Refresh();

            //刷新列表
            //Refresh();
        }
    }
}
