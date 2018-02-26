using nzy3D.Chart;
using nzy3D.Colors;
using nzy3D.Colors.ColorMaps;
using nzy3D.Maths;
using nzy3D.Plot3D.Builder;
using nzy3D.Plot3D.Builder.Concrete;
using nzy3D.Plot3D.Primitives;
using nzy3D.Plot3D.Primitives.Axes.Layout;
using nzy3D.Plot3D.Rendering.Canvas;
using nzy3D.Plot3D.Rendering.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Ai.Hong.Charts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ThreedChart : UserControl
    {
        private nzy3D.Chart.Controllers.Thread.Camera.CameraThreadController threadController;
        nzy3D.Chart.Chart dataChart = null;
        private IAxeLayout axeLayout;

        /// <summary>
        /// 几何形状信息
        /// </summary>
        private class ShapeInfo
        {
            /// <summary>
            /// 数据
            /// </summary>
            public PointData data { get; set; }
            /// <summary>
            /// key
            /// </summary>
            public Guid key { get; set; }
            /// <summary>
            /// 几何形状
            /// </summary>
            public Shape shape { get; set; }

            /// <summary>
            /// 通过key获取ShapeInfo
            /// </summary>
            /// <param name="key"></param>
            /// <param name="shapeInfos"></param>
            /// <returns></returns>
            public static ShapeInfo GetShapeInfo(Guid key, IList<ShapeInfo> shapeInfos)
            {
                if (shapeInfos == null)
                    return null;

                return shapeInfos.FirstOrDefault(p => p.key == key);
            }

            /// <summary>
            /// 通过key获取Shape
            /// </summary>
            /// <param name="key"></param>
            /// <param name="shapeInfos"></param>
            /// <returns></returns>
            public static Shape GetShape(Guid key, IList<ShapeInfo> shapeInfos)
            {
                var info = GetShapeInfo(key, shapeInfos);
                return info == null ? null : info.shape;
            }
        }

        #region ChartDisplayProperties
        /// <summary>
        /// 显示坐标线
        /// </summary>
        public bool DisplayTickLines
        {
            get { return (bool)GetValue(DisplayTickLinesProperty); }
            set
            {
                SetValue(DisplayTickLinesProperty, value);
                //OnPropertyChanged("DisplayTickLines");
                if (axeLayout != null)
                {
                    axeLayout.TickLineDisplayed = value;
                }
            }
        }
        /// <summary>
        /// 显示坐标线
        /// </summary>
        public static readonly DependencyProperty DisplayTickLinesProperty =
            DependencyProperty.Register("DisplayTickLines", typeof(bool), typeof(ThreedChart), new PropertyMetadata(false));

        /// <summary>
        /// 显示X轴坐标刻度
        /// </summary>
        public bool DisplayXTicks
        {
            get { return (bool)GetValue(DisplayXTicksProperty); }
            set
            {
                SetValue(DisplayXTicksProperty, value);
                //OnPropertyChanged("DisplayXTicks");
                if (axeLayout != null)
                {
                    axeLayout.XTickLabelDisplayed = value;
                }
            }
        }
        /// <summary>
        /// 显示X轴坐标刻度
        /// </summary>
        public static readonly DependencyProperty DisplayXTicksProperty =
            DependencyProperty.Register("DisplayXTicks", typeof(bool), typeof(ThreedChart), new PropertyMetadata(false));


        /// <summary>
        /// 显示Y轴坐标刻度
        /// </summary>
        public bool DisplayYTicks
        {
            get { return (bool)GetValue(DisplayYTicksProperty); }
            set
            {
                SetValue(DisplayYTicksProperty, value);
                //OnPropertyChanged("DisplayYTicks");
                if (axeLayout != null)
                {
                    axeLayout.YTickLabelDisplayed = value;
                }
            }
        }
        /// <summary>
        /// 显示Y轴坐标刻度
        /// </summary>
        public static readonly DependencyProperty DisplayYTicksProperty =
            DependencyProperty.Register("DisplayYTicks", typeof(bool), typeof(ThreedChart), new PropertyMetadata(false));

        /// <summary>
        /// 显示Z轴坐标刻度
        /// </summary>
        public bool DisplayZTicks
        {
            get { return (bool)GetValue(DisplayZTicksProperty); }
            set
            {
                SetValue(DisplayZTicksProperty, value);
                //OnPropertyChanged("DisplayZTicks");
                if (axeLayout != null)
                {
                    axeLayout.ZTickLabelDisplayed = value;
                }
            }
        }
        /// <summary>
        /// 显示Z轴坐标刻度
        /// </summary>
        public static readonly DependencyProperty DisplayZTicksProperty =
            DependencyProperty.Register("DisplayZTicks", typeof(bool), typeof(ThreedChart), new PropertyMetadata(false));

        /// <summary>
        /// 显示X轴坐标刻度
        /// </summary>
        public bool DisplayXAxisLabel
        {
            get { return (bool)GetValue(DisplayXAxisLabelProperty); }
            set
            {
                SetValue(DisplayXAxisLabelProperty, value);
                //OnPropertyChanged("DisplayXAxisLabel");
                if (axeLayout != null)
                {
                    axeLayout.XAxeLabelDisplayed = value;
                }
            }
        }
        /// <summary>
        /// 显示X轴坐标刻度
        /// </summary>
        public static readonly DependencyProperty DisplayXAxisLabelProperty =
            DependencyProperty.Register("DisplayXAxisLabel", typeof(bool), typeof(ThreedChart), new PropertyMetadata(false));


        /// <summary>
        /// 显示Y轴坐标刻度
        /// </summary>
        public bool DisplayYAxisLabel
        {
            get { return (bool)GetValue(DisplayYAxisLabelProperty); }
            set
            {
                SetValue(DisplayXAxisLabelProperty, value);
                //OnPropertyChanged("DisplayYAxisLabel");
                if (axeLayout != null)
                {
                    axeLayout.YAxeLabelDisplayed = value;
                }
            }
        }
        /// <summary>
        /// 显示Y轴坐标刻度
        /// </summary>
        public static readonly DependencyProperty DisplayYAxisLabelProperty =
            DependencyProperty.Register("DisplayYAxisLabel", typeof(bool), typeof(ThreedChart), new PropertyMetadata(false));

        /// <summary>
        /// 显示Z轴坐标刻度
        /// </summary>
        public bool DisplayZAxisLabel
        {
            get { return (bool)GetValue(DisplayZAxisLabelProperty); }
            set
            {
                SetValue(DisplayZAxisLabelProperty, value);
                //OnPropertyChanged("DisplayZAxisLabel");
                if (axeLayout != null)
                {
                    axeLayout.ZAxeLabelDisplayed = value;
                }
            }
        }
        /// <summary>
        /// 显示Z轴坐标刻度
        /// </summary>
        public static readonly DependencyProperty DisplayZAxisLabelProperty =
            DependencyProperty.Register("DisplayZAxisLabel", typeof(bool), typeof(ThreedChart), new PropertyMetadata(false));

        #endregion ChartDisplayProperties

        private List<ShapeInfo> allDisplayShapes = new List<ShapeInfo>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public ThreedChart()
        {
            InitializeComponent();
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // MVVM made simple : DataContext is current object
            this.DataContext = this;

            // Create the interop host control.
            System.Windows.Forms.Integration.WindowsFormsHost fromHost =
                new System.Windows.Forms.Integration.WindowsFormsHost();

            //// Create the Renderer 3D control.
            Renderer3D renderer = new Renderer3D();

            // Assign the Renderer 3D control as the host control's child.
            fromHost.Child = renderer;
            gridContent.Children.Add(fromHost);

            // Create the chart and embed the surface within
            dataChart = new nzy3D.Chart.Chart(renderer, Quality.Nicest);
            axeLayout = dataChart.AxeLayout;

            // Create a mouse control
            nzy3D.Chart.Controllers.Mouse.Camera.CameraMouseController mouse = new nzy3D.Chart.Controllers.Mouse.Camera.CameraMouseController();
            mouse.addControllerEventListener(renderer);
            dataChart.addController(mouse);

            // This is just to ensure code is reentrant (used when code is not in Form_Load but another reentrant event)
            DisposeBackgroundThread();

            // Create a thread to control the camera based on mouse movements
            threadController = new nzy3D.Chart.Controllers.Thread.Camera.CameraThreadController();
            threadController.addControllerEventListener(renderer);
            mouse.addSlaveThreadController(threadController);
            dataChart.addController(threadController);
            //threadController.Start();

            // Associate the chart with current control
            renderer.setView(dataChart.View);
        }

        private void DisposeBackgroundThread()
        {
            if ((threadController != null))
            {
                threadController.Dispose();
            }
            threadController = null;
        }

        /// <summary>
        /// 从windows的Color变换到nzy3D的color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private Color FromWindowColor(System.Windows.Media.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DisposeBackgroundThread();
            e.Handled = true;
        }

        /// <summary>
        /// 创建形状
        /// </summary>
        /// <param name="data">形状数据</param>
        /// <returns>形状</returns>
        private ShapeInfo CreateShape(PointData data)
        {
            if (data == null)
                return null;

            //ShapeInfo info = new ShapeInfo() { data = data, key = data.key };
            //var datacolor = FromWindowColor(data.color.Color);
            //switch (data.pointSharp)
            //{
            //    case PointSharp.Pyramid:
            //        info.shape = new Shape(SimpleObjectGenerator.BuildPyramid(data.centerX, data.centerY, data.centerZ, data.size, datacolor));
            //        break;
            //    case PointSharp.Cube:
            //        info.shape = new Shape(SimpleObjectGenerator.BuildCubic(data.centerX, data.centerY, data.centerZ, data.size, datacolor));
            //        break;
            //    case PointSharp.Sphere:
            //        info.shape = new Shape(SimpleObjectGenerator.BuildSphere(data.centerX, data.centerY, data.centerZ, data.size, 2, datacolor));
            //        break;
            //    default:
            //        break;
            //}

            //info.shape.FaceDisplayed = true;
            //info.shape.WireframeDisplayed = true;
            //info.shape.WireframeColor = Color.CYAN;
            //info.shape.WireframeColor.mul(new Color(1, 1, 1, 0.5));

            //return info;
            return null;

        }
        
        /// <summary>
        /// 添加一个形状
        /// </summary>
        /// <param name="data"></param>
        private void AddShape(PointData data)
        {
            if (data == null)
                return;

            //删除已经加载的图形
            RemoveShape(data.key);

            var shape = CreateShape(data);
            if (shape == null)
                return;

            allDisplayShapes.Add(shape);
            dataChart.Scene.Graph.Add(shape.shape);
            //dataChart.Scene.Graph
        }


        /// <summary>
        /// 创建3D图形
        /// </summary>
        /// <param name="datas">3D图形的数据</param>
        public void AddShapes(IList<PointData> datas)
        {
            if (datas == null || datas.Count == 0)
                return;

            foreach(var data in datas)
            {
                AddShape(data);
            }
            dataChart.View.Render();
        }

        /// <summary>
        /// 移除一个形状
        /// </summary>
        /// <param name="key"></param>
        public void RemoveShape(Guid key)
        {
            ShapeInfo shape = allDisplayShapes.FirstOrDefault(p => p.key == key);
            if (shape != null && shape.shape != null)
            {
                dataChart.Scene.Graph.Remove(shape.shape);
                allDisplayShapes.Remove(shape);
            }
        }

        /// <summary>
        /// 移除一系列形状
        /// </summary>
        /// <param name="keys"></param>
        public void RemoveShapes(IList<Guid> keys)
        {
            if (keys == null)
                return;

            foreach(var key in keys)
            {
                RemoveShape(key);
            }
        }

        /// <summary>
        /// 修改Shape的颜色
        /// </summary>
        /// <param name="key"></param>
        /// <param name="color"></param>
        public void ChangeShapeColor(Guid key, System.Windows.Media.Color color)
        {
            Shape shape = ShapeInfo.GetShape(key, allDisplayShapes);
            if (shape != null)
                shape.Color = FromWindowColor(color);
        }

        /// <summary>
        /// 清除已有的图形
        /// </summary>
        public void ClearChart()
        {
            allDisplayShapes.Clear();
            dataChart.Scene.Clear();
        }
    }
}
