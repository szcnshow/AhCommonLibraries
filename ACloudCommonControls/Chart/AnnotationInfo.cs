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
using OxyPlot.Annotations;

namespace Ai.Hong.Charts
{
    /// <summary>
    /// 标注属性
    /// </summary>
    class AnnotationInfo
    {
        public EnumAnnotationType AnnType = EnumAnnotationType.Arrow;

        /// <summary>
        /// 标注图形（根据EnumAnnotationType对应具体的图形类型）
        /// </summary>
        public Annotation AnnChart;

        /// <summary>
        /// 光谱ID
        /// </summary>
        public Guid key;

        /// <summary>
        /// 标注名称
        /// </summary>
        public string AnnName;

        /// <summary>
        /// 标注的文字
        /// </summary>
        public string AnnText { get; set; }

        /// <summary>
        /// 图形的数据点
        /// </summary>
        public List<DataPoint> Points { get { return GetDataPoints(); } }

        /// <summary>
        /// 线条的颜色
        /// </summary>
        public SolidColorBrush LineColor { get { return GetBorderColor(); } set { SetBorderColor(value); } }

        /// <summary>
        /// 填充的颜色
        /// </summary>
        public SolidColorBrush FillColor { get { return GetFillColor(); } set { SetFillColor(value); } }

        /// <summary>
        /// 线条宽度
        /// </summary>
        public double LineWidth { get { return GetLineWidth(); } set { SetLineWidth(value); } }

        public AnnotationInfo()
        {
        }

        /// <summary>
        /// 创建PointAnnotation图像
        /// </summary>
        /// <param name="markText">标注文字</param>
        /// <param name="centerX">标注的中心坐标</param>
        /// <param name="centerY">标注的中心坐标</param>
        /// <param name="markerSize">标注的大小</param>
        /// <param name="borderColor">边框颜色</param>
        /// <param name="fillColor">填充颜色</param>
        /// <param name="lineWidth">线宽</param>
        /// <param name="markerType">标记形状</param>
        /// <returns></returns>
        public static PointAnnotation CreatePointAnnotation(string markText, double centerX, double centerY, double markerSize = 5.0, SolidColorBrush borderColor = null, SolidColorBrush fillColor = null, double lineWidth = 1.0, MarkerType markerType= MarkerType.Circle)
        {
            if (borderColor == null)
                borderColor = Brushes.Black;
            if (fillColor == null)
                fillColor = Brushes.Transparent;

            PointAnnotation annotation = new PointAnnotation()
            {
                X = centerX,
                Y = centerY,
                Size = markerSize,
                Stroke = ChartCommonMethod.ToOxyColor(borderColor),
                StrokeThickness = lineWidth,
                Fill = ChartCommonMethod.ToOxyColor(fillColor),
                Text = markText,
                Shape= markerType,
            };
            return annotation;
        }

