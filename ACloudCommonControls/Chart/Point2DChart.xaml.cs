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

namespace Ai.Hong.Charts
{
    /// <summary>
    /// PredictResultChart.xaml 的交互逻辑
    /// </summary>
    public partial class Point2DChart : System.Windows.Controls.UserControl
    {
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

        Dictionary<PointSharp, MarkerStyle> sharpDict = new Dictionary<PointSharp, MarkerStyle>() {
                        { PointSharp.Pyramid, MarkerStyle.Circle } ,
                        { PointSharp.Cube, MarkerStyle.Circle } ,
                        { PointSharp.Sphere, MarkerStyle.Circle } ,
                        { PointSharp.Square, MarkerStyle.Square } ,
                        { PointSharp.Triangle, MarkerStyle.Triangle } ,
                        { PointSharp.Circle, MarkerStyle.Circle } ,
                        { PointSharp.Diamond, MarkerStyle.Diamond } ,
                        { PointSharp.Cross, MarkerStyle.Cross } ,
                        { PointSharp.Star4, MarkerStyle.Star4 } ,
                        { PointSharp.Star5, MarkerStyle.Star5 } ,
                        { PointSharp.Star6, MarkerStyle.Star6 } ,
                        { PointSharp.Star10, MarkerStyle.Star10 } 
                };
        private List<PointData> allChartPoints = new List<PointData>();

        /// <summary>
        /// 2D图像控件
        /// </summary>
        public Point2DChart()
        {
            InitializeComponent();

            mainArea = new ChartArea(areaName);
            resultChart.ChartAreas.Add(mainArea);

            mainChart = new Series();
            mainChart.ChartArea = areaName;
            mainChart.ChartType = SeriesChartType.Point;
            resultChart.Series.Add(mainChart);
        }

        /// <summary>
        /// 添加图形数据点
        /// </summary>
        /// <param name="datas"></param>
        public void AddPoints(IList<PointData> datas)
        {
            foreach (var pt in datas)
                AddPoint(pt);

            AdjustChartAxis(mainChart);
        }

        /// <summary>
        /// 增加数据点
        /// </summary>
        /// <param name="data">数据点数据</param>
        public void AddPoint(PointData data)
        {
            int index = mainChart.Points.AddXY(data.centerX, data.centerY);
            mainChart.Points[index].BorderColor = data.winformColor;
            if (data.solid)
                mainChart.Points[index].Color = data.winformColor;
            mainChart.Points[index].MarkerStyle = sharpDict[data.pointSharp];
            mainChart.Points[index].Tag = data.key;
        }

        /// <summary>
        /// 移除图像数据点
        /// </summary>
        /// <param name="IDs">数据点的ID</param>
        public void RemovePoints(IList<Guid> IDs)
        {
            if (mainChart.Points.Count == 0 || IDs == null || IDs.Count == 0)
                return;

            //获取图像中的所有数据点
            var chartPoints = mainChart.Points.ToList();

            //移除包含在IDs里面的数据点（图像数据点的Tag是Guid）
            chartPoints.RemoveAll(p => IDs.FirstOrDefault(id => id == (Guid)p.Tag) != null);

            //清除图像，然后加入没有移除的数据点
            mainChart.Points.Clear();
            foreach (var pt in chartPoints)
                mainChart.Points.Add(pt);
        }
        
        /// <summary>
        /// 清除图像
        /// </summary>
        public void ClearChart()
        {
            mainChart.Points.Clear();
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
    }
}
