using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace Ai.Hong.Charts
{
    /// <summary>
    /// 通用的图表菜单函数
    /// </summary>
    public class CommonMenuFunction
    {
        /// <summary>
        /// 通过名称查找ItemsControl包含的子节点
        /// </summary>
        /// <param name="currentNode">当前结点</param>
        /// <param name="name">查找的名称</param>
        /// <returns>找到的节点</returns>
        private static ItemsControl FindChildByName(ItemsControl currentNode, string name)
        {
            if (currentNode == null || string.IsNullOrEmpty(name))
                return null;

            if (currentNode.Name == name)
                return currentNode;

            if (currentNode.Items == null || currentNode.Items.Count == 0)
                return null;

            ItemsControl foundItem = null;
            foreach (var item in currentNode.Items)
            {
                foundItem = FindChildByName(item as ItemsControl, name);
                if (foundItem != null)
                    return foundItem;
            }

            return foundItem;
        }

        /// <summary>
        /// 绑定弹出菜单和操作工具条
        /// </summary>
        /// <param name="contextMenu">弹出菜单</param>
        /// <param name="panel">图形操作按钮面板</param>
        public static Dictionary<ChartMenuItems, System.Windows.Controls.Primitives.ButtonBase> BindMenuWithOperatePanel(ContextMenu contextMenu, GraphicOperatePanel panel)
        {
            if (panel == null)
                return new Dictionary<ChartMenuItems, System.Windows.Controls.Primitives.ButtonBase>();

            //创建操作按钮和菜单的对应关系
            var operateButtons = new Dictionary<ChartMenuItems, System.Windows.Controls.Primitives.ButtonBase>();
            var allbuttons = panel.GetAllButtons();
            foreach (var btn in allbuttons)
            {
                ChartMenuItems menuItem = ChartMenuItems.Display;
                if (btn == panel.btnSelect)
                    menuItem = ChartMenuItems.Select;
                else if (btn == panel.btnMove)
                    menuItem = ChartMenuItems.Pan;
                else if (btn == panel.btnZoomIn)
                    menuItem = ChartMenuItems.zoomIn;
                else if (btn == panel.btnZoomOut)
                    menuItem = ChartMenuItems.zoomOut;
                else if (btn == panel.btnInformation)
                    menuItem = ChartMenuItems.showInformation;
                else if (btn == panel.btnUpPeakPick)
                    menuItem = ChartMenuItems.upPeakPick;
                else if (btn == panel.btnDownPeakPick)
                    menuItem = ChartMenuItems.downPeakPick;
                else if (btn == panel.btnSizeAll)
                    menuItem = ChartMenuItems.resetXY;
                else if (btn == panel.btnSizeYAxis)
                    menuItem = ChartMenuItems.resetY;
                else if (btn == panel.btnColor)
                    menuItem = ChartMenuItems.Colors;
                else if (btn == panel.btnHide)
                    menuItem = ChartMenuItems.Hide;
                else if (btn == panel.btnGridShow)
                    menuItem = ChartMenuItems.showGridLine;
                else
                    continue;

                operateButtons.Add(menuItem, btn);
            }

            //按钮和菜单间的属性绑定
            foreach (var item in operateButtons)
            {
                MenuItem menu = FindChildByName(contextMenu, item.Key.ToString()) as MenuItem;
                if (menu != null)
                {
                    //刷新按钮属性
                    item.Value.IsEnabled = menu.IsEnabled;
                    item.Value.ToolTip = menu.Header;
                    item.Value.Visibility = menu.Visibility;
                    item.Value.ToolTip = menu.Header;

                    if (item.Value is RadioButton)
                    {
                        (item.Value as RadioButton).IsChecked = menu.IsChecked;

                        Binding checkbind = new Binding("IsChecked");
                        checkbind.Mode = BindingMode.TwoWay;
                        checkbind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        menu.IsCheckable = true;
                        menu.SetBinding(MenuItem.IsCheckedProperty, checkbind);
                    }

                    Binding bind = new Binding("IsEnabled");
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    menu.SetBinding(MenuItem.IsEnabledProperty, bind);

                    bind = new Binding("Visibility");
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    menu.SetBinding(MenuItem.VisibilityProperty, bind);

                    bind = new Binding("ToolTip");
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    menu.SetBinding(MenuItem.ToolTipProperty, bind);

                    menu.DataContext = item.Value;
                }
            }

            return operateButtons;
        }

        /// <summary>
        /// 在父菜单下添加一个菜单项
        /// </summary>
        /// <typeparam name="T">ContextMenu或者MenuItem</typeparam>
        /// <param name="commandItem">菜单项</param>
        /// <param name="parentMenu">父菜单</param>
        /// <param name="action">菜单消息处理过程</param>
        /// <param name="language">使用的语言</param>
        /// <returns></returns>
        public static MenuItem AddOneItem<T>(ChartMenuItems commandItem, T parentMenu, System.Windows.RoutedEventHandler action, Common.EnumLanguage language) where T : class
        {
            MenuItem curmenu = new MenuItem();
            curmenu.Name = commandItem.ToString();
            curmenu.Header = Common.Extenstion.EnumExtensions.GetEnumDescription(commandItem, language);
            curmenu.Click += action;

            if (parentMenu is ContextMenu)
                (parentMenu as ContextMenu).Items.Add(curmenu);
            else if (parentMenu is MenuItem)
                (parentMenu as MenuItem).Items.Add(curmenu);

            return curmenu;
        }

        /// <summary>
        /// 初始化图形上的所有系统菜单
        /// </summary>
        /// <param name="contextMenu">弹出菜单或者菜单项</param>
        /// <param name="action">菜单消息处理过程</param>
        /// <param name="language">使用的语言</param>
        /// <param name="colorPanelTemplate">颜色菜单项template</param>
        /// <param name="colorControlTemplate">颜色菜单显示Template</param>
        public static void InitChartPopupMenu<T>(T contextMenu, System.Windows.RoutedEventHandler action, Common.EnumLanguage language, ItemsPanelTemplate colorPanelTemplate, ControlTemplate colorControlTemplate) where T : class
        {
            //选择移动菜单
            MenuItem mainItem = AddOneItem(ChartMenuItems.Select, contextMenu, action, language);
            mainItem = AddOneItem(ChartMenuItems.Pan, contextMenu, action, language);

            //缩放菜单（放大，缩小，重置XY，重置Y）
            mainItem = AddOneItem(ChartMenuItems.Zoom, contextMenu, action, language);
            MenuItem subItem = AddOneItem(ChartMenuItems.zoomIn, mainItem, action, language);
            subItem = AddOneItem(ChartMenuItems.zoomOut, mainItem, action, language);
            subItem = AddOneItem(ChartMenuItems.resetXY, mainItem, action, language);
            subItem = AddOneItem(ChartMenuItems.resetY, mainItem, action, language);

            //显示菜单（颜色，信息，隐藏，网格）
            mainItem = AddOneItem(ChartMenuItems.Display, contextMenu, action, language);
            subItem = AddOneItem(ChartMenuItems.Colors, mainItem, action, language);
            //颜色列表菜单
            subItem.ItemsPanel = colorPanelTemplate;
            foreach (var color in ChartCommonMethod.strColors)
            {
                AddSubColorMenu(subItem, color, action, colorControlTemplate);
            }
            subItem = AddOneItem(ChartMenuItems.showInformation, mainItem, action, language);
            subItem = AddOneItem(ChartMenuItems.Hide, mainItem, action, language);
            subItem = AddOneItem(ChartMenuItems.showGridLine, mainItem, action, language);

            //标注峰位主菜单（上峰位，下峰位）
            mainItem = AddOneItem(ChartMenuItems.peakPick, contextMenu, action, language);
            subItem = AddOneItem(ChartMenuItems.upPeakPick, mainItem, action, language);
            subItem = AddOneItem(ChartMenuItems.downPeakPick, mainItem, action, language);
        }

        /// <summary>
        /// 添加颜色菜单
        /// </summary>
        /// <param name="parentMenu">父菜单</param>
        /// <param name="color">演示名称</param>
        /// <param name="action">消息处理过程</param>
        /// <param name="colorControlTemplate">菜单控件Template</param>
        private static void AddSubColorMenu(MenuItem parentMenu, string color, RoutedEventHandler action, ControlTemplate colorControlTemplate)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1);
            if (color == "More")    //高级菜单
            {
                TextBlock txt = new TextBlock();
                txt.Text = "...";
                txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                border.Child = txt;
                border.ToolTip = "More...";
            }
            else
            {
                SolidColorBrush curColor = (SolidColorBrush)typeof(Brushes).GetProperty(color).GetValue(null, null);
                border.Background = curColor;
            }
            border.Width = 16;
            border.Height = 16;
            border.Margin = new Thickness(2);
            MenuItem menu = new MenuItem();
            menu.Template = colorControlTemplate;
            menu.Header = border;
            menu.Name = "Color_" + color;
            menu.Click += action;

            parentMenu.Items.Add(menu);

        }

        /// <summary>
        /// 查找菜单列表
        /// </summary>
        /// <param name="contextMenu">弹出菜单</param>
        /// <param name="menuCommands">菜单项目</param>
        /// <returns></returns>
        private static List<MenuItem> GetMenuItems(ContextMenu contextMenu, List<ChartMenuItems> menuCommands)
        {
            List<MenuItem> retDatas = new List<MenuItem>();

            foreach (var item in menuCommands)
            {
                MenuItem menu = FindChildByName(contextMenu, item.ToString()) as MenuItem;
                if (menu != null)
                    retDatas.Add(menu);
            }

            return retDatas;
        }

        /// <summary>
        /// 显示或者隐藏菜单列表
        /// </summary>
        /// <param name="contextMenu"></param>
        /// <param name="menuCommands"></param>
        /// <param name="visible"></param>
        public static void VisibleMenuCommands(ContextMenu contextMenu, List<ChartMenuItems> menuCommands, bool visible)
        {
            var menuItems = GetMenuItems(contextMenu, menuCommands);
            foreach (var item in menuItems)
            {
                item.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 激活或者失效菜单列表
        /// </summary>
        /// <param name="contextMenu"></param>
        /// <param name="menuCommands"></param>
        /// <param name="visible"></param>
        public static void EnableMenuCommands(ContextMenu contextMenu, List<ChartMenuItems> menuCommands, bool visible)
        {
            var menuItems = GetMenuItems(contextMenu, menuCommands);
            foreach (var item in menuItems)
            {
                item.IsEnabled = visible;
            }
        }
    }
}
