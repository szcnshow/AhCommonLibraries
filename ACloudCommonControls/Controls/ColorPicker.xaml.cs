using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ai.Hong.Controls.Common;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// 颜色选择弹出菜单
    /// </summary>
    public partial class ColorPicker : System.Windows.Controls.Primitives.Popup
    {
        /// <summary>
        /// 选中的颜色
        /// </summary>
        public Brush selectedBursh = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ColorPicker()
        {
            InitializeComponent();
        }

        private void InitControl()
        {
            for (int i = 0; i < CommonMethod.preDefineColors.Length; i++)
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(1.0);
                border.BorderBrush = Brushes.Black;
                border.Background = CommonMethod.preDefineColors[i];
                border.ToolTip = CommonMethod.preDefineColors[i].ToString();
                
                border.Style = this.Resources["rectStyle"] as Style;
                Grid.SetColumn(border, i / 7);
                Grid.SetColumn(border, i % 5);

                border.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(border_MouseLeftButtonDown);
                rootGrid.Children.Add(border);
            }
        }

        void border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            selectedBursh = border.Background;

            this.IsOpen = false;
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            InitControl();
            this.StaysOpen = false;
        }
    }
}
