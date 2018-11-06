using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Series;

namespace Ai.Hong.Charts
{
    /// <summary>
    /// 图形属性
    /// </summary>
    class GraphicInfo
    {
        public EnumChartType ChartType = EnumChartType.ScatterSeries;

        /// <summary>
        /// 图形（根据ChartType对应具体的图形类型）
        /// </summary>
        public Series Chart;

        /// <summary>
        /// 光谱ID
        /// </summary>
        public Guid key;

        /// <summary>
        /// 图形颜色
        /// </summary>
        public SolidColorBrush chartColor { get; set; }

        /// <summary>
        /// 图形名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 图形的数据点
        /// </summary>
        public List<DataPoint> Points { get { return GetDataPoints(); } }

        /// <summary>
        /// 线条的颜色
        /// </summary>
        public SolidColorBrush LineColor { get { return GetLineColor(); } set { SetLineColor(value); } }

        /// <summary>
        /// 线条宽度
        /// </summary>
        public double LineWidth { get { return GetLineWidth(); } set { SetLineWidth(value); } }

        /// <summary>
        /// 图形显示位移（Y轴)
        /// </summary>
        public double offset { get; set; }

        /// <summary>
        /// 图形的标注
        /// </summary>
        public List<AnnotationInfo> Annotations { get; set; } = new List<AnnotationInfo>();

        public GraphicInfo()
        {
        }
        
        /// <summary>
        /// 创建LineSeries Chart图像
        /// </summary>
        /// <param name="xDatas">x轴数据</param>
        /// <param name="yDatas">y轴数据</param>
        /// <param name="color">颜色</param>
        /// <param name="lineWidth">线宽</param>
        /// <returns></returns>
        public static LineSeries CreateLineSeries(double[] xDatas, double[] yDatas, SolidColorBrush color, double lineWidth)
        {
            if (xDatas == null || yDatas == null || xDatas.Length == 0 || xDatas.Length != yDatas.Length)
                return null;

            LineSeries newchart = new LineSeries();
            for (int i = 0; i < xDatas.Length; i++)
                newchart.Points.Add(new DataPoint(xDatas[i], yDatas[i]));

            newchart.LineStyle = LineStyle.Solid;
            newchart.SelectionMode = OxyPlot.SelectionMode.Multiple;
            newchart.Selectable = true;
            newchart.StrokeThickness = lineWidth;
            newchart.Color = ChartCommonMethod.ToOxyColor(color);

            return newchart;
        }

        /// <summary>
        /// 创建ScatterSeries图像
        /// </summary>
        /// <param name="xDatas">x轴数据</param>
        /// <param name="yDatas">y轴数据</param>
        /// <param name="markerType">数据点形状</param>
        /// <param name="markerSize">数据点大小</param>
        /// <param name="borderColor">边框颜色</param>
        /// <param name="fillColor">填充颜色</param>
        /// <param name="lineWidth">边框线宽</param>
        /// <returns></returns>
        public static ScatterSeries CreateScatterSeries(double[] xDatas, double[] yDatas, EnumMarkerType markerType, double markerSize, SolidColorBrush borderColor, SolidColorBrush fillColor, double lineWidth)
        {
            if (xDatas == null || yDatas == null || xDatas.Length == 0 || xDatas.Length != yDatas.Length)
                return null;

            ScatterSeries newchart = new ScatterSeries();
            for (int i = 0; i < xDatas.Length; i++)
                newchart.Points.Add(new ScatterPoint(xDatas[i], yDatas[i]));

            newchart.MarkerType = (MarkerType)markerType;
            newchart.MarkerSize = markerSize;
            newchart.SelectionMode = OxyPlot.SelectionMode.Multiple;
            newchart.Selectable = true;
            newchart.MarkerStrokeThickness = lineWidth;
            newchart.MarkerStroke = ChartCommonMethod.ToOxyColor(borderColor);
            newchart.MarkerFill = ChartCommonMethod.ToOxyColor(fillColor);

            return newchart;
        }

