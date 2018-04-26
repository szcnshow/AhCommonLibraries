using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Series;
using System.Windows.Media;
using System.Windows.Data;

namespace Ai.Hong.Charts
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class SpectrumChart : UserControl
    {
        /// <summary>
        /// 图形属性
        /// </summary>
        private class GraphicInfo
        {
            /// <summary>
            /// 光谱图形
            /// </summary>
            public OxyPlot.Series.LineSeries chart;

            /// <summary>
            /// 光谱ID
            /// </summary>
            public Guid key;

            /// <summary>
            /// 图形颜色
            /// </summary>
            public SolidColorBrush chartColor { get; set; }

            /// <summary>
            /// 文件名
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// 图形显示位移（Y轴)
            /// </summary>
            public double offset { get; set; }

            public GraphicInfo()
            {
            }

            /// <summary>
            /// 创建Chart图像
            /// </summary>
            /// <param name="xDatas">x轴数据</param>
            /// <param name="yDatas">y轴数据</param>
            /// <param name="color">颜色</param>
            /// <param name="lineWidth">线宽</param>
            /// <param name="labelFormat">数据显示格式</param>
            /// <returns></returns>
            private OxyPlot.Series.LineSeries CreateChart(double[]xDatas, double[] yDatas, SolidColorBrush color, double lineWidth, string labelFormat)
            {
                if (xDatas == null || yDatas == null || xDatas.Length == 0 || xDatas.Length != yDatas.Length)
                    return null;

                OxyPlot.Series.LineSeries newchart = new OxyPlot.Series.LineSeries();
                for (int i = 0; i < xDatas.Length; i++)
                    newchart.Points.Add(new DataPoint(xDatas[i], yDatas[i]));

                newchart.LineStyle = LineStyle.Solid;
                newchart.SelectionMode = OxyPlot.SelectionMode.Multiple;
                newchart.Selectable = true;
                newchart.StrokeThickness = lineWidth;
                newchart.Color = OxyColor.FromArgb(color.Color.A, color.Color.R, color.Color.G, color.Color.B);
                if (!string.IsNullOrWhiteSpace(name))
                    newchart.TrackerFormatString = System.IO.Path.GetFileNameWithoutExtension(name) + Environment.NewLine + "{1}: {2:" + labelFormat + "} , {3}: {4:" + labelFormat + "}";

                return newchart;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="xDatas">X轴数据</param>
            /// <param name="yDatas">Y轴数据</param>
            /// <param name="color">显示颜色</param>
            /// <param name="lineWidth">图形线宽</param>
            /// <param name="key">Key</param>
            /// <param name="labelName">图形名称</param>
            /// <param name="labelFormat">数据显示格式</param>
            public GraphicInfo(double[] xDatas, double[] yDatas, SolidColorBrush color, double lineWidth, Guid key, string labelName = null, string labelFormat="F2")
            {
                this.name = labelName;
                this.key = key;
                chart = CreateChart(xDatas, yDatas, color, lineWidth,labelFormat);
            }
        }

        /// <summary>
        /// 颜色改变消息参数
        /// </summary>
        public class ColorChangedArg:RoutedEventArgs
        {
            /// <summary>
            /// 当前显示的颜色
            /// </summary>
            public SolidColorBrush color { get; set; }

            /// <summary>
            /// 选中的图形
            /// </summary>
            public List<Guid> items { get; set; }
        }

        /// <summary>
        /// 弹出菜单项
        /// </summary>
        public enum chartMenuItems
        {
            /// <summary>
            /// 选择
            /// </summary>
            Select,     //选择
            /// <summary>
            /// 缩放的主菜单
            /// </summary>
            Zoom,       //缩放的主菜单
            /// <summary>
            /// 放大
            /// </summary>
            zoomIn,     //放大
            /// <summary>
            /// 缩小
            /// </summary>
            zoomOut,    //缩小
            /// <summary>
            /// 重置XY
            /// </summary>
            resetXY,    //重置XY
            /// <summary>
            /// 重置Y
            /// </summary>
            resetY,     //重置Y
            /// <summary>
            /// 移动
            /// </summary>
            Pan,        //移动
            /// <summary>
            /// 显示的主菜单
            /// </summary>
            Display,        //显示的主菜单
            /// <summary>
            /// 修改颜色
            /// </summary>
            Colors,         //修改颜色
            /// <summary>
            /// 隐藏图形
            /// </summary>
            Hide,           //隐藏图形
            /// <summary>
            /// 显示网格
            /// </summary>
            showGridLine,   //显示网格
            /// <summary>
            /// 显示信息
            /// </summary>
            showInformation,    //显示信息
            /// <summary>
            /// 标志峰位的主菜单
            /// </summary>
            peakPick,       //标志峰位的主菜单
            /// <summary>
            /// 向上的峰位
            /// </summary>
            upPeakPick,     //向上的峰位
            /// <summary>
            /// 向下的峰位
            /// </summary>
            downPeakPick    //向下的峰位 
        }

        /// <summary>
        /// 绘图区域的颜色
        /// </summary>
        public SolidColorBrush PlotAreaBackground
        {
            get { return ConvertToSolidBrush(spectrumChart.Model.PlotAreaBackground); }
            set { spectrumChart.Model.PlotAreaBackground = ConvertOxyColor(value); }
        }

        /// <summary>
        /// 底部区域的颜色
        /// </summary>
        public SolidColorBrush ModelBackground
        {
            get { return ConvertToSolidBrush(spectrumChart.Model.Background); }
            set { spectrumChart.Model.Background = ConvertOxyColor(value); }
        }

        #region Propertyies

        /// <summary>
        /// 所有图形
        /// </summary>
        List<GraphicInfo> graphicFiles = new List<GraphicInfo>();

        /// <summary>
        /// 选中的图形
        /// </summary>
        List<GraphicInfo> selectedFiles = new List<GraphicInfo>();

        /// <summary>
        /// 图形控制器
        /// </summary>
        PlotController myController = new PlotController();

        /// <summary>
        /// 自定义的X轴
        /// </summary>
        OxyPlot.Axes.LinearAxis xAxs;

        /// <summary>
        /// 操作按钮面板
        /// </summary>
        GraphicOperatePanel operatePanel = null;

        //OxyPlot.Wpf.TrackerControl
        /// <summary>
        /// 所有操作按钮
        /// </summary>
        Dictionary<chartMenuItems, System.Windows.Controls.Primitives.ButtonBase> operateButtons = new Dictionary<chartMenuItems, System.Windows.Controls.Primitives.ButtonBase>();

        /// <summary>
        /// 列表选择消息
        /// </summary>
        public static readonly RoutedEvent ItemSelectedEvent = EventManager.RegisterRoutedEvent("ItemSelected",
                RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SpectrumChart));
        /// <summary>
        /// 列表选择消息
        /// </summary>
        public event SelectionChangedEventHandler ItemSelected
        {
            add { AddHandler(ItemSelectedEvent, value); }
            remove { RemoveHandler(ItemSelectedEvent, value); }
        }

        /// <summary>
        /// 颜色改变消息
        /// </summary>
        public static readonly RoutedEvent ColorChangedEvent = EventManager.RegisterRoutedEvent("ColorChanged",
                RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<SolidColorBrush>), typeof(SpectrumChart));
        /// <summary>
        /// 颜色改变消息
        /// </summary>
        public event RoutedPropertyChangedEventHandler<SolidColorBrush> ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }

        /// <summary>
        /// 图形隐藏消息
        /// </summary>
        public static readonly RoutedEvent ItemHiddenEvent = EventManager.RegisterRoutedEvent("ItemHidden",
                RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SpectrumChart));
        /// <summary>
        /// 图形隐藏消息
        /// </summary>
        public event SelectionChangedEventHandler ItemHidden
        {
            add { AddHandler(ItemHiddenEvent, value); }
            remove { RemoveHandler(ItemHiddenEvent, value); }
        }

        /// <summary>
        /// 当前选择项
        /// </summary>
        public Guid SelectedItem
        {
            get { return (Guid)GetValue(SelectedItemProperty); }
            set
            {
                //检查Value是否在图形列表中
                Guid newvalue = graphicFiles.FirstOrDefault(p => p.key == value) == null ? Guid.Empty : value;

                //如果新选择项没有在列表中，则需要清除原有选择项
                bool clear = selectedFiles.FirstOrDefault(p => p.key == newvalue) == null;
                bool needrefresh = AddNewSelectedItems(new List<Guid>() { newvalue }, clear);
                SetValue(SelectedItemProperty, newvalue);

                if (needrefresh)
                    Refresh();
            }
        }
        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 当前选择项
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(Guid), typeof(SpectrumChart), new PropertyMetadata(Guid.Empty));

        /// <summary>
        /// 选中项列表
        /// </summary>
        public List<Guid> SelectedItems
        {
            get
            {
                return (from p in selectedFiles select p.key).ToList();
            }
            set
            {
                bool needrefresh = AddNewSelectedItems(value, true);
                //SetValue(SelectedItemsProperty, value);

                //如果当前选择项没有在选择列表中
                if(selectedFiles.Count == 0 || selectedFiles.FirstOrDefault(p=>p.key == SelectedItem) == null)
                {
                    SetValue(SelectedItemProperty, Guid.Empty);
                }

                if (needrefresh)
                    Refresh();

            }
        }
        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 选中项列表
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(List<Guid>), typeof(SpectrumChart), new PropertyMetadata(null));

        /// <summary>
        /// 正常图形的线宽
        /// </summary>
        public double LineWidth
        {
            get { return (double)GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }
        // Using a DependencyProperty as the backing store for LineWidth.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 正常图形的线宽
        /// </summary>
        public static readonly DependencyProperty LineWidthProperty =
            DependencyProperty.Register("LineWidth", typeof(double), typeof(SpectrumChart), new PropertyMetadata(1.0));

        /// <summary>
        /// 选中的图形线宽
        /// </summary>
        public double SelectedLineWidth
        {
            get { return (double)GetValue(SelectedLineWidthProperty); }
            set { SetValue(SelectedLineWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedLineWidth.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 选中的图形线宽
        /// </summary>
        public static readonly DependencyProperty SelectedLineWidthProperty =
            DependencyProperty.Register("SelectedLineWidth", typeof(double), typeof(SpectrumChart), new PropertyMetadata(2.0));

        /// <summary>
        /// 图像选择的敏感度
        /// </summary>
        public double selectSensitiy
        {
            get { return (double)GetValue(selectSensitiyProperty); }
            set { if(value>=1.0) SetValue(selectSensitiyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for selectSensitiy.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 图像选择的敏感度
        /// </summary>
        public static readonly DependencyProperty selectSensitiyProperty =
            DependencyProperty.Register("selectSensitiy", typeof(double), typeof(SpectrumChart), new PropertyMetadata(2.0));

        
        /// <summary>
        /// 预定义的颜色（More为自定义)
        /// </summary>
        private static string[] strColors =
            {
                "Black", "Red", "Blue", "Gold", "Brown", "DarkGreen", "Navy", "LawnGreen", 
                "DimGray", "DeepPink", "OrangeRed", "Olive",  "Teal", "SlateGray", "Gray", "MidnightBlue", 
                "Orange", "YellowGreen", "SeaGreen", "Aqua", "LightBlue", "Violet", "DarkGray",
                "Pink", "Yellow", "Lime", "Turquoise", "SkyBlue", "Plum", "LightGray", "Indigo",
                "LightPink", "Tan", "LightCoral", "More"
            };

        System.Windows.Input.Cursor currentCursor = System.Windows.Input.Cursors.Arrow;

        /// <summary>
        /// 背景颜色
        /// </summary>
        public SolidColorBrush ChartBackground
        {
            get { return (SolidColorBrush)GetValue(ChartBackgroundProperty); }
            set { SetValue(ChartBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChartBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartBackgroundProperty =
            DependencyProperty.Register("ChartBackground", typeof(SolidColorBrush), typeof(SpectrumChart), new PropertyMetadata(Brushes.White));

        
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public SpectrumChart()
        {
            InitializeComponent();
            InitChart();
            InitContextMenu();

            //默认峰位标注按钮隐藏
            SetMenuItemVisible(chartMenuItems.upPeakPick, false);
            SetMenuItemVisible(chartMenuItems.downPeakPick, false);
            SetMenuItemVisible(chartMenuItems.peakPick, false);

        }

        /// <summary>
        /// 初始化菜单
        /// </summary>
        private void InitContextMenu()
        {
            spectrumChart.ContextMenu = new ContextMenu();
            MenuItem item = new MenuItem();
            item.Name = chartMenuItems.Select.ToString();
            item.Header = "选择";
            item.Click += ContextMenu_Click;
            spectrumChart.ContextMenu.Items.Add(item);

            item = new MenuItem();
            item.Name = chartMenuItems.Pan.ToString();
            item.Header = "移动";
            item.Click += ContextMenu_Click;
            spectrumChart.ContextMenu.Items.Add(item);

            item = new MenuItem();
            item.Name = chartMenuItems.Zoom.ToString();
            item.Header = "缩放";
            spectrumChart.ContextMenu.Items.Add(item);

            MenuItem subitem = new MenuItem();
            subitem.Name = chartMenuItems.zoomIn.ToString();
            subitem.Header = "放大";
            subitem.Click += ContextMenu_Click;
            item.Items.Add(subitem);

            subitem = new MenuItem();
            subitem.Name = chartMenuItems.zoomOut.ToString();
            subitem.Header = "缩小";
            subitem.Click += ContextMenu_Click;
            item.Items.Add(subitem);

            subitem = new MenuItem();
            subitem.Name = chartMenuItems.resetXY.ToString();
            subitem.Header = "重置XY轴";
            subitem.Click += ContextMenu_Click;
            item.Items.Add(subitem);

            subitem = new MenuItem();
            subitem.Name = chartMenuItems.resetY.ToString();
            subitem.Header = "重置Y轴";
            subitem.Click += ContextMenu_Click;
            item.Items.Add(subitem);

            //显示主菜单
            item = new MenuItem();
            item.Name = chartMenuItems.Display.ToString();
            item.Header = "显示";
            item.Click += ContextMenu_Click;
            spectrumChart.ContextMenu.Items.Add(item);

            //颜色菜单
            subitem = new MenuItem();
            subitem.Name = chartMenuItems.Colors.ToString();
            subitem.Header = "更换颜色";
            subitem.Click += ContextMenu_Click;
            subitem.ItemsPanel = this.Resources["ItemsPanelTemplate1"] as ItemsPanelTemplate;
            foreach(var color in strColors)
            {
                AddSubColorMenu(subitem, color);
            }
            item.Items.Add(subitem);

            //显示信息菜单
            subitem = new MenuItem();
            subitem.Name = chartMenuItems.showInformation.ToString();
            subitem.Header = "光谱信息";
            subitem.Click += ContextMenu_Click;
            item.Items.Add(subitem);

            //隐藏光谱菜单
            subitem = new MenuItem();
            subitem.Name = chartMenuItems.Hide.ToString();
            subitem.Header = "隐藏光谱";
            subitem.Click += ContextMenu_Click;
            item.Items.Add(subitem);

            //网格菜单
            subitem = new MenuItem();
            subitem.Name = chartMenuItems.showGridLine.ToString();
            subitem.Header = "显示网格线";
            subitem.IsCheckable = false;
            subitem.Click += ContextMenu_Click;
            item.Items.Add(subitem);

            //标注峰位主菜单
            item = new MenuItem();
            item.Name = chartMenuItems.peakPick.ToString();
            item.Header = "标注峰位";
            item.Click += ContextMenu_Click;
            spectrumChart.ContextMenu.Items.Add(item);

            //向上峰位
            subitem = new MenuItem();
            subitem.Name = chartMenuItems.upPeakPick.ToString();
            subitem.Header = "吸收谱峰位";
            subitem.Click += ContextMenu_Click;
            item.Items.Add(subitem);

            //向下峰位菜单
            subitem = new MenuItem();
            subitem.Name = chartMenuItems.downPeakPick.ToString();
            subitem.Header = "透过谱峰位";
            subitem.Click += ContextMenu_Click;
            item.Items.Add(subitem);

            spectrumChart.ContextMenu.Opened += ContextMenu_Opened;
        }

        /// <summary>
        /// Context Menu菜单显示消息
        /// </summary>
        void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            //如果没有选择，某些菜单不能操作
            bool hasSelItems = selectedFiles != null && selectedFiles.Count > 0;
            SetMenuItemEnable(chartMenuItems.Colors, hasSelItems);
            SetMenuItemEnable(chartMenuItems.Hide, hasSelItems);
            SetMenuItemEnable(chartMenuItems.peakPick, hasSelItems);
        }

        /// <summary>
        /// 添加颜色菜单
        /// </summary>
        /// <param name="parentMenu"></param>
        /// <param name="color"></param>
        private void AddSubColorMenu(MenuItem parentMenu, string color)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1);
            if (color == "More")    //高级菜单
            {
                TextBlock txt = new TextBlock();
                txt.Text = "...";
                txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                border.Child = txt;
                border.ToolTip = "More...";
            }
            else
            {
                SolidColorBrush curColor = (SolidColorBrush)typeof(Brushes).GetProperty(color).GetValue(null, null);
                border.Background = curColor;
            }
            border.Width = 16;
            border.Height = 16;
            border.Margin = new Thickness(2);
            MenuItem menu = new MenuItem();
            menu.Template = this.Resources["SubmenuItemTemplateKey"] as ControlTemplate;
            menu.Header = border;
            menu.Name = "Color_" + color;
            menu.Click += ContextMenu_Click;

            parentMenu.Items.Add(menu);

        }

        /// <summary>
        /// 获取序号对应的颜色
        /// </summary>
        /// <param name="index">序号</param>
        /// <returns></returns>
        public static SolidColorBrush GetDisplayColor(int index)
        {
            //-1：最后一个为More，特殊用法
            return (SolidColorBrush)typeof(System.Windows.Media.Brushes).GetProperty(strColors[index % (strColors.Length-1)]).GetValue(null, null);
        }

        /// <summary>
        /// 菜单消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;

            //颜色菜单
            if (menu.Name.IndexOf("Color_") == 0)
            {
                string color = menu.Name.Replace("Color_", "");
                if(color == "More")     //More菜单，需要弹出颜色选择框
                {
                    
                }
                else if(selectedFiles.Count > 0)
                {
                    SolidColorBrush curColor = (SolidColorBrush)typeof(Brushes).GetProperty(color).GetValue(null, null);
                    foreach(var item in selectedFiles)
                    {
                        item.chart.Color = ConvertOxyColor(curColor);
                    }
                    Refresh();

                    //发送颜色选中消息
                    RoutedPropertyChangedEventArgs<SolidColorBrush> args =
                        new RoutedPropertyChangedEventArgs<SolidColorBrush>(null, curColor, ColorChangedEvent);
                    RaiseEvent(args);
                }
            }
            else
            {
                chartMenuItems curmenu = (chartMenuItems)Enum.Parse(typeof(chartMenuItems), menu.Name);
                switch (curmenu)
                {
                    case chartMenuItems.Select:     //选择
                        myController.UnbindMouseDown(OxyMouseButton.Left);
                        currentCursor = System.Windows.Input.Cursors.Arrow;
                        currentSelectedMenu = curmenu;
                        break;
                    case chartMenuItems.Pan:        //移动
                        myController.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
                        currentCursor = System.Windows.Input.Cursors.Hand;
                        currentSelectedMenu = curmenu;
                        break;
                    case chartMenuItems.zoomIn:     //放大
                        myController.BindMouseDown(OxyMouseButton.Left, PlotCommands.ZoomRectangle);
                        currentCursor = System.Windows.Input.Cursors.Cross;
                        currentSelectedMenu = curmenu;
                        break;
                    case chartMenuItems.zoomOut:    //缩小
                        spectrumChart.Model.ZoomAllAxes(0.5);
                        Refresh();
                        break;
                    case chartMenuItems.resetXY:    //重置XY
                        spectrumChart.ResetAllAxes();
                        break;
                    case chartMenuItems.resetY:     //重置Y
                        Point pt = System.Windows.Input.Mouse.GetPosition(spectrumChart);
                        ScreenPoint srcpt = new ScreenPoint(pt.X, pt.Y);
                        OxyPlot.Axes.Axis xaxis, yaxis;
                        spectrumChart.Model.GetAxesFromPoint(srcpt, out xaxis, out yaxis);
                        List<double> alldatas = new List<double>();
                        if(xaxis == null || yaxis == null)  //从按钮过来的操作，鼠标没有在图形上,取图形中间位置
                        {
                            srcpt = new ScreenPoint(spectrumChart.ActualWidth / 2, spectrumChart.ActualHeight / 2);
                            spectrumChart.Model.GetAxesFromPoint(srcpt, out xaxis, out yaxis);
                        }                        
                        if (xaxis != null && yaxis != null)
                        {
                            foreach (var item in graphicFiles)
                            {
                                List<double> curdatas = (from p in item.chart.Points where p.X >= xaxis.ActualMinimum && p.X <= xaxis.ActualMaximum select p.Y).ToList();
                                alldatas.AddRange(curdatas);
                            }
                        }
                        if (alldatas.Count > 2)
                        {
                            yaxis.Zoom(alldatas.Min(), alldatas.Max());
                            Refresh();
                        }

                        break;
                    case chartMenuItems.showGridLine:   //显示关闭网格
                        foreach (var item in spectrumChart.Model.Axes)
                        {
                            if (item.MajorGridlineStyle == LineStyle.None)
                            {
                                item.MajorGridlineStyle = LineStyle.Solid;
                                item.MajorGridlineThickness = 1.0;
                                item.MajorGridlineColor = OxyColor.FromArgb(0x60, 0, 0, 0);
                            }
                            else
                                item.MajorGridlineStyle = LineStyle.None;
                        }
                        Refresh();

                        break;
                    case chartMenuItems.showInformation:    //显示信息
                        currentCursor = LoadCustomCursor("Ai.Hong.Controls.Images.SelectInformation.cur");
                        myController.BindMouseDown(OxyMouseButton.Left, PlotCommands.HoverSnapTrack);
                        currentSelectedMenu = curmenu;
                        break;
                    case chartMenuItems.Hide:               //隐藏选中的光谱
                        //从图形中隐藏
                        foreach(var item in selectedFiles)
                        {
                            spectrumChart.ActualModel.Series.Remove(item.chart);
                            graphicFiles.Remove(item);
                        }

                        //发送图形隐藏消息
                        List<Guid> removedItems = (from p in selectedFiles select p.key).ToList();
                        List<Guid> addedItems = new List<Guid>();
                        System.Windows.Controls.SelectionChangedEventArgs selArgs = new SelectionChangedEventArgs(ItemHiddenEvent, removedItems, addedItems);
                        RaiseEvent(selArgs);
                        selectedFiles.Clear();
                        Refresh();
                        break;
                    case chartMenuItems.upPeakPick:     //标上峰位
                        currentCursor = LoadCustomCursor("CommonLibrary.Images.UpArrowCursor.cur");
                        currentSelectedMenu = curmenu;
                        break;
                    case chartMenuItems.downPeakPick:   //标下峰位
                        currentCursor = LoadCustomCursor("CommonLibrary.Images.DownArrowCursor.cur");
                        currentSelectedMenu = curmenu;
                        break;
                }
                //spectrumChart.Cursor = currentCursor;
                //spectrumChart.ForceCursor = true;
                this.Cursor = currentCursor;
                this.ForceCursor = true;
            }
        }

        /// <summary>
        /// 加载光标
        /// </summary>
        private System.Windows.Input.Cursor LoadCustomCursor(string curfile)
        {
            //这里必须要使用GetExecutingAssembly才能获取Assembly
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            //鼠标必须设置为嵌入式资源才行
            System.IO.Stream stream = asm.GetManifestResourceStream(curfile);
            return new System.Windows.Input.Cursor(stream);
        }

        /// <summary>
        /// 设置显示坐标
        /// </summary>
        private void InitChart()
        {
            spectrumChart.Model = new PlotModel();

            //x轴倒叙
            xAxs = new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, StartPosition = 1, EndPosition = 0 };
            spectrumChart.ActualModel.Axes.Add(xAxs);
         
            spectrumChart.Model.SelectionColor = OxyColor.FromArgb(255, 0, 0, 0);

            spectrumChart.MouseDoubleClick += spectrumChart_MouseDoubleClick;
            spectrumChart.Controller = myController;
            myController.UnbindMouseDown(OxyMouseButton.Right);
            myController.UnbindMouseDown(OxyMouseButton.Left);
            //spectrumChart.IsMouseCapturedChanged += spectrumChart_IsMouseCapturedChanged;
            InitMouseDownEvent();

        }

        void spectrumChart_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (spectrumChart.IsMouseCaptured == false)
            {
                spectrumChart.Cursor = currentCursor;
                spectrumChart.ForceCursor = true;

                this.Cursor = currentCursor;
                this.ForceCursor = true;
            }
        }

        /// <summary>
        /// 鼠标左键按下的相应信息
        /// </summary>
        private bool leftButtonDown = false;
        private double oldx, oldy, newx, newy;
        private bool zoomRectangleVisible = false;

        private chartMenuItems currentSelectedMenu = chartMenuItems.Select;

        private void InitMouseDownEvent()
        {
            spectrumChart.Model.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left && currentSelectedMenu == chartMenuItems.Select)
                {
                    leftButtonDown = true;
                    oldx = e.Position.X;
                    oldy = e.Position.Y;
                    spectrumChart.CaptureMouse();
                }
            };
            spectrumChart.Model.MouseMove += (s, e) =>
            {
                if (leftButtonDown && currentSelectedMenu == chartMenuItems.Select)
                {
                    //鼠标有移动才显示选择框
                    if (Math.Abs(e.Position.X - oldx) > 2 || Math.Abs(e.Position.Y - oldy) > 2)
                    {
                        newx = e.Position.X;
                        newx = newx < spectrumChart.Model.PlotArea.Left ? spectrumChart.Model.PlotArea.Left : newx;
                        newx = newx > spectrumChart.Model.PlotArea.Right ? spectrumChart.Model.PlotArea.Right : newx;

                        newy = e.Position.Y;
                        newy = newy < spectrumChart.Model.PlotArea.Top ? spectrumChart.Model.PlotArea.Top : newy;
                        newy = newy > spectrumChart.Model.PlotArea.Bottom ? spectrumChart.Model.PlotArea.Bottom : newy;

                        spectrumChart.ShowZoomRectangle(OxyRect.Create(oldx, oldy, newx, newy));
                        zoomRectangleVisible = true;
                    }
                }
            };

            spectrumChart.Model.MouseUp += (s, e) =>
            {
                if (leftButtonDown && currentSelectedMenu == chartMenuItems.Select)
                {
                    //按住Control, Shift多选
                    bool needClear = (e.ModifierKeys & OxyPlot.OxyModifierKeys.Shift) == 0 && 
                        (e.ModifierKeys & OxyPlot.OxyModifierKeys.Control) == 0;

                    bool needRefresh = false;
                    Guid curselItem = Guid.Empty;

                    spectrumChart.ReleaseMouseCapture();
                    if (zoomRectangleVisible)
                    {
                        spectrumChart.HideZoomRectangle();
                        var selSeries = SeriesInRectangle(oldx, oldy, newx, newy);
                        var ids = (from p in graphicFiles where selSeries.Contains(p.chart) select p.key).ToList();
                        needRefresh = AddNewSelectedItems(ids, needClear);
                    }
                    else
                    {
                        var selSr = spectrumChart.Model.GetSeriesFromPoint(e.Position, selectSensitiy);
                        if (selSr != null)
                        {
                            var ids = (from p in graphicFiles where p.chart == selSr select p.key ).ToList();
                            needRefresh = AddNewSelectedItems(ids, needClear);                   
                        }
                    }
                    if (needRefresh)
                    {
                        Refresh();
                        List<Guid> addedItems = (from p in selectedFiles select p.key).ToList();
                        List<Guid> removedItems = new List<Guid>();
                        if(addedItems.Count > 0)
                            SetValue(SelectedItemProperty, addedItems[addedItems.Count-1]);
                        else
                            SetValue(SelectedItemProperty, Guid.Empty);

                        System.Windows.Controls.SelectionChangedEventArgs selArgs = new SelectionChangedEventArgs(ItemSelectedEvent, removedItems, addedItems);
                        RaiseEvent(selArgs);
                    }
                }
                leftButtonDown = false;
                zoomRectangleVisible = false;
            };
        }

        /// <summary>
        /// 获取矩形选择框内的图形
        /// </summary>
        private List<OxyPlot.Series.LineSeries> SeriesInRectangle(double x0, double y0, double x1, double y1)
        {
            ScreenPoint screenBegin = new ScreenPoint(x0, y0);
            ScreenPoint screenEnd = new ScreenPoint(x1, y1);

            List<LineSeries> selSeries = new List<LineSeries>();

            foreach(var item in spectrumChart.Model.Series)
            {
                LineSeries linesr = item as LineSeries;

                if (linesr != null && linesr.Points != null && linesr.Points.Count > 0)
                {
                    var begin = linesr.InverseTransform(screenBegin);
                    var end = linesr.InverseTransform(screenEnd);

                    if (begin.X > end.X)
                    {
                        double tempx = begin.X;
                        begin.X = end.X;
                        end.X = tempx;
                    }
                    if (begin.Y > end.Y)
                    {
                        double tempy = begin.Y;
                        begin.Y = end.Y;
                        end.Y = tempy;
                    }

                    Rect selrc = new Rect(begin.X, begin.Y, end.X - begin.X, end.Y - begin.Y);
                    Rect allrc = new Rect(linesr.MinX, linesr.MinY, linesr.MaxX - linesr.MinX, linesr.MaxY - linesr.MinY);
                    //if (linesr.MinX <= end.X && linesr.MaxX >= end.X && linesr.MinY <= begin.Y && linesr.MaxY >= end.Y)
                    if(selrc.IntersectsWith(allrc))
                    {
                        //int index = linesr.Points.FindIndex(p=> p.X>=begin.X && p.X<=end.X && p.Y >=begin.Y && p.Y <=end.Y);
                        int index = linesr.Points.FindIndex(p => selrc.Contains(p.X, p.Y));
                        if(index >=0)
                        {
                            selSeries.Add(linesr);
                        }
                    }
                }
            }

            return selSeries;
        }

        /// <summary>
        /// 鼠标双击消息
        /// </summary>
        void spectrumChart_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point mousept = e.GetPosition(spectrumChart);
            ScreenPoint pt = new ScreenPoint(mousept.X, mousept.Y);

            HitTestArguments args = new HitTestArguments(pt, 2);
            bool needrefresh = false;
            LineSeries selSeries = spectrumChart.Model.GetSeriesFromPoint(pt, 2) as LineSeries;

            //找到图形对应的类
            GraphicInfo info = graphicFiles.FirstOrDefault(p=>p.chart == selSeries);
            //按下了Ctrl键，多选

            List<Guid> addedItems = new List<Guid>();
            List<Guid> removedItems = new List<Guid>();
            System.Windows.Controls.SelectionChangedEventArgs selArgs = new SelectionChangedEventArgs(ItemSelectedEvent, removedItems, addedItems);

            
            if ((System.Windows.Input.Keyboard.GetKeyStates(System.Windows.Input.Key.LeftShift) & System.Windows.Input.KeyStates.Down) > 0 ||
                (System.Windows.Input.Keyboard.GetKeyStates(System.Windows.Input.Key.RightShift) & System.Windows.Input.KeyStates.Down) > 0)
            {
                //有刚刚选中的图形，如果没在selectedFiles中，添加
                if (selSeries != null && selectedFiles.FirstOrDefault(p => p.chart == selSeries) == null && info != null)
                {
                    selSeries.StrokeThickness = SelectedLineWidth;
                    selectedFiles.Add(info);
                    addedItems.Add(info.key);   //选择项
                    needrefresh = true;
                }
            }
            else    //没有Ctrl键，单选
            {
                //如果没有选中图形
                if (info == null)
                {
                    //清除以前选中的图形
                    foreach (var item in selectedFiles)
                    {
                        item.chart.StrokeThickness = LineWidth;

                        //移除项
                        removedItems.Add(item.key);
                    }
                    if(selectedFiles.Count > 0)
                    {
                        selectedFiles.Clear();
                        needrefresh = true;
                    }
                }
                else  //只保留当前选中的图形
                {
                    //如果selectedFiles还有其它图形
                    selSeries.StrokeThickness = SelectedLineWidth;
                    if (selectedFiles.FirstOrDefault(p => p.chart != selSeries) != null)
                    {
                        foreach(var item in selectedFiles)
                        {
                            if (item.chart != selSeries)
                            {
                                item.chart.StrokeThickness = LineWidth;

                                //移除项
                                removedItems.Add(item.key);
                            }
                        }

                        //移除其它图形
                        selectedFiles.RemoveAll(p => p.chart != selSeries);
                        needrefresh = true;
                    }

                    //如果没在selectedFiles，添加
                    if (selectedFiles.FirstOrDefault(p => p.chart == selSeries) == null)
                    {
                        selectedFiles.Add(info);

                        addedItems.Add(info.key);   //添加项
                        needrefresh = true;
                    }
                }
            }

            if (needrefresh)
            {
                Refresh();
                SetValue(SelectedItemProperty, info == null ? Guid.Empty : info.key);
                RaiseEvent(selArgs);
            }
        }

        /// <summary>
        /// 设置图形线宽
        /// </summary>
        /// <param name="items"></param>
        /// <param name="linewidth"></param>
        private void SetChartLineWidth(List<GraphicInfo> items, double linewidth)
        {
            foreach (var item in items)
                item.chart.StrokeThickness = linewidth;
        }

        /// <summary>
        /// 添加到选择图形列表
        /// </summary>
        /// <param name="selItems">新添加的图形</param>
        /// <param name="clear">清除以前的图形</param>
        /// <returns>是否需要刷新</returns>
        private bool AddNewSelectedItems(List<Guid> selItems, bool clear)
        {
            if(selItems == null)
                return false;

            bool needrefresh = false;
            selItems = selItems.Distinct().ToList();
            //只添加当前显示列表中有的图形
            List<GraphicInfo> selInfos = (from p in graphicFiles where selItems.FirstOrDefault(k => k== p.key) != Guid.Empty select p).ToList();

            //需要清除原有选择项
            if(clear)
            {
                //没有新选择项
                if (selInfos.Count == 0)
                {
                    needrefresh = selectedFiles.Count > 0;
                    SetChartLineWidth(selectedFiles, LineWidth);
                    selectedFiles.Clear();
                }
                else
                {
                    //新列表和原有列表数量相同
                    if (selectedFiles.Count == selInfos.Count)
                    {                        
                        //如果在当前列表没有找到新增项，需要刷新
                        foreach (var item in selInfos)
                        {
                            if (selectedFiles.FirstOrDefault(p => p == item) == null)
                                needrefresh = true;
                        }
                    }

                    if (selectedFiles.Count != selInfos.Count || needrefresh)
                    {
                        SetChartLineWidth(selectedFiles, LineWidth);
                        selectedFiles.Clear();
                        selectedFiles.AddRange(selInfos);
                        SetChartLineWidth(selectedFiles, SelectedLineWidth);
                        needrefresh = true;
                    }
                }
            }
            else    //添加到选择项中
            {
                SetChartLineWidth(selInfos, SelectedLineWidth);

                //如果在当前列表没有找到新增项，需要刷新
                foreach (var item in selInfos)
                {
                    if (selectedFiles.FirstOrDefault(p => p == item) == null)
                    {
                        selectedFiles.Add(item);
                        needrefresh = true;
                    }
                }

            }

            return needrefresh;
        }

        /// <summary>
        /// 实际添加光谱图形到Chart
        /// </summary>
        private bool RealAddToChart(GraphicInfo info)
        {
            if (info.chart == null)
                return false;

            graphicFiles.Add(info);
            spectrumChart.ActualModel.Series.Add(info.chart);

            AdjustAxisValue();

            return true;
        }

        /// <summary>
        /// 通过名称查找ItemsControl包含的子节点
        /// </summary>
        /// <param name="currentNode">当前结点</param>
        /// <param name="name">查找的名称</param>
        /// <returns>找到的节点</returns>
        private ItemsControl FindChildByName(ItemsControl currentNode, string name)
        {
            if (currentNode == null || string.IsNullOrEmpty(name))
                return null;

            if (currentNode.Name == name)
                return currentNode;

            if (currentNode.Items == null || currentNode.Items.Count == 0)
                return null;

            ItemsControl foundItem = null;
            foreach (var item in currentNode.Items)
            {
                foundItem = FindChildByName(item as ItemsControl, name);
                if (foundItem != null)
                    return foundItem;
            }

            return foundItem;
        }

        /// <summary>
        /// 修改弹出菜单的名称
        /// </summary>
        /// <param name="item">菜单项</param>
        /// <param name="caption">菜单名称</param>
        public void SetMenuItemCaption(chartMenuItems item, string caption)
        {
            string menuName = item.ToString();
            MenuItem menu = FindChildByName(spectrumChart.ContextMenu,menuName) as MenuItem;
            if (menu != null)
            {
                menu.Header = caption;
                if (operateButtons.ContainsKey(item))
                    operateButtons[item].ToolTip = caption;
            }
        }

        /// <summary>
        /// 隐藏或者显示弹出菜单
        /// </summary>
        /// <param name="item">菜单项</param>
        /// <param name="visible">True=显示, False=隐藏</param>
        public void SetMenuItemVisible(chartMenuItems item, bool visible)
        {
            string menuName = item.ToString();
            MenuItem menu = FindChildByName(spectrumChart.ContextMenu, menuName) as MenuItem;
            if (menu != null)
            {
                menu.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
                if (operateButtons.ContainsKey(item))
                    operateButtons[item].Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Enable弹出菜单
        /// </summary>
        /// <param name="item">菜单项</param>
        /// <param name="enable">Tru, False</param>
        public void SetMenuItemEnable(chartMenuItems item, bool enable)
        {
            string menuName = item.ToString();
            MenuItem menu = FindChildByName(spectrumChart.ContextMenu, menuName) as MenuItem;
            if (menu != null)
            {
                menu.IsEnabled = enable;
                if (operateButtons.ContainsKey(item))
                    operateButtons[item].IsEnabled = enable;
            }
        }

        /// <summary>
        /// 添加图形
        /// </summary>
        /// <param name="xDatas">x轴数据</param>
        /// <param name="yDatas">y轴数据</param>
        /// <param name="color">显示颜色</param>
        /// <param name="key">文件标识</param>
        /// <param name="label">图形名称</param>
        /// <param name="labelFormat">数据显示格式</param>
        public bool AddChart(double[] xDatas, double[] yDatas, SolidColorBrush color, Guid key, string label = null, string labelFormat="F2")
        {
            //已经添加了
            if (graphicFiles.Find(item => item.key == key) != null)
                return true;
            return RealAddToChart(new GraphicInfo(xDatas, yDatas, color, LineWidth, key, label, labelFormat));
        }

        private OxyColor ConvertOxyColor(SolidColorBrush color)
        {
            return OxyColor.FromArgb(color.Color.A, color.Color.R, color.Color.G,color.Color.B);
        }

        /// <summary>
        /// Convert Oxy to solid brush
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private SolidColorBrush ConvertToSolidBrush(OxyColor color)
        {
            var cl = new Color() { A = color.A, R = color.R, G = color.G, B = color.B };
            return new SolidColorBrush(cl);
        }

        /// <summary>
        /// 设置X轴和Y轴的最大最小值
        /// </summary>
        private void AdjustAxisValue()
        {
            //限制移动的范围

            //x轴
            //axis = spectrumChart.Model.Axes.FirstOrDefault(p => p.IsHorizontal() == true);
            //if (graphicFiles.Count > 0)
            //{
            //    xAxs.AbsoluteMaximum = (from item in graphicFiles select item.para.lastX).Max();
            //    xAxs.AbsoluteMinimum = (from item in graphicFiles select item.para.firstX).Min();
            //}
        }

        private void RaiseSelectionChangedEvent()
        {

        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="key">光谱的ID</param>
        public void RemoveChart(Guid key)
        {
            if (key == Guid.Empty)
                return;

            GraphicInfo info =  graphicFiles.Find(item => item.key == key);
            if (info != null)
            {
                spectrumChart.ActualModel.Series.Remove(info.chart);
                graphicFiles.Remove(info);

                //是否需要发送选择变动消息
                bool needEvent = selectedFiles.FirstOrDefault(p => p.key == key) != null;
                if(needEvent)
                {
                    selectedFiles.Remove(info);
                    //移除当前选项，当前选项设置为Empty
                    if (key == SelectedItem)
                    {
                        SetValue(SelectedItemProperty, Guid.Empty);
                    }
                    RaiseSelectionChangedEvent();
                }

                Refresh();
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="keys">光谱的ID</param>
        public void RemoveCharts(IList<Guid> keys)
        {
            if (keys == null || keys.Count ==0)
                return;

            bool needEvent = false;
            bool needRefresh = false;
            foreach (var key in keys)
            {
                GraphicInfo info = graphicFiles.Find(item => item.key == key);
                if (info != null)
                {
                    //从图形中移除
                    spectrumChart.ActualModel.Series.Remove(info.chart);
                    graphicFiles.Remove(info);

                    //是否需要发送选择变动消息
                    if (selectedFiles.FirstOrDefault(p => p.key == key) != null)
                    {
                        needEvent = true;
                        selectedFiles.Remove(info);
                    }
                    //移除当前选项，当前选项设置为Empty
                    if (key == SelectedItem)
                    {
                        SetValue(SelectedItemProperty, Guid.Empty);
                        needEvent = true;
                    }

                    needRefresh = true;
                }
            }

            if (needRefresh)
                Refresh();

            if (needEvent)
                RaiseSelectionChangedEvent();
        }


        /// <summary>
        /// 删除所有光谱
        /// </summary>
        public void RemoveAllChart()
        {
            if (graphicFiles.Count == 0)
                return;

            spectrumChart.ActualModel.Series.Clear();
            graphicFiles.Clear();

            //清除选择项
            if(selectedFiles.Count > 0)
            {
                selectedFiles.Clear();
                SetValue(SelectedItemProperty, Guid.Empty);
                RaiseSelectionChangedEvent();
            }

            Refresh();
        }

        /// <summary>
        /// 设置新的显示颜色
        /// </summary>
        /// <param name="key">替换的对象</param>
        /// <param name="newcolor">显示颜色</param>
        public void ChangeColor(Guid key, SolidColorBrush newcolor)
        {
            GraphicInfo info = graphicFiles.Find(item => item.key == key);
            if (info != null)
            {
                info.chart.Color = OxyColor.FromArgb(newcolor.Color.A, newcolor.Color.R, newcolor.Color.G, newcolor.Color.B);
                Refresh();
            }
        }

        /// <summary>
        /// 设置新的显示颜色
        /// </summary>
        /// <param name="keys">替换的对象</param>
        /// <param name="newcolor">显示颜色</param>
        public void ChangeColor(List<Guid> keys, SolidColorBrush newcolor)
        {
            if (keys == null || keys.Count == 0)
                return;

            foreach (var key in keys)
            {
                GraphicInfo info = graphicFiles.Find(item => item.key == key);
                if (info != null)
                {
                    info.chart.Color = OxyColor.FromArgb(newcolor.Color.A, newcolor.Color.R, newcolor.Color.G, newcolor.Color.B);
                }
            }
            Refresh();
        }

        /// <summary>
        /// 刷新图像
        /// </summary>
        public void Refresh()
        {
            //如果没有选择，某些菜单不能操作
            bool hasSelItems = selectedFiles != null && selectedFiles.Count > 0;
            SetMenuItemEnable(chartMenuItems.Colors, hasSelItems);
            SetMenuItemEnable(chartMenuItems.Hide, hasSelItems);
            SetMenuItemEnable(chartMenuItems.peakPick, hasSelItems);

            spectrumChart.InvalidatePlot();
        }

        /// <summary>
        /// 重置图形的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="xDatas"></param>
        /// <param name="yDatas"></param>
        public void RedrawItem(Guid key, double[] xDatas, double[] yDatas)
        {
            if (xDatas == null || yDatas == null || xDatas.Length < 1 || xDatas.Length != yDatas.Length)
                return;

            var item = graphicFiles.FirstOrDefault(p => p.key == key);
            if (item == null)
                return;

            item.chart.Points.Clear();
            for(int i=0; i<xDatas.Length; i++)
            {
                item.chart.Points.Add(new DataPoint(xDatas[i], yDatas[i]));
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);      //清晰显示
        }

        /// <summary>
        /// 设置图形操作按钮面板
        /// </summary>
        /// <param name="panel">图形操作按钮面板</param>
        public void SetGraphicOperatePanel(GraphicOperatePanel panel)
        {
            operatePanel = panel;

            if(panel != null)
            {
                panel.ButtonChecked += operatePanel_ButtonChecked;
                panel.ButtonClicked += operatePanel_ButtonClicked;

                //创建操作按钮和菜单的对应关系
                operateButtons = new Dictionary<chartMenuItems, System.Windows.Controls.Primitives.ButtonBase>();
                var allbuttons = panel.GetAllButtons();
                foreach (var btn in allbuttons)
                {
                    chartMenuItems menuItem = chartMenuItems.Display;
                    if (btn == panel.btnSelect)
                        menuItem = chartMenuItems.Select;
                    else if (btn == panel.btnMove)
                        menuItem = chartMenuItems.Pan;
                    else if (btn == panel.btnZoomIn)
                        menuItem = chartMenuItems.zoomIn;
                    else if (btn == panel.btnZoomOut)
                        menuItem = chartMenuItems.zoomOut;
                    else if (btn == panel.btnInformation)
                        menuItem = chartMenuItems.showInformation;
                    else if (btn == panel.btnUpPeakPick)
                        menuItem = chartMenuItems.upPeakPick;
                    else if (btn == panel.btnDownPeakPick)
                        menuItem = chartMenuItems.downPeakPick;
                    else if (btn == panel.btnSizeAll)
                        menuItem = chartMenuItems.resetXY;
                    else if (btn == panel.btnSizeYAxis)
                        menuItem = chartMenuItems.resetY;
                    else if (btn == panel.btnColor)
                        menuItem = chartMenuItems.Colors;
                    else if (btn == panel.btnHide)
                        menuItem = chartMenuItems.Hide;
                    else if (btn == panel.btnGridShow)
                        menuItem = chartMenuItems.showGridLine;
                    else
                        continue;

                    operateButtons.Add(menuItem, btn);
                }

                //刷新菜单状态
                bool hasSelItems = selectedFiles != null && selectedFiles.Count > 0;
                SetMenuItemEnable(chartMenuItems.Colors, hasSelItems);
                SetMenuItemEnable(chartMenuItems.Hide, hasSelItems);
                SetMenuItemEnable(chartMenuItems.peakPick, hasSelItems);

                //按钮和菜单间的属性绑定
                foreach (var item in operateButtons)
                {
                    MenuItem menu = FindChildByName(spectrumChart.ContextMenu, item.Key.ToString()) as MenuItem;
                    if(menu != null)
                    {
                        //刷新按钮属性
                        item.Value.IsEnabled = menu.IsEnabled;
                        item.Value.ToolTip = menu.Header;
                        item.Value.Visibility = menu.Visibility;
                        item.Value.ToolTip = menu.Header;

                        if (item.Value is RadioButton)
                        {
                            (item.Value as RadioButton).IsChecked = menu.IsChecked;

                            Binding checkbind = new Binding("IsChecked");
                            checkbind.Mode = BindingMode.TwoWay;
                            checkbind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            menu.IsCheckable = true;
                            menu.SetBinding(MenuItem.IsCheckedProperty, checkbind);
                        }

                        Binding bind = new Binding("IsEnabled");
                        bind.Mode = BindingMode.TwoWay;
                        bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        menu.SetBinding(MenuItem.IsEnabledProperty, bind);

                        bind = new Binding("Visibility");
                        bind.Mode = BindingMode.TwoWay;
                        bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        menu.SetBinding(MenuItem.VisibilityProperty, bind);

                        bind = new Binding("ToolTip");
                        bind.Mode = BindingMode.TwoWay;
                        bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        menu.SetBinding(MenuItem.ToolTipProperty, bind);

                        menu.DataContext = item.Value;
                    }
                }

                operatePanel.ColorChanged += OperatePanelChartColor_Changed;
            }
            else
                operateButtons = new Dictionary<chartMenuItems, System.Windows.Controls.Primitives.ButtonBase>();
        }

        /// <summary>
        /// 按钮面板的颜色改变消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OperatePanelChartColor_Changed(object sender, RoutedPropertyChangedEventArgs<SolidColorBrush> e)
        {
            if (e == null || e.NewValue == null)
                return;

            foreach (var item in selectedFiles)
            {
                item.chart.Color = ConvertOxyColor(e.NewValue);
            }
            Refresh();

            //发送颜色选中消息
            RoutedPropertyChangedEventArgs<SolidColorBrush> args =
                new RoutedPropertyChangedEventArgs<SolidColorBrush>(null, e.NewValue, ColorChangedEvent);
            RaiseEvent(args);            
        }

        /// <summary>
        /// 操作按钮的按钮Click消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void operatePanel_ButtonClicked(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;
            if (btn == null)
                return;
            if (!operateButtons.ContainsValue(btn))
                return;
            chartMenuItems menutype = operateButtons.First(p => p.Value == btn).Key;
            MenuItem menu = FindChildByName(spectrumChart.ContextMenu, menutype.ToString()) as MenuItem;
            if (menu != null)
            {
                ContextMenu_Click(menu, null);
            }
        }

        /// <summary>
        /// 操作按钮的RadioButton选中消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void operatePanel_ButtonChecked(object sender, RoutedEventArgs e)
        {
            RadioButton btn = e.OriginalSource as RadioButton;
            if (btn == null)
                return;
            if(!operateButtons.ContainsValue(btn))
                return;
            chartMenuItems menutype = operateButtons.First(p=>p.Value == btn).Key;
            MenuItem menu = FindChildByName(spectrumChart.ContextMenu, menutype.ToString()) as MenuItem;
            if (menu != null && btn.IsChecked == true)
            {
                ContextMenu_Click(menu, null);
            }            
        }

        /// <summary>
        /// 获取显示图形的数量
        /// </summary>
        /// <returns></returns>
        public int GetDisplayChartsCount()
        {
            return graphicFiles.Count;
        }

        /// <summary>
        /// 获取所有显示的图形
        /// </summary>
        /// <returns></returns>
        public List<Guid> GetDisplayCharts()
        {
            return (from p in graphicFiles select p.key).ToList();
        }

        /// <summary>
        /// 保存到图像文件
        /// </summary>
        /// <param name="filename">图像文件名</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="backgroundColor">背景颜色</param>
        public void SaveToBitmapFile(string filename, int width, int height, SolidColorBrush backgroundColor)
        {
            spectrumChart.SaveBitmap(filename, width, height, ConvertOxyColor(backgroundColor));
        }

    }

}
