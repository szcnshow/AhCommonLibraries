using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Series;

namespace Ai.Hong.Charts
{
    /// <summary>
    /// 散点图，也包括折线，线段等
    /// </summary>
    public partial class ScatterChart : UserControl
    {


        #region properties
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
        /// 所有操作按钮
        /// </summary>
        Dictionary<ChartMenuItems, System.Windows.Controls.Primitives.ButtonBase> operateButtons = new Dictionary<ChartMenuItems, System.Windows.Controls.Primitives.ButtonBase>();

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

        /*
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
                if (selectedFiles.Count == 0 || selectedFiles.FirstOrDefault(p => p.key == SelectedItem) == null)
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
            set { if (value >= 1.0) SetValue(selectSensitiyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for selectSensitiy.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 图像选择的敏感度
        /// </summary>
        public static readonly DependencyProperty selectSensitiyProperty =
            DependencyProperty.Register("selectSensitiy", typeof(double), typeof(SpectrumChart), new PropertyMetadata(2.0));
            */
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ScatterChart()
        {
            InitializeComponent();
            InitChart();
        }

        /// <summary>
        /// 设置显示坐标
        /// </summary>
        private void InitChart()
        {
            DrawingChart.Model = new PlotModel();

            //x轴倒叙
            xAxs = new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, StartPosition = 0, EndPosition = 1 };
            DrawingChart.ActualModel.Axes.Add(xAxs);

            DrawingChart.Model.SelectionColor = OxyColor.FromArgb(255, 0, 0, 0);

            //DrawingChart.MouseDoubleClick += spectrumChart_MouseDoubleClick;
            //DrawingChart.Controller = myController;
            //myController.UnbindMouseDown(OxyMouseButton.Right);
            //myController.UnbindMouseDown(OxyMouseButton.Left);

            //spectrumChart.IsMouseCapturedChanged += spectrumChart_IsMouseCapturedChanged;
            //InitMouseDownEvent();

        }
        /// <summary>
        /// 创建ScatterSeries图像
        /// </summary>
        /// <param name="chartID">图形ID</param>
        /// <param name="chartName">图形名称</param>
        /// <param name="xDatas">x轴数据</param>
        /// <param name="yDatas">y轴数据</param>
        /// <param name="markerType">数据点形状，Default=Circle</param>
        /// <param name="markerSize">数据点大小,Default=10.0</param>
        /// <param name="borderColor">边框颜色，Default=Blue</param>
        /// <param name="fillColor">填充颜色，Default=Transparent</param>
        /// <param name="lineWidth">边框线宽，Default=1.0</param>
        /// <param name="labelFormat">数据显示格式, Default=F2</param>
        /// <returns></returns>
        public void AddScatterChart(Guid chartID, string chartName, double[] xDatas, double[] yDatas, EnumMarkerType markerType= EnumMarkerType.Circle, double markerSize=5.0, SolidColorBrush borderColor=null, SolidColorBrush fillColor=null, double lineWidth=1.0, string labelFormat="F2")
        {
            if (borderColor == null)
                borderColor = Brushes.Blue;
            if (fillColor == null)
                fillColor = Brushes.Transparent;

            var chart = GraphicInfo.CreateScatterSeries(xDatas, yDatas, markerType, markerSize, borderColor, fillColor, lineWidth);

            RealAddToChart(new GraphicInfo(chartID, chart, EnumChartType.ScatterSeries, chartName, labelFormat));
        }

        /// <summary>
        /// 实际添加光谱图形到Chart
        /// </summary>
        private bool RealAddToChart(GraphicInfo info)
        {
            if (info.Chart == null)
                return false;

            graphicFiles.Add(info);
            DrawingChart.ActualModel.Series.Add(info.Chart);

            return true;
        }

        /// <summary>
        /// 刷新图像
        /// </summary>
        public void Refresh()
        {
            DrawingChart.InvalidatePlot();
        }
    }
}
