using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;
using System.Data;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Xps;
using System.Windows.Documents;
using System.Windows.Xps.Serialization;
using System.IO;
using System.Reflection;

namespace Ai.Hong.Controls.Common
{
    /// <summary>
    /// 通用功能
    /// </summary>
    public class CommonMethod
    {
        /// <summary>
        /// 预定义的颜色（More为自定义)
        /// </summary>
        public static SolidColorBrush[] preDefineColors =
            {
                Brushes.Black, Brushes.Red, Brushes.Blue, Brushes.Gold, Brushes.Brown, Brushes.DarkGreen, Brushes.Navy, Brushes.LawnGreen, 
                Brushes.DimGray, Brushes.DeepPink, Brushes.OrangeRed, Brushes.Olive,  Brushes.Teal, Brushes.SlateGray, Brushes.Gray, Brushes.MidnightBlue, 
                Brushes.Orange, Brushes.YellowGreen, Brushes.SeaGreen, Brushes.Aqua, Brushes.LightBlue, Brushes.Violet, Brushes.DarkGray,
                Brushes.Pink, Brushes.Yellow, Brushes.Lime, Brushes.Turquoise, Brushes.SkyBlue, Brushes.Plum, Brushes.LightGray, Brushes.Indigo,
                Brushes.LightPink, Brushes.Tan, Brushes.LightCoral, Brushes.WhiteSmoke
            };

        /// <summary>
        /// 移除DataGrid中的选定项
        /// </summary>
        /// <param name="allItems"></param>
        /// <param name="removeItems"></param>
        public static void RemoveDataGridItems(System.Collections.IList allItems, System.Collections.IList removeItems)
        {
            List<object> tempList = new List<object>();
            foreach (object item in removeItems)
                tempList.Add(item);

            foreach (object item in tempList)
            {
                if (allItems.IndexOf(item) >= 0)
                    allItems.Remove(item);
            }
        }

        /// <summary>
        /// 获取Datagrid中的内容
        /// </summary>
        /// <param name="dataGrid">DataGrid</param>
        /// <returns>以行为主的内容</returns>
        public static List<List<string>> GetDataGridContent(DataGrid dataGrid)
        {
            List<List<string>> allContents = new List<List<string>>();
            List<string> contents = new List<string>();

            //表头，不导出隐藏的列
            foreach (var datacol in dataGrid.Columns)
            {
                if (datacol.Visibility== Visibility.Visible && datacol.Header is string)
                    contents.Add(datacol.Header.ToString());
            }
            allContents.Add(contents);

            //内容
            for (int rowindex = 0; rowindex < dataGrid.Items.Count; rowindex++)
            {
                contents = new List<string>();
                for (int colindex = 0; colindex < dataGrid.Columns.Count; colindex++)
                {
                    if (dataGrid.Columns[colindex].Visibility != Visibility.Visible)
                        continue;

                    var cell = dataGrid.GetCell(rowindex, colindex);
                    if (cell != null && cell.Content != null)
                    {
                        if (dataGrid.Columns[colindex] is DataGridTextColumn)     //导出文本数据
                        {
                            TextBlock ctrl = cell.Content as TextBlock;
                            if (ctrl != null)
                                contents.Add(ctrl.Text);
                        }
                        else if (dataGrid.Columns[colindex] is DataGridCheckBoxColumn)  //CheckBox
                        {
                            CheckBox ctrl = cell.Content as CheckBox;
                            if (ctrl != null)
                                contents.Add(ctrl.IsChecked == true ? "True" : "False");
                        }
                        else if (dataGrid.Columns[colindex] is DataGridComboBoxColumn)  //ComboBox
                        {
                            ComboBox ctrl = cell.Content as ComboBox;
                            if (ctrl != null)
                                contents.Add(ctrl.Text);
                        }
                        else if (dataGrid.Columns[colindex] is DataGridTemplateColumn)
                        {
                            var ctrls = GetLogicalChildCollection<TextBlock>(cell.Content);
                            if (ctrls != null && ctrls.Count > 0)
                                contents.Add(ctrls[0].Text);
                        }
                    }
                }
                allContents.Add(contents);
            }

            return allContents;
        }

        /// <summary>
        /// 获取控件中的所有子控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
        {
            List<T> logicalCollection = new List<T>();
            GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
            return logicalCollection;
        }

