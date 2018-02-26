using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.Windows.Media;

namespace ACloudCommonControls.Chart
{
    /// <summary>
    /// PredictResultChart.xaml 的交互逻辑
    /// </summary>
    public partial class PointChart : System.Windows.Controls.UserControl
    {
        private int compIndex = 0;
        private int rank = 1;

        /// <summary>
        /// 图形区域名称
        /// </summary>
        private const string areaName = "mainArea";

        /// <summary>
        /// 图形区域
        /// </summary>
        private ChartArea mainArea = null;

        /// <summary>
        /// 图形
        /// </summary>
        private Series mainChart = null;

        /// <summary>
        /// 正常显示颜色
        /// </summary>
        private System.Drawing.Color unselColor = System.Drawing.Color.Blue;
        /// <summary>
        /// 选中显示颜色
        /// </summary>
        private System.Drawing.Color selColor = System.Drawing.Color.Red;
        private System.Drawing.Color transColor = System.Drawing.Color.Transparent;

        private MarkerStyle[] pointStyles = new MarkerStyle[]{MarkerStyle.Square, MarkerStyle.Circle, MarkerStyle.Diamond, 
            MarkerStyle.Triangle, MarkerStyle.Cross, MarkerStyle.Star4, MarkerStyle.Star5, MarkerStyle.Star6, MarkerStyle.Star10};

        public PointChart()
        {
            InitializeComponent();

            mainArea = new ChartArea(areaName);
            resultChart.ChartAreas.Add(mainArea);
        }

        /// <summary>
        /// 设置需要显示的数据
        /// </summary>
        /// <param name="plsFile"></param>
        /// <param name="datas"></param>
        public void SetResultDatas()
        {
        }

        /// <summary>
        /// 获取每个数据是否为排除状态
        /// </summary>
        /// <returns></returns>
        private bool[] GetDataSelectState()
        {
            //if (curDatas == null || curDatas.Count() == 0 || compIndex == -1)
            //    return null;

            //bool[] state = new bool[curDatas.Count()];
            //int i = 0;
            //foreach (var item in curDatas)
            //{
            //    state[i++] = item.spectrum.status[compIndex] == enumFileState.exclude;
            //}
            //return state;

            return new bool[10];
        }

        /// <summary>
        /// 获取一个数据是否为排除状态
        /// </summary>
        private bool GetDataSelectState(int dataIndex)
        {
            return false;
            //return curDatas.ElementAt(dataIndex).spectrum.status[compIndex] == enumFileState.exclude;
        }

        /// <summary>
        /// 设置数据状态（是否排除）
        /// </summary>
        /// <param name="dataIndex">数据序号</param>
        /// <param name="exclude">状态</param>
        private void SetDataSelectState(int dataIndex, bool excludeState)
        {
            //curDatas.ElementAt(dataIndex).spectrum.status[compIndex] = excludeState ? enumFileState.exclude : enumFileState.calibration;
        }

        /// <summary>
        /// 真值/绝对偏差
        /// </summary>
        private void DrawRealvsPredictChart()
        {
            //if (resultChart == null)
            //    return;

            //resultChart.Series.Clear();
            //if (curDatas == null)
            //    return;
            //double[] xdatas = (from item in curDatas select item.predictConcent).ToArray();
            //double[] ydatas = (from item in curDatas select item.realConcent).ToArray();

            ////int compIndex = curDatas.ElementAt(0).co
            //bool[] outleters = GetDataSelectState();

            //string xtitle = Common.CommonClass.ResourceString("ResultChartPanel_PredictConcentration");
            //string ytitle = Common.CommonClass.ResourceString("ResultChartPanel_RealConcentration");

            //DrawEachResultChart(xdatas, ydatas, outleters, xtitle, ytitle);
        }

        /// <summary>
        /// 绝对偏差/真值
        /// </summary>
        private void DrawAbsolutevsRealChart()
        {
            //resultChart.Series.Clear();
            //if (curDatas == null)
            //    return;
            //double[] xdatas = (from item in curDatas select item.realConcent).ToArray();
            //double[] ydatas = (from item in curDatas select item.absoluteDifferent).ToArray();
            //bool[] outleters = GetDataSelectState();

            //string xtitle = Common.CommonClass.ResourceString("ResultChartPanel_RealConcentration");
            //string ytitle = Common.CommonClass.ResourceString("ResultChartPanel_AbsoluteDifferent"); 

            //DrawEachResultChart(xdatas, ydatas, outleters, xtitle, ytitle);
        }

