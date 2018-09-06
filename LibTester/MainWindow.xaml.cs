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
            
            Ai.Hong.FileFormat.FileFormat fmt = new Ai.Hong.FileFormat.FileFormat(@"D:\艾宏文件\软件开发\EVISTAR\天大傅里叶光谱仪\LongLight_New\tempData\IntegrateError.SPC");
            var temp = Ai.Hong.Algorithm.CommonAlgorithm.Integrate(fmt.xDatas, fmt.yDatas, 4000, 9999.8374771240724, true, 0);
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