        /// <summary>
        /// 创建EllipseSeries图像
        /// x = a * cost * cosθ - b * sint * sinθ + X,
        /// y = a * cost * sinθ + b * sint * cosθ + Y.
        /// </summary>
        /// <param name="x">椭圆中心坐标</param>
        /// <param name="y">椭圆中心坐标</param>
        /// <param name="a">椭圆长轴</param>
        /// <param name="b">椭圆短轴</param>
        /// <param name="angle">倾斜角度</param>
        /// <param name="step">步长(0 - 2π)</param>
        /// <param name="borderColor">边框颜色</param>
        /// <param name="fillColor">填充颜色</param>
        /// <param name="lineWidth">边框线宽</param>
        /// <returns></returns>
        public static AreaSeries CreateEllipseSeries(double x, double y, double a, double b, double angle, double step, SolidColorBrush borderColor, SolidColorBrush fillColor, double lineWidth)
        {
            AreaSeries area = new AreaSeries();
            for (double t = 0; t <= 2 * Math.PI; t += 0.1)
            {
                var curx = a * Math.Cos(t) * Math.Cos(angle) - b * Math.Sin(t) * Math.Sin(angle) + x;
                var cury = a * Math.Cos(t) * Math.Sin(angle) + b * Math.Sin(t) * Math.Cos(angle) + y;
                area.Points.Add(new DataPoint(curx, cury));
            }
            area.Points.Add(new DataPoint(area.Points[0].X, area.Points[0].Y));

            area.StrokeThickness = lineWidth;
            area.BrokenLineColor = ChartCommonMethod.ToOxyColor(borderColor);
            area.BrokenLineThickness = lineWidth;
            area.Fill = ChartCommonMethod.ToOxyColor(fillColor);
            
            return area;
        }

        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="chartID">图形ID</param>
        /// <param name="chart">显示的图形</param>
        /// <param name="chartName">图形名称</param>
        /// <param name="chartType">图形类型</param>
        /// <param name="labelFormat">数据显示格式</param>
        public GraphicInfo(Guid chartID, Series chart, EnumChartType chartType, string chartName, string labelFormat = "F2")
        {
            this.key = chartID;
            this.Chart = chart;
            this.ChartType = chartType;
            this.name = chartName;

            if (!string.IsNullOrWhiteSpace(chartName))
                Chart.TrackerFormatString = System.IO.Path.GetFileNameWithoutExtension(chartName) + Environment.NewLine + "{1}: {2:" + labelFormat + "} , {3}: {4:" + labelFormat + "}";
        }

        /// <summary>
        /// 获取图形线条颜色
        /// </summary>
        /// <returns></returns>
        private SolidColorBrush GetLineColor()
        {
            if (Chart == null)
                return Brushes.White;

            OxyColor oxyColor;
            if (Chart is LineSeries)
                oxyColor = (Chart as LineSeries).Color;
            else if (Chart is FunctionSeries)
                oxyColor = (Chart as FunctionSeries).Color;
            else if (Chart is ScatterSeries)
                oxyColor = (Chart as ScatterSeries).MarkerStroke;
            else
                oxyColor = new OxyColor();

            var cl = new Color() { A = oxyColor.A, R = oxyColor.R, G = oxyColor.G, B = oxyColor.B };
            return new SolidColorBrush(cl);
        }


        /// <summary>
        /// 设置图形线条颜色
        /// </summary>
        /// <param name="color"></param>
        private void SetLineColor(SolidColorBrush color)
        {
            if (Chart == null)
                return;

            var oxyColor = OxyColor.FromArgb(color.Color.A, color.Color.R, color.Color.G, color.Color.B);
            if (Chart is LineSeries)
                (Chart as LineSeries).Color = oxyColor;
            else if(Chart is FunctionSeries)
                (Chart as FunctionSeries).Color = oxyColor;
            else if (Chart is ScatterSeries)
                (Chart as ScatterSeries).MarkerStroke = oxyColor;
        }

        /// <summary>
        /// 获取图形线条宽度
        /// </summary>
        private double GetLineWidth()
        {
            if (Chart == null)
                return 0;

            if (Chart is LineSeries)
                return (Chart as LineSeries).StrokeThickness;
            else if (Chart is FunctionSeries)
                return (Chart as FunctionSeries).StrokeThickness;
            else if (Chart is ScatterSeries)
                return (Chart as ScatterSeries).MarkerStrokeThickness;

            return 0;
        }

        /// <summary>
        /// 设置图形线条宽度
        /// </summary>
        /// <param name="lineWidth"></param>
        private void SetLineWidth(double lineWidth)
        {
            if (Chart == null)
                return;

            if (Chart is LineSeries)
                (Chart as LineSeries).StrokeThickness = lineWidth;
            else if (Chart is FunctionSeries)
                (Chart as FunctionSeries).StrokeThickness = lineWidth;
            else if (Chart is ScatterSeries)
                (Chart as ScatterSeries).MarkerStrokeThickness = lineWidth;
        }

        /// <summary>
        /// 获取图形数据
        /// </summary>
        private List<DataPoint> GetDataPoints()
        {
            if (Chart == null)
                return null;

            if (Chart is LineSeries)
                return (Chart as LineSeries).Points;
            else if (Chart is FunctionSeries)
                return (Chart as FunctionSeries).Points;
            else if (Chart is ScatterSeries)
            {
                 return (Chart as ScatterSeries).Points.Select(p=>new DataPoint(p.X, p.Y)).ToList();
            }

            return null;
        }