        /// <summary>
        /// 相对偏差/真值
        /// </summary>
        private void DrawPercentvsRealChart()
        {
            //resultChart.Series.Clear();
            //if (curDatas == null)
            //    return;
            //double[] xdatas = (from item in curDatas select item.realConcent).ToArray();
            //double[] ydatas = (from item in curDatas select item.percentDifferent).ToArray();
            //bool[] outleters = GetDataSelectState();

            //string xtitle = Common.CommonClass.ResourceString("ResultChartPanel_RealConcentration"); 
            //string ytitle = Common.CommonClass.ResourceString("ResultChartPanel_PercentDifferent"); 

            //DrawEachResultChart(xdatas, ydatas, outleters, xtitle, ytitle);
        }

        /// <summary>
        /// 实际汇总图形
        /// </summary>
        /// <param name="xdatas">X轴的值</param>
        /// <param name="ydatas">Y轴的值</param>
        /// <param name="outleters">需要选中的数据</param>
        /// <param name="xAxisTitle">X轴标题</param>
        /// <param name="yAxisTitle">Y轴标题</param>
        private void DrawEachResultChart(double[] xdatas, double[] ydatas, bool[] outleters, string xAxisTitle, string yAxisTitle)
        {
            //if (xdatas == null || ydatas == null || outleters==null ||
            //    xdatas.Length == 0 || xdatas.Length != ydatas.Length || xdatas.Length != outleters.Length)
            //    return;

            //mainChart = new Series();
            //mainChart.ChartArea = areaName;
            
            ////当前图形形状
            //MarkerStyle style = listPointType.SelectedIndex == -1 ? MarkerStyle.Circle : pointStyles[listPointType.SelectedIndex];
            //DataPoint
            //for(int i=0; i<xdatas.Length; i++)
            //{
            //    mainChart.Points.AddXY(xdatas[i], ydatas[i]);

            //    int index = mainChart.Points.Count - 1;
            //    mainChart.Points[index].MarkerStyle = style;
            //    mainChart.Points[index].MarkerSize = 10;
            //    //空心的，为Transparent，否则，如果是outleter, 显示红色。
            //    mainChart.Points[index].MarkerColor = listFillType.SelectedIndex == 0 ? (outleters[i] ? selColor : unselColor) : transColor;
            //    mainChart.Points[index].BorderColor = outleters[i] ? selColor : unselColor;
            //    mainChart.Points[index].ToolTip = "(" + xdatas[i].ToString("F2") + "," + xdatas[i].ToString("F2") + ")";
            //}
            //mainChart.ChartType = SeriesChartType.Point;

            //mainArea.AxisX.Title = xAxisTitle;
            //mainArea.AxisY.Title = yAxisTitle;
            //mainArea.InnerPlotPosition.Auto = true;

            //AdjustChartAxis(mainChart);

            ////resultChart.ChartAreas["mainArea"].InnerPlotPosition.X = 5;
            ////resultChart.ChartAreas["mainArea"].InnerPlotPosition.Y = 1;
            ////resultChart.ChartAreas["mainArea"].InnerPlotPosition.Width = 90;
            ////resultChart.ChartAreas["mainArea"].InnerPlotPosition.Height = 90;
            //resultChart.Series.Add(mainChart);
        }

        /// <summary>
        /// 调整图像的坐标
        /// </summary>
        /// <param name="sr">图像</param>
        private void AdjustChartAxis(Series sr)
        {
            double axisMin, axisMax, axisStep;
            double minx = (from item in sr.Points select item.XValue).Min();
            double maxx = (from item in sr.Points select item.XValue).Max();
            double miny = (from item in sr.Points select item.YValues[0]).Min();
            double maxy = (from item in sr.Points select item.YValues[0]).Max();

            GetMaxAxisValue(miny, maxy, out axisMin, out axisMax, out axisStep);
            mainArea.AxisY.Minimum = axisMin;
            mainArea.AxisY.Maximum = axisMax;
            mainArea.AxisY.Interval = axisStep;

            GetMaxAxisValue(minx, maxx, out axisMin, out axisMax, out axisStep);
            mainArea.AxisX.Minimum = axisMin;
            mainArea.AxisX.Maximum = axisMax;
            mainArea.AxisX.Interval = axisStep;
        }

