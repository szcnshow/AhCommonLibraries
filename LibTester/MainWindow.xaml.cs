using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.IO;
using Ai.Hong.Common;
using System.Windows.Media;

namespace LibTester
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {        
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            fileList.SetGridData(allFiles);
            fileList.SetGraphicChart(graphicCtrl);
            graphicCtrl.SetGraphicOperatePanel(panelOperate);

            string file = @"..\testfile\蓝娇2014-04-08 15_32_42.spc";
            Ai.Hong.FileFormat.FileFormat fmt = new Ai.Hong.FileFormat.FileFormat(file);
            Guid chartID = Guid.NewGuid();
            graphicCtrl.AddChart(fmt.xDatas, fmt.yDatas, System.Windows.Media.Brushes.Red, chartID);
            graphicCtrl.AddPointAnnotation(chartID, Guid.NewGuid(), "5186.33", 5186, 0.633);
            graphicCtrl.AddRectangleAnnotation(chartID, Guid.NewGuid(), "5186.33", 5000, 6000, double.NaN, double.NaN, 5, Brushes.Red, new SolidColorBrush(Color.FromArgb(50, 0, 255, 0)), 3.0);

            //Random rnd = new Random();
            //for (int i = 0; i < 100; i++)
            //{
            //    double[] xDatas = new double[] { rnd.NextDouble()*2000+4000 };
            //    double[] yDatas = new double[] { rnd.NextDouble() };
            //    graphicCtrl.AddScatterChart(Guid.NewGuid(), "test", xDatas, yDatas, Ai.Hong.Charts.EnumMarkerType.Circle, 5, (i % 2) == 0 ? Brushes.Blue : Brushes.Red);
            //}
            //graphicCtrl.AddChart(xDatas.ToArray(), yDatas.ToArray(), System.Windows.Media.Brushes.Red, Guid.NewGuid());

            //graphicCtrl.AddEllipseChart(Guid.NewGuid(), "test", fmt.xDatas[fmt.xDatas.Length/2], fmt.dataInfo.minYValue + (fmt.dataInfo.maxYValue-fmt.dataInfo.minYValue)/2, 100, fmt.dataInfo.maxYValue - fmt.dataInfo.minYValue, 0, 0.1, Brushes.Red, new SolidColorBrush(Color.FromArgb(50, 0, 255, 0)));
            //graphicCtrl.AddEllipseChart(Guid.NewGuid(), "test", 0, 0, 10, 100, -Math.PI / 4, 0.1, Brushes.Red, new SolidColorBrush(Color.FromArgb(50, 0, 255, 0)));


            graphicCtrl.Refresh();
        }

        System.Collections.ObjectModel.ObservableCollection<Ai.Hong.Controls.Common.ColorChartDisplayInfo> allFiles = new System.Collections.ObjectModel.ObservableCollection<Ai.Hong.Controls.Common.ColorChartDisplayInfo>();
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