        /// <summary>
        /// 创建RectangleAnnotation图像,NaN表示值由显示的高度或宽度决定
        /// </summary>
        /// <param name="markText">标注文字</param>
        /// <param name="minX">起始X</param>
        /// <param name="minY">起始Y</param>
        /// <param name="maxX">结束X</param>
        /// <param name="maxY">结束Y</param>
        /// <param name="markerSize">标注的大小</param>
        /// <param name="borderColor">边框颜色</param>
        /// <param name="fillColor">填充颜色</param>
        /// <param name="lineWidth">线宽</param>
        /// <returns></returns>
        public static RectangleAnnotation CreateRectangleAnnotation(string markText, double minX = double.NaN, double maxX = double.NaN, double minY = double.NaN, double maxY = double.NaN, double markerSize = 5.0, SolidColorBrush borderColor = null, SolidColorBrush fillColor = null, double lineWidth = 1.0)
        {
            if (borderColor == null)
                borderColor = Brushes.Black;
            if (fillColor == null)
                fillColor = Brushes.Transparent;

            RectangleAnnotation annotation = new RectangleAnnotation()
            {
                MinimumX = minX,
                MaximumX = maxX,
                MinimumY = minY,
                MaximumY = maxY,
                Stroke = ChartCommonMethod.ToOxyColor(borderColor),
                StrokeThickness = lineWidth,
                Fill = ChartCommonMethod.ToOxyColor(fillColor),
                Text = markText
            };

            return annotation;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="annID">图形ID</param>
        /// <param name="annotation">显示的标注</param>
        /// <param name="annName">图形名称</param>
        /// <param name="annType">图形类型</param>
        /// <param name="fontSize">图形字体大小</param>
        public AnnotationInfo(Guid annID, Annotation annotation, EnumAnnotationType annType, string annName, double fontSize=10.0)
        {
            this.key = annID;
            this.AnnChart = annotation;
            this.AnnType = annType;
            this.AnnName = annName;
            this.AnnChart.FontSize = fontSize;
        }

        /// <summary>
        /// 获取图形线条颜色
        /// </summary>
        /// <returns></returns>
        private SolidColorBrush GetBorderColor()
        {
            if (AnnChart is ShapeAnnotation shapeAnn)
                return ChartCommonMethod.ToSolidColor(shapeAnn.Stroke);
            else
                return Brushes.Black;
        }

        /// <summary>
        /// 获取图形线条颜色
        /// </summary>
        /// <param name="color">线条颜色</param>
        /// <returns></returns>
        private void SetBorderColor(SolidColorBrush color)
        {
            if (AnnChart is ShapeAnnotation shapeAnn)
                shapeAnn.Stroke = ChartCommonMethod.ToOxyColor(color);
        }

        /// <summary>
        /// 获取图形填充颜色
        /// </summary>
        /// <returns></returns>
        private SolidColorBrush GetFillColor()
        {
            if (AnnChart is ShapeAnnotation shapeAnn)
                return ChartCommonMethod.ToSolidColor(shapeAnn.Fill);
            else
                return Brushes.Transparent;
        }

        /// <summary>
        /// 获取图形填充颜色
        /// </summary>
        /// <param name="color">线条颜色</param>
        /// <returns></returns>
        private void SetFillColor(SolidColorBrush color)
        {
            if (AnnChart is ShapeAnnotation shapeAnn)
                shapeAnn.Fill = ChartCommonMethod.ToOxyColor(color);
        }

        /// <summary>
        /// 获取图形线条宽度
        /// </summary>
        private double GetLineWidth()
        {
            if (AnnChart is ShapeAnnotation shapeAnn)
                return shapeAnn.StrokeThickness;
            else
                return 0;
        }

        /// <summary>
        /// 设置图形线条宽度
        /// </summary>
        /// <param name="lineWidth"></param>
        private void SetLineWidth(double lineWidth)
        {
            if (AnnChart is ShapeAnnotation shapeAnn)
                shapeAnn.StrokeThickness = lineWidth;
        }

        /// <summary>
        /// 获取图形数据
        /// </summary>
        private List<DataPoint> GetDataPoints()
        {
            if (AnnChart is PointAnnotation ptAnn)
                return new List<DataPoint> { new DataPoint(ptAnn.X, ptAnn.Y) };
            else if (AnnChart is RectangleAnnotation rcAnn)
                return new List<DataPoint> { new DataPoint(rcAnn.MinimumX, rcAnn.MinimumY), new DataPoint(rcAnn.MaximumX, rcAnn.MaximumY) };
            else if (AnnChart is PolygonAnnotation polyAnn)
                return polyAnn.Points;
            else
                return new List<DataPoint>();
        }

        /// <summary>
        /// 重置图像数据
        /// </summary>
        /// <param name="xDatas"></param>
        /// <param name="yDatas"></param>
        public void SetDataPoint(double[] xDatas, double[] yDatas)
        {
            if (AnnChart is PointAnnotation ptAnn)
            {
                ptAnn.X = xDatas[0];
                ptAnn.Y = yDatas[0];
            }
            else if (AnnChart is RectangleAnnotation rcAnn)
            {
                rcAnn.MinimumX = xDatas[0];
                rcAnn.MinimumY = yDatas[0];
                rcAnn.MaximumX = xDatas[1];
                rcAnn.MaximumY = yDatas[1];
            }
            else if (AnnChart is PolygonAnnotation polyAnn)
            {
                polyAnn.Points.Clear();
                for (int i = 0; i < xDatas.Length; i++)
                    polyAnn.Points.Add(new DataPoint(xDatas[i], yDatas[i]));
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
        /// <param name="ptAnn">点标注</param>
        /// <param name="rect">矩形框</param>
        /// <returns></returns>
        private bool PointAnnotationSelected(PointAnnotation ptAnn, Rect rect)
        {
            return rect.Contains(ptAnn.X, ptAnn.Y);
        }

        /// <summary>
        /// 散点图形是否选中
        /// </summary>
        /// <param name="rcAnn">矩形标注</param>
        /// <param name="rect">矩形框屏幕坐标起点</param>
        /// <returns></returns>
        private bool RectangleAnnotationSelected(RectangleAnnotation rcAnn, Rect rect)
        {            
            Rect curRc = new Rect(rcAnn.MinimumX, rcAnn.MinimumY, rcAnn.MaximumX - rcAnn.MinimumX, rcAnn.MaximumY - rcAnn.MinimumY);
            return rect.IntersectsWith(curRc);
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
            if(AnnChart is ShapeAnnotation spAnn)
            {
                if (x0 > x1)
                {
                    double tempx = x0;
                    x0 = x1;
                    x1 = tempx;
                }
                if (y0 > y1)
                {
                    double tempy = y0;
                    y0 = y1;
                    y1 = tempy;
                }

                var startPt = spAnn.InverseTransform(new ScreenPoint(x0, y0));
                var endPt = spAnn.InverseTransform(new ScreenPoint(x1, y1));

                Rect rect = new Rect(startPt.X, startPt.Y, endPt.X-startPt.X, endPt.Y-startPt.Y);

                if (AnnChart is PointAnnotation ptAnn)
                    return PointAnnotationSelected(ptAnn, rect);
                else if (AnnChart is RectangleAnnotation rcAnn)
                    return RectangleAnnotationSelected(rcAnn, rect);
                else
                    return false;
            }
            else
                return false;
        }
    }
}