        /// <summary>
        /// 计算图像的坐标轴的最大最小值和步长
        /// </summary>
        /// <param name="minValue">显示数据的最小值</param>
        /// <param name="maxValue">显示数据的最大值</param>
        /// <param name="axisMin">返回坐标轴的最小值</param>
        /// <param name="axisMax">返回坐标轴的最大值</param>
        /// <param name="axisStep">返回坐标轴的步长</param>
        private void GetMaxAxisValue(double minValue, double maxValue, out double axisMin, out double axisMax, out double axisStep)
        {
            double diffValue = Math.Abs(maxValue - minValue) / 10;
            double power = 0;
            if (diffValue < 0.1)
                power = 1;
            else if (diffValue > 1)
                power = -1;
            else
                power = 0;

            int count = 0;
            while (diffValue < 0.1 || diffValue > 1)
            {
                diffValue *= Math.Pow(10, power);
                count++;
            }

            diffValue = Math.Truncate(diffValue * 10) / 10 ;

            if (diffValue == 0.1)
                axisStep = 0.1;
            else if (diffValue <= 0.2)
                axisStep = 0.2;
            else if (diffValue <= 0.25)
                axisStep = 0.25;
            else if (diffValue <= 0.3)
                axisStep = 0.3;
            else if (diffValue <= 0.4)
                axisStep = 0.4;
            else if (diffValue <= 0.5)
                axisStep = 0.5;
            else if (diffValue <= 0.6)
                axisStep = 0.6;
            else if (diffValue <= 0.7)
                axisStep = 0.7;
            else if (diffValue <= 0.75)
                axisStep = 0.75;
            else if (diffValue <= 0.8)
                axisStep = 0.8;
            else if (diffValue <= 0.9)
                axisStep = 0.9;
            else if (diffValue <= 1.0)
                axisStep = 1.0;

            axisStep = diffValue / Math.Pow(10, count * power);
            if(minValue > 0)
                axisMin = axisStep * Math.Truncate(minValue / axisStep);
            else
                axisMin = axisStep * (Math.Truncate(minValue / axisStep) - 1);

            if(maxValue > 0)
                axisMax = axisStep * (1 + Math.Truncate(maxValue / axisStep));
            else
                axisMax = axisStep * Math.Truncate(maxValue / axisStep);
        }

        Boolean bHaveMouse;
        System.Drawing.Point ptOriginal = new System.Drawing.Point();
        System.Drawing.Point ptLast = new System.Drawing.Point();

        private void resultChart_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Make a note that we "have the mouse".
            bHaveMouse = true;
            // Store the "starting point" for this rubber-band rectangle.
            ptOriginal.X = e.X;
            ptOriginal.Y = e.Y;
            // Special value lets us know that no previous
            // rectangle needs to be erased.
            ptLast.X = -1;
            ptLast.Y = -1;
        }

        // Convert and normalize the points and draw the reversible frame.
        private void MyDrawReversibleRectangle(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            System.Drawing.Rectangle rc = new System.Drawing.Rectangle();

            // Convert the points to screen coordinates.
            p1 = resultChart.PointToScreen(p1);
            p2 = resultChart.PointToScreen(p2);
            // Normalize the rectangle.
            if (p1.X < p2.X)
            {
                rc.X = p1.X;
                rc.Width = p2.X - p1.X;
            }
            else
            {
                rc.X = p2.X;
                rc.Width = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                rc.Y = p1.Y;
                rc.Height = p2.Y - p1.Y;
            }
            else
            {
                rc.Y = p2.Y;
                rc.Height = p1.Y - p2.Y;
            }
            // Draw the reversible frame.
            ControlPaint.DrawReversibleFrame(rc, System.Drawing.Color.Black, FrameStyle.Dashed);
        }

