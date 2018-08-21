using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Ai.Hong.Driver.Controls
{
    /// <summary>
    /// 扫描参数设置面板
    /// </summary>
    public partial class CommonParaPanel : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CommonParaPanel()
        {
            InitializeComponent();
        }

        private List<BasePropertyInfo> properties;

        /// <summary>
        /// 语言
        /// </summary>
        private Common.EnumLanguage language;

        /// <summary>
        /// 初始化扫描属性
        /// </summary>
        /// <param name="properties">属性列表</param>
        /// <param name="language">语言</param>
        public void Init(List<BasePropertyInfo> properties, Common.EnumLanguage language)
        {
            this.properties = properties;
            this.language = language;
            for(int i =0; i<properties.Count; i++)
            {
                AddOneProperty(properties[i], i);
            }
        }

        /// <summary>
        /// 添加一个属性
        /// </summary>
        /// <param name="property"></param>
        /// <param name="index"></param>
        private void AddOneProperty(BasePropertyInfo property, int index)
        {
            int row = index / 2;
            //增加一行
            if (rootGrid.RowDefinitions.Count <= row)
            {
                var gridrow = new RowDefinition();
                gridrow.Height = new GridLength(1, GridUnitType.Auto);
                rootGrid.RowDefinitions.Add(gridrow);
            }

            //每一行两个属性，先确定起始列
            int col = (index % 2) == 0 ? 0 : 3;

            //属性名称
            TextBlock txtctrl = new TextBlock();
            txtctrl.Text = property.PropertyDispalayName(language);
            rootGrid.Children.Add(txtctrl);
            txtctrl.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            txtctrl.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txtctrl.Margin = new Thickness(4);
            Grid.SetColumn(txtctrl, col);
            Grid.SetRow(txtctrl, row);

            //属性输入
            col++;
            System.Windows.Data.Binding bind = new System.Windows.Data.Binding("value");
            if (property.Selections != null && property.Selections.Count > 0)
            {
                ComboBox listctrl = new ComboBox();
                bind.UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
                if(property.Inputable)
                    listctrl.SetBinding(ComboBox.TextProperty, bind);
                else
                    listctrl.SetBinding(ComboBox.SelectedValueProperty, bind);
                listctrl.DataContext = property;
                listctrl.ItemsSource = property.Selections;
                listctrl.DisplayMemberPath = "Value";
                listctrl.SelectedValuePath = "Key";
                listctrl.IsEditable = property.Inputable;
                listctrl.Margin = new Thickness(4);
                rootGrid.Children.Add(listctrl);
                Grid.SetColumn(listctrl, col);
                Grid.SetRow(listctrl, row);
            }
            else
            {
                TextBox inputctrl = new TextBox();
                inputctrl.SetBinding(TextBox.TextProperty, bind);
                inputctrl.Margin = new Thickness(4);
                inputctrl.DataContext = property;
                rootGrid.Children.Add(inputctrl);
                Grid.SetColumn(inputctrl, col);
                Grid.SetRow(inputctrl, row);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            i = i + 2;
        }
    }
}
