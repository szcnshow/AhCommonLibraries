using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;
using Ai.Hong.Common;

// Instrument test
namespace Ai.Hong.Driver.IT
{

    /// <summary>
    /// 创建XPS报告
    /// </summary>
    public class XPSReport
    {
        private Border totalBorder = null;
        private Border detailBorder = null;

        private static EnumLanguage language = EnumLanguage.Chinese;

        const double DPCM = 96 / 2.54;      //1cm中的像素点数量
        const double headerFontSize = 20;
        const double textFontSize = 13;
        private static System.Windows.Media.SolidColorBrush headerBackground = System.Windows.Media.Brushes.LightSteelBlue;
        private static System.Windows.Media.SolidColorBrush headerForeground = System.Windows.Media.Brushes.Black;
        private static System.Windows.Media.SolidColorBrush textBackground = System.Windows.Media.Brushes.White;
        private static System.Windows.Media.SolidColorBrush textForeground = System.Windows.Media.Brushes.Black;

        private FTDriver scanner = null;

        private string UserName;
        private string UnitName;
        private EnumLanguage Language = EnumLanguage.Chinese;
        /// <summary>
        /// AHCommonResources 资源目录
        /// </summary>
        private ResourceDictionary resourceDir;

        /// <summary>
        /// 构造函数
        /// </summary>
        public XPSReport()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="scanner">测试仪器</param>
        /// <param name="totalTemplate">报告总模板</param>
        /// <param name="detailTemplate">详细报告模板</param>
        /// <param name="UserName">测量人员名称</param>
        /// <param name="UnitName">测量单位名称</param>
        /// <param name="resourceDir">结果图形路径</param>
        /// <param name="language">语言</param>
        public XPSReport(FTDriver scanner, string totalTemplate, string detailTemplate, string UserName, string UnitName, 
            ResourceDictionary resourceDir,
            EnumLanguage language= EnumLanguage.Chinese)
        {
            totalBorder = GetRootBorder(totalTemplate);
            detailBorder = GetRootBorder(detailTemplate);
            this.scanner = scanner;
            this.UserName = UserName;
            this.UnitName = UnitName;
            this.Language = language;
            this.resourceDir = resourceDir;
        }       
        
        /// <summary>
        /// 获取模板的rootBorder
        /// </summary>
        /// <param name="templateName">模板名称</param>
        /// <returns></returns>
        private Border GetRootBorder(string templateName)
        {
            var assemb = System.Reflection.Assembly.GetExecutingAssembly();
            var templateDoc = ResourceOperator.EmbededResourceElement(assemb, templateName) as FlowDocument;
            var blockUI = Ai.Hong.Controls.Common.XPSReportTemplate.GetBlcokUIContainer(templateDoc);
            Border rootBorder = blockUI.Child as Border;
            
            return rootBorder;
        }    

        /// <summary>
        /// 添加Textblock到Grid中
        /// </summary>
        /// <param name="paraGrid"></param>
        /// <param name="textstr">内容</param>
        /// <param name="index">参数Index</param>
        /// <param name="isLabel">是否Label</param>
        private void AddTextToGrid(Grid paraGrid, string textstr, int index, bool isLabel)
        {
            int row = index / 2 + 1;
            int col = (index % 2) * 2;
            if (!isLabel)
                col++;

            if (index / 2 + 1 >= paraGrid.RowDefinitions.Count)
            {
                RowDefinition rowctrl = new RowDefinition();
                rowctrl.Height = new GridLength(1, GridUnitType.Auto);
                paraGrid.RowDefinitions.Add(rowctrl);
            }

            TextBlock txtctrl = new TextBlock();
            txtctrl.HorizontalAlignment = isLabel ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            txtctrl.Text = textstr;
            txtctrl.Margin = new Thickness(4);

            Grid.SetRow(txtctrl, row);
            Grid.SetColumn(txtctrl, col);
            paraGrid.Children.Add(txtctrl);
        }