        private void resultChart_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Drawing.Point ptCurrent = new System.Drawing.Point(e.X, e.Y);
            // If we "have the mouse", then we draw our lines.
            if (bHaveMouse)
            {
                // If we have drawn previously, draw again in
                // that spot to remove the lines.
                if (ptLast.X != -1)
                {
                    MyDrawReversibleRectangle(ptOriginal, ptLast);
                }
                // Update last point.
                ptLast = ptCurrent;
                // Draw new lines.
                MyDrawReversibleRectangle(ptOriginal, ptCurrent);
            }
        }

        /// <summary>
        /// 按照大小交换数据
        /// </summary>
        private void ExchangeValueWhenInverted(ref double minValue, ref double maxValue)
        {
            if (minValue > maxValue)
            {
                double temp = maxValue;
                maxValue = minValue;
                minValue = temp;
            }
        }

        private void resultChart_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //// Set internal flag to know we no longer "have the mouse".
            //bHaveMouse = false;
            //// If we have drawn previously, draw again in that spot
            //// to remove the lines.
            //if (ptLast.X != -1)
            //{
            //    Point ptCurrent = new Point(e.X, e.Y);
            //    MyDrawReversibleRectangle(ptOriginal, ptLast);
            //}

            //if (ptLast.X > 0 && ptLast.Y > 0 && ptOriginal.X > 0 && ptOriginal.Y > 0)
            //{
            //    if (radioSelect.IsChecked==true || radioUnselect.IsChecked==true)
            //    {
            //        //选择所有框中的点
            //        double xsellast = mainArea.AxisX.PixelPositionToValue(ptLast.X);
            //        double xselbegin = mainArea.AxisX.PixelPositionToValue(ptOriginal.X);
            //        double ysellast = mainArea.AxisY.PixelPositionToValue(ptLast.Y);
            //        double yselbegin = mainArea.AxisY.PixelPositionToValue(ptOriginal.Y);

            //        ExchangeValueWhenInverted(ref xselbegin, ref xsellast);
            //        ExchangeValueWhenInverted(ref yselbegin, ref ysellast);

            //        foreach (var item in mainChart.Points)
            //        {
            //            if (item.XValue > xselbegin && item.XValue < xsellast &&
            //                item.YValues[0] > yselbegin && item.YValues[0] < ysellast)
            //            {
            //                //修改显示颜色
            //                System.Drawing.Color setcolor = radioSelect.IsChecked == true ? selColor:unselColor;
            //                item.MarkerBorderColor = setcolor;
            //                item.MarkerColor = listFillType.SelectedIndex == 0 ? setcolor : transColor;

            //                //设置当前数据的状态
            //                int index = mainChart.Points.IndexOf(item);
            //                SetDataSelectState(index, radioSelect.IsChecked == true);
            //            }
            //        }
            //    }
            //}

            //// Set flags to know that there is no "previous" line to reverse.
            //ptLast.X = -1;
            //ptLast.Y = -1;
            //ptOriginal.X = -1;
            //ptOriginal.Y = -1;
        }

        /// <summary>
        /// 双击鼠标选择或者取消选择Point
        /// </summary>
        private void resultChart_DoubleClick(object sender, EventArgs e)
        {
            //MouseEventArgs arg = e as MouseEventArgs;
            //HitTestResult result = resultChart.HitTest(arg.X, arg.Y);

            //if (result != null)
            //{
            //    if (result.ChartArea.Name == areaName && result.Series == resultChart.Series[0] && result.PointIndex >= 0)
            //    {
            //        ComponentPredictData data = curDatas.ElementAt(result.PointIndex);
            //        if (data != null)
            //        {
            //            bool selected = !GetDataSelectState(result.PointIndex);     //反转当前状态
            //            System.Drawing.Color color = selected ? selColor : unselColor;
            //            mainChart.Points[result.PointIndex].MarkerBorderColor = color;
            //            mainChart.Points[result.PointIndex].MarkerColor = listFillType.SelectedIndex == 0 ? color : transColor;

            //            //设置当前数据的状态
            //            SetDataSelectState(result.PointIndex, selected);
            //        }
            //    }
            //}
        }

        private void listPointType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (mainChart == null || resultChart.Series.Count == 0 || listPointType.SelectedIndex == -1)
            //    return;

            //MarkerStyle style = pointStyles[listPointType.SelectedIndex];
            //foreach (var item in mainChart.Points)
            //    item.MarkerStyle = style;
        }

        private void listFillType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (mainChart == null || resultChart.Series.Count == 0 || listPointType.SelectedIndex == -1)
            //    return;
            
            //bool trans = listFillType.SelectedIndex == 1;

            //foreach (var item in mainChart.Points)
            //    item.MarkerColor = trans ? System.Drawing.Color.Transparent : item.MarkerBorderColor;
        }

        private void listChartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (listChartType.SelectedIndex == 0)   //真值\预测值
            //    DrawRealvsPredictChart();
            //else if (listChartType.SelectedIndex == 1)   //绝对偏差\真值
            //    DrawAbsolutevsRealChart();
            //else if (listChartType.SelectedIndex == 2)   //相对偏差\真值
            //    DrawPercentvsRealChart();

        }
    }
}