        /// <summary>
        /// 重置图像数据
        /// </summary>
        /// <param name="xDatas"></param>
        /// <param name="yDatas"></param>
        public void ResetDataPoint(double[] xDatas, double[] yDatas)
        {
            if (Chart == null)
                return;

            if (Chart is LineSeries)
            {
                var pts = (Chart as LineSeries).Points;
                pts.Clear();
                for (int i = 0; i < xDatas.Length; i++)
                {
                    pts.Add(new DataPoint(xDatas[i], yDatas[i]));
                }
            }
            else if (Chart is FunctionSeries)
            {
                var pts = (Chart as FunctionSeries).Points;
                pts.Clear();
                for (int i = 0; i < xDatas.Length; i++)
                {
                    pts.Add(new DataPoint(xDatas[i], yDatas[i]));
                }
            }
            else if (Chart is ScatterSeries)
            {
                var pts = (Chart as ScatterSeries).Points;
                pts.Clear();
                for (int i = 0; i < xDatas.Length; i++)
                {
                    pts.Add(new ScatterPoint(xDatas[i], yDatas[i]));
                }
            }
        }

        /// <summary>
        /// 判断图像是否在矩形框中
        /// </summary>
        /// <param name="begin">矩形框起点</param>
        /// <param name="end">矩形框终点</param>
        /// <param name="dataPoints">图形数据</param>
        /// <param name="minX">数据最小X</param>
        /// <param name="minY">数据最小Y</param>
        /// <param name="maxX">数据最大X</param>
        /// <param name="maxY">数据最大Y</param>
        /// <returns>True=在矩形框中</returns>
        private bool SeriesInRectangle(DataPoint begin, DataPoint end, List<DataPoint> dataPoints, double minX, double minY, double maxX, double maxY)
        {
            if (begin.X > end.X)
            {
                double tempx = begin.X;
                begin.X = end.X;
                end.X = tempx;
            }
            if (begin.Y > end.Y)
            {
                double tempy = begin.Y;
                begin.Y = end.Y;
                end.Y = tempy;
            }

            Rect selrc = new Rect(begin.X, begin.Y, end.X - begin.X, end.Y - begin.Y);
            if(maxX == minX || maxY == minY)    //单个数据点，直接判断
            {
                int index = dataPoints.FindIndex(p => selrc.Contains(p.X, p.Y));
                if (index >= 0)
                {
                    return true;
                }
            }
            else
            {
                Rect allrc = new Rect(minX, minY, maxX - minY, maxY - minY);
                if (selrc.IntersectsWith(allrc))   //多个数据点，先判断是否有交集
                {
                    int index = dataPoints.FindIndex(p => selrc.Contains(p.X, p.Y));
                    if (index >= 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 折线图形是否选中
        /// </summary>
        /// <param name="screenBegin">矩形框屏幕坐标起点</param>
        /// <param name="screenEnd">矩形框屏幕坐标终点</param>
        /// <returns></returns>
        private bool LineSeriesSelected(ScreenPoint screenBegin, ScreenPoint screenEnd)
        {
            var linesr = Chart as LineSeries;
            if (linesr == null || linesr.Points == null || linesr.Points.Count == 0)
                return false;

            var begin = linesr.InverseTransform(screenBegin);
            var end = linesr.InverseTransform(screenEnd);

            return SeriesInRectangle(begin, end, linesr.Points, linesr.MinX, linesr.MinY, linesr.MaxX, linesr.MaxY);
        }

        /// <summary>
        /// 散点图形是否选中
        /// </summary>
        /// <param name="screenBegin">矩形框屏幕坐标起点</param>
        /// <param name="screenEnd">矩形框屏幕坐标终点</param>
        /// <returns></returns>
        private bool ScatterSeriesSelected(ScreenPoint screenBegin, ScreenPoint screenEnd)
        {
            var sr = Chart as ScatterSeries;
            if (sr == null || sr.Points == null || sr.Points.Count == 0)
                return false;

            var begin = sr.InverseTransform(screenBegin);
            var end = sr.InverseTransform(screenEnd);

            return SeriesInRectangle(begin, end, GetDataPoints(), sr.MinX, sr.MinY, sr.MaxX, sr.MaxY);
        }

        /// <summary>
        /// 本图形是否被选中
        /// </summary>
        /// <param name="x0">屏幕矩形框X0</param>
        /// <param name="y0">屏幕矩形框Y0</param>
        /// <param name="x1">屏幕矩形框X1</param>
        /// <param name="y1">屏幕矩形框Y1</param>
        /// <returns></returns>
        public bool InSelectionRectangle(double x0, double y0, double x1, double y1)
        {
            ScreenPoint screenBegin = new ScreenPoint(x0, y0);
            ScreenPoint screenEnd = new ScreenPoint(x1, y1);

            if (Chart is LineSeries)
            {
                return LineSeriesSelected(screenBegin, screenEnd);
            }
            else if (Chart is ScatterSeries)
            {
                return ScatterSeriesSelected(screenBegin, screenEnd);
            }
            else
                return false;
        }
    }
}
