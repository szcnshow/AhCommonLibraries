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
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace LibTester
{
    /// <summary>
    /// TestControl.xaml 的交互逻辑
    /// </summary>
    public partial class TestControl : Window
    {
        public TestControl()
        {
            InitializeComponent();
            this.Loaded += TestControl_Loaded;
        }

        private void TestControl_Loaded(object sender, RoutedEventArgs e)
        {
            string file = @"F:\大数据平台\数据\OTO测试\1.4-2.txt";
            txtBowser.SetTextFile(file);
        }
    }
}
