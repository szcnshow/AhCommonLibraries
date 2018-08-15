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

            Ai.Hong.Driver.ScanParameter parameter = new Ai.Hong.Driver.ScanParameter();
            parameter.SetAddtionalData("test1", "1");
            parameter.SetAddtionalData("test2", "2");

            Ai.Hong.Driver.SerializableDictionary<string, string> test = new Ai.Hong.Driver.SerializableDictionary<string, string>() { { "1", "1" }, { "2", "2" } };
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(test.GetType());
            StringWriter textWriter = new StringWriter();
            xs.Serialize(textWriter, test);
            var temp = textWriter.ToString();

            StringReader textReader = new StringReader(temp);

            xs = new System.Xml.Serialization.XmlSerializer(test.GetType());
            var retData = xs.Deserialize(textReader);

        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            fileList.SetGridData(allFiles);
            fileList.SetGraphicChart(graphicCtrl);
            graphicCtrl.SetGraphicOperatePanel(panelOperate);
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
