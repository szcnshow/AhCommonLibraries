using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// ComboxWithDeleteButton.xaml 的交互逻辑
    /// </summary>
    public partial class ComboxWithDeleteButton : ComboBox
    {
        /// <summary>
        /// 列表删除消息
        /// </summary>
        public static readonly RoutedEvent ItemDeleteEvent = EventManager.RegisterRoutedEvent("ItemDelete",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ComboxWithDeleteButton));
        /// <summary>
        /// 列表删除消息
        /// </summary>
        public event RoutedEventHandler ItemDelete
        {
            add { AddHandler(ItemDeleteEvent, value); }
            remove { RemoveHandler(ItemDeleteEvent, value); }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ComboxWithDeleteButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 发送列表操作消息(包含当前操作的项)
        /// </summary>
        /// <param name="node">操作方法</param>
        private void RaiseNodeEvent(object node)
        {
            SelectionChangedEventArgs arg = new SelectionChangedEventArgs(ItemDeleteEvent, new List<object>(){node}, new List<string>());
            arg.Source = this;
            RaiseEvent(arg);
        }

        private void btnDelete_Clicked(object sender, RoutedEventArgs e)
        {
            var btn = sender as Ai.Hong.Controls.VectorImageButton;

            if (btn.Tag != null)
            {
                var templist = ItemsSource as List<string>;
                ItemsSource = null;
                templist.Remove(btn.Tag as string);
                ItemsSource = templist;
                Items.Refresh();
            }
        }
    }
}