        /// <summary>
        /// 获取下属指定类型的控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="parent">父控件</param>
        /// <param name="logicalCollection">子控件</param>
        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            System.Collections.IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }

        /// <summary>
        /// 组合字符串列表
        /// </summary>
        /// <param name="contents">字符串列表</param>
        /// <param name="spliter">分割符</param>
        /// <returns>组合后的字符串列表</returns>
        public static string CombineStringContents(IEnumerable<string> contents, string spliter=",")
        {
            if (contents == null || contents.Count() == 0)
                return null;

            string retStr = null;
            foreach (var item in contents)
                retStr += item + spliter;

            if(retStr != null)
                retStr = retStr.Remove(retStr.Length - 1);

            return retStr;
        }
    }

    /// <summary>
    /// WPF DataGrid控件扩展方法 
    /// </summary>
    public static class DataGridPlus
    {
        /// <summary>
        /// 获取DataGrid控件单元格
        /// </summary>
        /// <param name="dataGrid">DataGrid控件</param>
        /// <param name="rowIndex">单元格所在的行号</param>
        /// <param name="columnIndex">单元格所在的列号</param>
        /// <returns>指定的单元格</returns>
        public static DataGridCell GetCell(this DataGrid dataGrid, int rowIndex, int columnIndex)
        {
            DataGridRow rowContainer = dataGrid.GetRow(rowIndex);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[columnIndex]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter == null)
                    return null;

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                if (cell == null)
                {
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[columnIndex]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                }
                return cell;
            }
            return null;
        }

        /// <summary>
        /// 获取DataGrid的行
        /// </summary>
        /// <param name="dataGrid">DataGrid控件</param>
        /// <param name="rowIndex">DataGrid行号</param>
        /// <returns>指定的行号</returns>
        public static DataGridRow GetRow(this DataGrid dataGrid, int rowIndex)
        {
            DataGridRow rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (rowContainer == null)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            }
            return rowContainer;
        }

        /// <summary>
        /// 获取父可视对象中第一个指定类型的子可视对象
        /// </summary>
        /// <typeparam name="T">可视对象类型</typeparam>
        /// <param name="parent">父可视对象</param>
        /// <returns>第一个指定类型的子可视对象</returns>
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }

    /// <summary>
    /// 带颜色选择的DataGrid列表框的基本类
    /// </summary>
    public class ColorChartDisplayInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorMessage = null;

        /// <summary>
        /// 获取显示颜色
        /// </summary>
        /// <param name="index">颜色列表中的序号</param>
        public static SolidColorBrush GetDisplayColor(int index)
        {
            return CommonMethod.preDefineColors[index % CommonMethod.preDefineColors.Length];
        }

        /// <summary>
        /// 属性变化消息
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性变化消息
        /// </summary>
        public void DoProperChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isChecked { get; set; }
        /// <summary>
        /// 是否显示光谱图
        /// </summary> 
        public bool isChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                DoProperChange("isChecked");
            }
        }

        private string _name { get; set; }
        /// <summary>
        /// 数据显示名称
        /// </summary> 
        public string name { get { return _name; } set { _name = value; DoProperChange("name"); } }

        private bool _isSelected;
        /// <summary>
        /// 是否在列表中选中
        /// </summary>
        public bool isSelected { get { return _isSelected; } set { _isSelected = value; DoProperChange("isSelected"); } }

        private SolidColorBrush _rowColor = Brushes.Black;
        /// <summary>
        /// 数据行显示颜色
        /// </summary>
        public SolidColorBrush rowColor { get { return _rowColor; } set { _rowColor = value; DoProperChange("rowColor"); } }

        private SolidColorBrush _color;
        /// <summary>
        /// 显示颜色
        /// </summary>
        public SolidColorBrush color { get { return _color; } set { _color = value; DoProperChange("color"); } }

        /// <summary>
        /// 当前数据，用于绑定
        /// </summary>
        public ColorChartDisplayInfo thisItem { get { return this; } }

        /// <summary>
        /// X数据
        /// </summary>
        public double[] xDatas { get; set; }

        /// <summary>
        /// Y数据
        /// </summary>
        public double[] yDatas { get; set; }

        /// <summary>
        /// 当前数据的KEY
        /// </summary>
        public Guid key { get; set; }

        /// <summary>
        /// 附加的变量,通常用于查找
        /// </summary>
        public object tag { get; set; }

        /// <summary>
        /// 数据显示格式（小数位数）
        /// </summary>
        public string dataDisplayFormat = "F2";

        /// <summary>
        /// 附加信息名称
        /// </summary>
        public string[] addInfoTitles { get; set; }

        /// <summary>
        /// 附加信息内容
        /// </summary>
        public string[] addInfoValues { get; set; }

        private void innerInit(string name, double[] xDatas, double[] yDatas, Brush color, string dataDisplayFormat)
        {
            this.isChecked = false;
            this.color = (SolidColorBrush)color;
            this.key = Guid.NewGuid();
            this.xDatas = xDatas;
            this.yDatas = yDatas;
            this.name = name;
            this.tag = null;
            this.dataDisplayFormat = dataDisplayFormat;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ColorChartDisplayInfo()
        {
            innerInit(null, null, null, Brushes.Black, "F2");
        }

        /// <summary>
        /// 初始化图形数据
        /// </summary>
        /// <param name="name">数据显示名称</param>
        /// <param name="xDatas">X轴值</param>
        /// <param name="yDatas">Y轴值</param>
        /// <param name="color">数据颜色</param>
        /// <param name="dataDisplayFormat">小数的显示格式</param>
        public ColorChartDisplayInfo(string name, double[] xDatas, double[] yDatas, Brush color, string dataDisplayFormat = "F2")
        {
            innerInit(name, xDatas, yDatas, color, dataDisplayFormat);
        }

    }

    /// <summary>
    /// Checked与Visibility之间的转换, True=Visible, False=Collapsed
    /// </summary>
    [ValueConversion(typeof(Visibility), typeof(bool))]
    public class TrueIsVisibilityConvert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value == true) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }


    /// <summary>
    /// False与Visibility之间的转换, False=Visible, True=Collapsed
    /// </summary>
    [ValueConversion(typeof(Visibility), typeof(bool))]
    public class FalseIsVisibilityConvert : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value == true) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value != Visibility.Visible;
        }
    }

    /// <summary>
    /// 长时间操作函数运行参数
    /// </summary>
    public class WaitProcessParameter : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性变化消息
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性变化消息
        /// </summary>
        public void DoProperChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _message = null;
        /// <summary>
        /// 显示信息
        /// </summary>
        public string message { get { return _message; } set { _message = value; DoProperChange("message"); } }

        private double _maximum = 1.0;
        /// <summary>
        /// 滚动条最大值
        /// </summary>
        public double maximum { get { return _maximum; } set { _maximum = value; DoProperChange("maximum"); } }

        private double _curValue = 1.0;
        /// <summary>
        /// 滚动条当前值
        /// </summary>
        public double curValue { get { return _curValue; } set { _curValue = value; DoProperChange("curValue"); } }

        /// <summary>
        /// 是否为用户取消
        /// </summary>
        public bool cancel { get; set; }

        /// <summary>
        /// 是否操作成功
        /// </summary>
        public bool sucessed { get; set; }

        /// <summary>
        /// 线程运行参数
        /// </summary>
        public object threadParameter { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }


    /// <summary>
    /// XPS报表模板
    /// </summary>
    public static class XPSReportTemplate
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = null;

        private const double DPCM = 96 / 2.54;      //1cm中的像素点数量

        /// <summary>
        /// 保存到XPS格式文档
        /// </summary>
        /// <param name="xpsFilename">XPS文件名</param>
        /// <param name="fixedDoc">XPS文档</param>
        public static bool SaveToXpsFile(string xpsFilename, FixedDocument fixedDoc)
        {
            try
            {
                //保存到XPS文件
                DocumentPaginator paginator = fixedDoc.DocumentPaginator;
                System.Windows.Xps.Packaging.XpsDocument xpsDocument = new System.Windows.Xps.Packaging.XpsDocument(xpsFilename, System.IO.FileAccess.Write);
                var documentWriter = System.Windows.Xps.Packaging.XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                documentWriter.Write(paginator);
                xpsDocument.Close();

                return true;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 通过写入和读出Xaml方式克隆一个Object
        /// </summary>
        public static T CloneObject<T>(T obj)
        {
            string gridXaml = System.Windows.Markup.XamlWriter.Save(obj);
            MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(gridXaml));
            Object clone = System.Windows.Markup.XamlReader.Load(stream);
            return (T)clone;
        }

        /// <summary>
        /// 填写TextBlock控件内容
        /// </summary>
        /// <param name="rootEl">root控件</param>
        /// <param name="IdName">TextBlock控件名称</param>
        /// <param name="content">填写内容</param>
        public static void FillTextData(FrameworkElement rootEl, string IdName, string content)
        {
            object el = rootEl.FindName(IdName);
            if (el == null)
                return;

            if (el is TextBlock)
            {
                (el as TextBlock).Text = content;
            }
        }

        /// <summary>
        /// 加载打印模版
        /// </summary>
        /// <param name="templateName">模版文件名</param>
        /// <param name="assemb">当前调用进程的Assembly</param>
        /// <returns>加载后的FlowDocument</returns>
        public static FlowDocument LoadDocumentTemplate(Assembly assemb, string templateName)
        {
            return Ai.Hong.Common.ResourceOperator.EmbededResourceElement(assemb, templateName) as FlowDocument;
        }

        /// <summary>
        /// 获取FlowDocument包含的最上层的BlockUIContainer
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static BlockUIContainer GetBlcokUIContainer(FlowDocument document)
        {
            if (document == null)
                return null;

            var enumertor = document.Blocks.GetEnumerator();
            while(enumertor.MoveNext())
            {
                if (enumertor.Current is BlockUIContainer)
                    return enumertor.Current as BlockUIContainer;
            }

            return null;
        }

        /// <summary>
        /// 获取BlockUIContainer包含的Element
        /// </summary>
        /// <param name="parentControl"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetElement<T>(FrameworkElement parentControl, string name)
        {
            if (parentControl == null)
                return default(T);

            var el = (T)parentControl.FindName(name);

            return el;
        }


        /// <summary>
        /// 创建一页报告
        /// </summary>
        /// <param name="rootBorder"></param>
        /// <returns></returns>
        public static PageContent CreatePageContent(Border rootBorder)
        {
            PageContent pageContent = new PageContent();
            FixedPage page = new FixedPage();
            page.Width = 21 * DPCM;        //A4 Paper: 21cm x 29.7cm
            page.Height = 29.7 * DPCM;

            //page.ContentBox = new Rect(2 * DPCM, 2 * DPCM, (21 - 4) * DPCM, (29.7 - 4) * DPCM);
            page.Children.Add(rootBorder);
            ((System.Windows.Markup.IAddChild)pageContent).AddChild(page);
            return pageContent;
        }

        /// <summary>
        /// 显示光谱图形
        /// </summary>
        /// <param name="rootBorder">控件树的根</param>
        /// <param name="graphicBorderName">图像控件名</param>
        /// <param name="graphicFile">光谱文件</param>
        /// <param name="graphicWidth">图像的宽度</param>
        /// <param name="graphicHeight">图像的高度</param>
        /// <param name="DPI">图像分辨率</param>
        public static void ShowSpectrumGraphic(Border rootBorder, string graphicBorderName, string graphicFile, double graphicWidth, double graphicHeight = double.MaxValue, double DPI = double.MaxValue)
        {
            Border graphicBorder = rootBorder.FindName(graphicBorderName) as Border;
            if (graphicBorder != null)
            {
                //System.Windows.Forms.DataVisualization.Charting.Chart graphicChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
                //System.Windows.Forms.DataVisualization.Charting.ChartArea ca = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
                //graphicChart.ChartAreas.Add(ca);
                //CommonMethod.DrawSpectrumGraphic(graphicChart, ca, graphicFile, System.Drawing.Color.Black);

                //DPI = (DPI == double.MaxValue) ? 96 : DPI;
                //graphicHeight = (graphicHeight == double.MaxValue) ? graphicBorder.Height * DPI / 96 : graphicHeight * DPI / 2.54;

                //graphicChart.Width = (int)(graphicWidth * DPI / 2.54);        //1cm = 2.54inch = 96dpi
                //graphicChart.Height = (int)(graphicHeight);

                //System.IO.MemoryStream stream = new MemoryStream();
                //graphicChart.SaveImage(stream, System.Drawing.Imaging.ImageFormat.Png);

                //var bitmapImage = new BitmapImage();
                //bitmapImage.BeginInit();
                //bitmapImage.StreamSource = stream;
                //bitmapImage.EndInit();

                //Image img = new Image();
                //graphicBorder.Child = img;
                //img.Source = bitmapImage;
                //img.Stretch = System.Windows.Media.Stretch.Uniform;
            }
        }
    }

}
