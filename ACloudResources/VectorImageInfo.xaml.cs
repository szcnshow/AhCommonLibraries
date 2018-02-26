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

namespace Ai.Hong.CommonResources
{
    /// <summary>
    /// VectorImageInfo.xaml 的交互逻辑
    /// </summary>
    public partial class VectorImageInfo : UserControl
    {
        /// <summary>
        /// 矢量图形（DrawingGroup）
        /// </summary>
        public DrawingGroup VectorSource
        {
            get { return (DrawingGroup)GetValue(VectorSourceProperty); }
            set { SetValue(VectorSourceProperty, value); }
        }

        /// <summary>
        /// 矢量图形（DrawingGroup）
        /// </summary>
        public static readonly DependencyProperty VectorSourceProperty =
            DependencyProperty.Register("VectorSource", typeof(DrawingGroup), typeof(VectorImageInfo), new UIPropertyMetadata(null));


        /// <summary>
        /// 矢量图形名称
        /// </summary>
        public string VectorText
        {
            get { return (string)GetValue(VectorTextProperty); }
            set { SetValue(VectorTextProperty, value); }
        }

        /// <summary>
        /// 矢量图形名称
        /// </summary>
        public static readonly DependencyProperty VectorTextProperty =
            DependencyProperty.Register("VectorText", typeof(string), typeof(VectorImageInfo), new PropertyMetadata(null));

        /// <summary>
        /// 构造函数
        /// </summary>
        public VectorImageInfo()
        {
            InitializeComponent();
        }
    }
}