        /// <summary>
        /// 创建检测结果Grid
        /// </summary>
        /// <param name="rowCount">总共多少行</param>
        /// <param name="titleTextBox">返回标题TextBlock</param>
        /// <param name="imageBorder">返回标题图像的Border</param>
        /// <returns></returns>
        private Grid GenerateOneGroupGrid(int rowCount, out TextBlock titleTextBox, out Border imageBorder)
        {
            Grid rootGrid = new Grid();
            //rootGrid.Margin = new System.Windows.Thickness(0, 10, 0, 10);

            //创建列, 0-6列是内容，7，8是结果的图标
            for (int i = 0; i < 9; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto);
                rootGrid.ColumnDefinitions.Add(col);
            }

            //0, 1, 3, 5, 7, 8是Auto，2, 4, 6列是1*
            int[] allwidth = new int[] { 2, 4, 6 };
            foreach (var i in allwidth)
                rootGrid.ColumnDefinitions[i].Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);

            //创建行
            for (int i = 0; i < rowCount; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto);
                rootGrid.RowDefinitions.Add(row);
            }

            //创建Border Title
            Border titleBoarder = new Border();
            titleBoarder.Background = headerBackground;
            titleBoarder.BorderThickness = new System.Windows.Thickness(0);
            Grid.SetRow(titleBoarder, 0);
            Grid.SetColumn(titleBoarder, 0);
            Grid.SetColumnSpan(titleBoarder, rootGrid.ColumnDefinitions.Count);
            rootGrid.Children.Add(titleBoarder);

            //创建Title TextBox
            titleTextBox = new TextBlock();
            titleTextBox.FontSize = headerFontSize;
            titleTextBox.Foreground = headerForeground;
            titleTextBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            titleTextBox.Margin = new System.Windows.Thickness(0, 5, 0, 5);
            titleBoarder.Child = titleTextBox;

            //总结果显示图标(跨2列)
            imageBorder = new Border();
            imageBorder.Background = headerBackground;
            Grid.SetRow(imageBorder, 0);
            Grid.SetColumn(imageBorder, 7);
            Grid.SetColumnSpan(imageBorder, 2);
            rootGrid.Children.Add(imageBorder);

            return rootGrid;
        }

        /// <summary>
        /// 添加一个TextBlock到Grid中
        /// </summary>
        /// <param name="rootGrid">Grid</param>
        /// <param name="textstr">内容</param>
        /// <param name="row">Textblock在Grid中的行</param>
        /// <param name="col">Textblock在Grid中的列</param>
        /// <param name="isLabel">是否为标题Label</param>
        private void AddTextBlockToGrid(Grid rootGrid, string textstr, int row, int col, bool isLabel)
        {
            TextBlock txtctrl = new TextBlock();
            txtctrl.HorizontalAlignment = isLabel ? System.Windows.HorizontalAlignment.Right : System.Windows.HorizontalAlignment.Left;
            txtctrl.Text = textstr;
            txtctrl.FontSize = textFontSize;
            txtctrl.Foreground = textForeground;
            txtctrl.Background = textBackground;
            txtctrl.Margin = new System.Windows.Thickness(4);   //Margin 4
            Grid.SetRow(txtctrl, row);
            Grid.SetColumn(txtctrl, col);
            rootGrid.Children.Add(txtctrl);
        }

        /// <summary>
        /// 添加一个测试结果到Grid中
        /// </summary>
        /// <param name="rootGrid">Grid</param>
        /// <param name="testingInfo">测试结果</param>
        /// <param name="rowIndex">结果在Grid中的行</param>
        /// <param name="showName">是否显示测试的名称</param>
        private void AddOneTestingToGrid(Grid rootGrid, BaseSelfTestInfo testingInfo, int rowIndex, bool showName)
        {
            if(showName)
                AddTextBlockToGrid(rootGrid, testingInfo.DisplayName(language), rowIndex, 0, false);

            AddTextBlockToGrid(rootGrid, "光谱区间:", rowIndex, 1, true);
            AddTextBlockToGrid(rootGrid, testingInfo.firstX.ToString() + " - " + testingInfo.lastX.ToString() + "cm-1", rowIndex, 2, false);

            AddTextBlockToGrid(rootGrid, "测量值:", rowIndex, 3, true);
            double tempresult = Math.Round(testingInfo.FinalResult * 1000) / 1000;
            AddTextBlockToGrid(rootGrid, tempresult.ToString()+testingInfo.ResultUnit, rowIndex, 4, false);

            AddTextBlockToGrid(rootGrid, "阈值:", rowIndex, 5, true);
            string thresoldstr = null;
            if (testingInfo.LessThresold == double.MaxValue)
                thresoldstr = "< " + (testingInfo.TargetResult + testingInfo.GreatThresold).ToString();
            else if (testingInfo.GreatThresold == double.MaxValue)
                thresoldstr = "> " + (testingInfo.TargetResult - testingInfo.LessThresold).ToString();
            else if (testingInfo.LessThresold == testingInfo.GreatThresold)
                thresoldstr = testingInfo.TargetResult.ToString() + "±" + testingInfo.GreatThresold.ToString();
            else
                thresoldstr = (testingInfo.TargetResult - testingInfo.LessThresold).ToString() + " - " + (testingInfo.TargetResult + testingInfo.GreatThresold).ToString();
            AddTextBlockToGrid(rootGrid, thresoldstr + testingInfo.ResultUnit, rowIndex, 6, false);

            var image = CreateResultImage(testingInfo.IsValidResult());
            Grid.SetRow(image, rowIndex);
            Grid.SetColumn(image, 7);
            rootGrid.Children.Add(image);
        }


        /// <summary>
        /// 添加一个测试结果到Grid中
        /// </summary>
        /// <param name="rootGrid">Grid</param>
        /// <param name="testingInfo">测试结果</param>
        /// <param name="rowIndex">结果在Grid中的行</param>
        /// <param name="showName">是否显示测试的名称</param>
        private int AddLineSlopTestingToGrid(Grid rootGrid, LineSlopeTestInfo testingInfo, int rowIndex, bool showName)
        {
            if (showName)
                AddTextBlockToGrid(rootGrid, testingInfo.DisplayName(language), rowIndex, 0, false);

            for (int i = 0; i < testingInfo.slopeX.Count; i++)
            {
                AddTextBlockToGrid(rootGrid, "光谱区间:", rowIndex, 1, true);
                AddTextBlockToGrid(rootGrid, testingInfo.slopeX[i].X.ToString() + " - " + testingInfo.slopeX[i].Y.ToString() + "cm-1", rowIndex, 2, false);

                AddTextBlockToGrid(rootGrid, "测量值:", rowIndex, 3, true);
                double tempmin = Math.Round(testingInfo.slopeResult[i].X  * 1000) / 1000;
                double tempmax = Math.Round(testingInfo.slopeResult[i].Y * 1000) / 1000;
                AddTextBlockToGrid(rootGrid, tempmin.ToString()+"-"+tempmax.ToString() + testingInfo.ResultUnit, rowIndex, 4, false);

                AddTextBlockToGrid(rootGrid, "阈值:", rowIndex, 5, true);
                AddTextBlockToGrid(rootGrid, testingInfo.slopeThresold[i].X.ToString() + " - " + testingInfo.slopeThresold[i].Y.ToString() + testingInfo.ResultUnit, rowIndex, 6, false);

                //测量值在阈值区间内
                var image = CreateResultImage(testingInfo.slopeResult[i].X > testingInfo.slopeThresold[i].X && testingInfo.slopeResult[i].Y < testingInfo.slopeThresold[i].Y);
                Grid.SetRow(image, rowIndex);
                Grid.SetColumn(image, 7);
                rootGrid.Children.Add(image);

                rowIndex++;
            }

            return rowIndex;
        }

        /// <summary>
        /// 显示一个测量组合
        /// </summary>
        /// <param name="group">测试组合</param>
        /// <param name="showName">是否显示测量名称</param>
        /// <returns></returns>
        private Grid GenerateOneGroupResult(SelTestGroup group, bool showName)
        {
            TextBlock titleText = null;

            //计算总共有多少行(LineSlopeTestInfo需要特殊处理)
            int rowIndex = rowIndex = group.TestItems.Count * 2;
            var slope = group.TestItems.FirstOrDefault(p => p is LineSlopeTestInfo) as LineSlopeTestInfo;
            if (slope != null)
                rowIndex += slope.slopeX.Count;
            var rootGrid = GenerateOneGroupGrid(rowIndex, out titleText, out Border imageBorder);

            //标题
            titleText.Text = group.DisplayName(language);

            //标题结果图标, 只要有一个没通过就算错误
            var titleImage = CreateResultImage(group.TestItems.FirstOrDefault(p => p.IsValidResult() == false) == null ? true : false);
            titleImage.Width = titleImage.Height = 20;
            imageBorder.Child = titleImage;

            rowIndex = 1;   //跳过标题行
            for(int i=0; i<group.TestItems.Count; i++)
            {
                if (group.TestItems[i] is LineSlopeTestInfo)  //结果有多行, 特殊处理
                    rowIndex = AddLineSlopTestingToGrid(rootGrid, group.TestItems[i] as LineSlopeTestInfo, rowIndex, showName);
                else
                    AddOneTestingToGrid(rootGrid, group.TestItems[i], rowIndex, showName);
                rowIndex++;

                //添加测试之间的分隔线
                if (i < group.TestItems.Count - 1)
                {
                    GridSplitter sp = new GridSplitter();
                    sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    sp.Height = 1;
                    sp.Background = System.Windows.Media.Brushes.Gray;
                    sp.Margin = new System.Windows.Thickness(5, 0, 5, 0);
                    Grid.SetColumnSpan(sp, rootGrid.ColumnDefinitions.Count);
                    Grid.SetRow(sp, rowIndex);
                    rootGrid.Children.Add(sp);

                    rowIndex++;
                }
            }

            return rootGrid;
        }

        /// <summary>
        /// 创建grid的外框
        /// </summary>
        /// <param name="contentGrid"></param>
        /// <returns></returns>
        private Border GenerateOuterBorder(Grid contentGrid)
        {
            Border border = new Border();
            border.BorderThickness = new System.Windows.Thickness(1);
            border.BorderBrush = System.Windows.Media.Brushes.Black;
            border.Child = contentGrid;
            border.Margin = new System.Windows.Thickness(0, 10, 0, 10);

            return border;
        }

        /// <summary>
        /// 创建测试的基本信息
        /// </summary>
        /// <returns></returns>
        private Grid GenerageReportTestBasenfo()
        {
            TextBlock titleText;
            Border imageBorder;
            Grid contentGrid = GenerateOneGroupGrid(3, out titleText, out imageBorder);

            titleText.Text = "测试信息";
            
            AddTextBlockToGrid(contentGrid, "测试单位", 1, 0, true);
            AddTextBlockToGrid(contentGrid, UnitName , 1, 1, true);

            AddTextBlockToGrid(contentGrid, "测试人员", 1, 2, true);
            AddTextBlockToGrid(contentGrid, UserName, 1, 3, true);

            AddTextBlockToGrid(contentGrid, "测试时间", 1, 4, true);
            AddTextBlockToGrid(contentGrid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1, 5, true);

            AddTextBlockToGrid(contentGrid, "仪器类型", 2, 0, true);
            AddTextBlockToGrid(contentGrid, scanner == null ? null : Common.Extenstion.EnumExtensions.GetEnumDescription(scanner.ConnectedDevice.Category, Language), 2, 1, true);

            AddTextBlockToGrid(contentGrid, "仪器型号", 2, 2, true);
            AddTextBlockToGrid(contentGrid, scanner == null ? null : Common.Extenstion.EnumExtensions.GetEnumDescription(scanner.ConnectedDevice.Model, Language), 2, 3, true);

            AddTextBlockToGrid(contentGrid, "序列号", 2, 4, true);
            AddTextBlockToGrid(contentGrid, scanner?.GetSerialNumber(), 2, 5, true);

            return contentGrid;
        }

        /// <summary>
        /// 设置最终测试结果信息
        /// </summary>
        /// <param name="groups">测量类别</param>
        /// <param name="rootBorder">根元素</param>
        private void SetFinalResult(List<SelTestGroup> groups, Border rootBorder)
        {
            bool successed = true;
            foreach(var group in groups)
            {
                foreach(var item in group.TestItems)
                {
                    if (item.IsValidResult() == false)
                        successed = false;
                }
            }

            //结果
            var txtctrl = Ai.Hong.Controls.Common.XPSReportTemplate.GetElement<TextBlock>(rootBorder, "txtAllResult");
            if(successed)
            {
                txtctrl.Text ="测试结果 = 通过";
                txtctrl.Foreground = System.Windows.Media.Brushes.LimeGreen;
            }
            else
            {
                txtctrl.Text ="测试结果 = 未通过";
                txtctrl.Foreground = System.Windows.Media.Brushes.Red;
            }

            //Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(detailBorder, "txtAllResult", "测试结果=" + (successed ? "通过" : "未通过"));

            //结果图标
            var imgBorder = Ai.Hong.Controls.Common.XPSReportTemplate.GetElement<Border>(rootBorder, "resultImageBorder");
            var image = CreateResultImage(successed);
            image.Width = image.Height = 20;
            imgBorder.Child = image;

            //日期
            Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "testingDate", DateTime.Now.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// 创建总报告
        /// </summary>
        /// <param name="groups">测试组合的列表</param>
        /// <param name="reportTitle">报告标题</param>
        /// <returns></returns>
        public Border GenerateTotalPage(List<SelTestGroup> groups, string reportTitle)
        {
            Border rootBorder = Ai.Hong.Controls.Common.XPSReportTemplate.CloneObject(totalBorder);
            Grid rootGrid = rootBorder.Child as Grid;

            //报告名称
            Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "txtReportTitle", reportTitle);

            //报告内容
            for (int i = -1; i < groups.Count; i++ )
            {
                Grid contentGrid = null;
                if(i == -1)
                    contentGrid = GenerageReportTestBasenfo();    //测试的基本信息
                else
                    contentGrid = GenerateOneGroupResult(groups[i], true);  //具体测试信息

                var border = GenerateOuterBorder(contentGrid);
                Grid.SetRow(border, i + 2);    //row 0是report Title, 1=base info
                rootGrid.Children.Add(border);
            }

            SetFinalResult(groups, rootBorder);

            ////报告结尾
            //bool totalResult = true;
            //foreach(var group in groups)
            //{
            //    foreach (var item in group.TestItems)
            //    {
            //        if (item.IsValidResult() == false)
            //            totalResult = false;
            //    }
            //}
            //Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "txtAllResult", "测试结果 = " + (totalResult ? "通过" : "未通过"));
            //Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "testingDate", DateTime.Now.ToString("yyyy-MM-dd"));

            return rootBorder;
        }

        /// <summary>
        /// 创建一页详细测试结果报告
        /// </summary>
        /// <param name="hardware">设备类型</param>
        /// <param name="testingInfo">测试结果</param>
        /// <returns></returns>
        public Border GenerateDetailPage(DeviceHardware hardware, BaseSelfTestInfo testingInfo)
        {
            Border rootBorder = Ai.Hong.Controls.Common.XPSReportTemplate.CloneObject(detailBorder);
            Grid rootGrid = rootBorder.Child as Grid;

            //测试名称
            Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "testingTitle", testingInfo.DisplayName(language));

            TextBlock titleText = null;
            Border imageBorder = null;

            //创建结果Grid
            int rowCount = 2;
            if (testingInfo is LineSlopeTestInfo)    //结果有多行, 特殊处理
                rowCount += (testingInfo as LineSlopeTestInfo).slopeX.Count;
            var resultGrid = GenerateOneGroupGrid(rowCount, out  titleText, out imageBorder);
            resultGrid.Margin = new System.Windows.Thickness(0, 10, 0, 10);

            titleText.Text = "测试结果";
            if (testingInfo is LineSlopeTestInfo)    //结果有多行, 特殊处理
                AddLineSlopTestingToGrid(resultGrid, testingInfo as LineSlopeTestInfo, 1, false);
            else
                AddOneTestingToGrid(resultGrid, testingInfo, 1, false);
            Grid.SetRow(resultGrid, 1);
            rootGrid.Children.Add(resultGrid);

            //光谱图
            var bmpfile = System.IO.Path.GetTempFileName();
            Ai.Hong.Charts.SpectrumChart chart = new Ai.Hong.Charts.SpectrumChart();

            string spctype = FileFormat.FileFormat.GetYAxisTypeName(testingInfo.SpectraDatas[0].dataInfo.dataType, false);
            Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "txtChartType", spctype);

            //显示光谱图
            foreach (var item in testingInfo.SpectraDatas)
                chart.AddChart(item.xDatas, item.yDatas, System.Windows.Media.Brushes.Black, Guid.NewGuid());
            chart.SaveToBitmapFile(bmpfile, 1000, 500, System.Windows.Media.Brushes.White);

            System.Windows.Controls.Image spcChartImage = new Image();
            Ai.Hong.Common.CommonMethod.SetImageSource(spcChartImage, bmpfile, null);
            //System.IO.File.Delete(bmpfile);
            Border border = rootBorder.FindName("borderGraphic") as Border;
            border.Child = spcChartImage;

            //光谱文件列表
            string files = null;
            foreach (var item in testingInfo.SpectraDatas)
                files += item.fileInfo.filename + "\r\n";
            //border = rootBorder.FindName("borderGraphicPath") as Border;
            for (int i = 0; i < 2; i++)
                files += files;
            Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "filePaths", files);

            //测量参数
            Grid paraGrid = rootBorder.FindName("gridParameters") as Grid;
            var scanpara = testingInfo.AcquireParameter;
            var parasEnglish = new string[] { "Resolution", "Count", "BackGain", "ZeroFilling", "PhaseCorrect", "Apodization" };
            var parasChinese = new string[] { "分辨率", "扫描次数", "背景增益", "填零系数", "截趾函数", "相位校正方法", "相位分辨率" };
            var paraValues = new string[] { scanpara.Resolution.ToString(), scanpara.ScanCount.ToString(), scanpara.BackGain.ToString(), scanpara.ZeroFilling.ToString(), scanpara.Apodization.ToString(), scanpara.PhaseCorrect.ToString(), scanpara.PhaseResolution.ToString() };

            for (int index = 0; index < paraValues.Length; index++)
            {
                if(Language == EnumLanguage.Chinese)
                    AddTextToGrid(paraGrid, parasChinese[index] + ":", index, true);
                else
                    AddTextToGrid(paraGrid, parasEnglish[index] + ":", index, true);
                AddTextToGrid(paraGrid, paraValues[index], index, false);
            }

            return rootBorder;
        }

        /// <summary>
        /// 创建结果图标
        /// </summary>
        /// <param name="result">结果，NULL=Unknown</param>
        /// <returns></returns>
        private Ai.Hong.Controls.VectorImage CreateResultImage(bool? result)
        {
            Ai.Hong.Controls.VectorImage image = new Ai.Hong.Controls.VectorImage();
            string key = null;
            System.Windows.Media.SolidColorBrush brush;
            if (result == null)
            {
                key = "InfoWithCircelGeometry";
                brush = System.Windows.Media.Brushes.Goldenrod;
            }
            else if (result == true)
            {
                key = "SingleRightGeometry";
                brush = System.Windows.Media.Brushes.LimeGreen;
            }
            else
            {
                key = "SingleWrongGeometry";
                brush = System.Windows.Media.Brushes.IndianRed;
            }

            image.VectorSource = resourceDir[key] as System.Windows.Media.DrawingGroup;
            image.Height = 12;
            image.Width = 12;
            image.DrawColor = brush;
            image.Margin = new System.Windows.Thickness(0, 0, 4, 0);
            return image;
        }

        /// <summary>
        /// 创建并保存测试报告
        /// </summary>
        /// <param name="hardware">设备硬件</param>
        /// <param name="filename">XPS文件名</param>
        /// <param name="groups">测试组合的列表</param>
        /// <param name="reportTitle">报告的标题</param>
        /// <returns></returns>
        public bool CreateAndSaveXPSFile(DeviceHardware hardware, string filename, List<SelTestGroup> groups, string reportTitle)
        {
            FixedDocument fixedDoc = new FixedDocument();

            //总报告
            Border border = GenerateTotalPage(groups, reportTitle);
            var page = Ai.Hong.Controls.Common.XPSReportTemplate.CreatePageContent(border);
            fixedDoc.Pages.Add(page);

            //详细报告
            foreach (var group in groups)
            {
                foreach (var item in group.TestItems)
                {
                    border = GenerateDetailPage(hardware, item);
                    page = Ai.Hong.Controls.Common.XPSReportTemplate.CreatePageContent(border);
                    fixedDoc.Pages.Add(page);
                }
            }

            return Ai.Hong.Controls.Common.XPSReportTemplate.SaveToXpsFile(filename, fixedDoc);
        }
    }
}
