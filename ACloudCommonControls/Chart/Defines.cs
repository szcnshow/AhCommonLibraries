using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot;
using System.ComponentModel;

namespace Ai.Hong.Charts
{

    /// <summary>
    /// 图形类型
    /// </summary>
    public enum EnumChartType
    {
        /// <summary>
        /// AreaSeries
        /// </summary>
        AreaSeries,
        /// <summary>
        /// BarSeries
        /// </summary>
        BarSeries,
        /// <summary>
        /// BoxPlotSeries
        /// </summary>
        BoxPlotSeries,
        /// <summary>
        /// CandleStickSeries
        /// </summary>
        CandleStickSeries,
        /// <summary>
        /// ColumnSeries
        /// </summary>
        ColumnSeries,
        /// <summary>
        /// FunctionSeries
        /// </summary>
        FunctionSeries,
        /// <summary>
        /// HeatMapSeries
        /// </summary>
        HeatMapSeries,
        /// <summary>
        /// LineSeries
        /// </summary>
        LineSeries,
        /// <summary>
        /// PieSeries
        /// </summary>
        PieSeries,
        /// <summary>
        /// ScatterSeries
        /// </summary>
        ScatterSeries,
    }

    /// <summary>
    /// 散点图形状
    /// </summary>
    public enum EnumMarkerType
    {
        /// <summary>
        /// Circle
        /// </summary>
        [Description("圆形,Circle")]
        Circle = MarkerType.Circle,
        /// <summary>
        /// Cross
        /// </summary>
        [Description("十字,Cross")]
        Cross = MarkerType.Cross,
        /// <summary>
        /// Diamond
        /// </summary>
        [Description("菱形,Diamond")]
        Diamond = MarkerType.Diamond,
        /// <summary>
        /// Plus
        /// </summary>
        [Description("加号,Plus")]
        Plus = MarkerType.Plus,
        /// <summary>
        /// Square
        /// </summary>
        [Description("正方形,Square")]
        Square = MarkerType.Square,
        /// <summary>
        /// Star
        /// </summary>
        [Description("星形,Star")]
        Star = MarkerType.Star,
        /// <summary>
        /// Triangle
        /// </summary>
        [Description("三角形,Triangle")]
        Triangle = MarkerType.Triangle
    }

    /// <summary>
    /// 弹出菜单项
    /// </summary>
    public enum ChartMenuItems
    {
        /// <summary>
        /// 选择
        /// </summary>
        [Description("选择,Select")]
        Select,     //选择
        /// <summary>
        /// 缩放的主菜单
        /// </summary>
        [Description("缩放,Zoom")]
        Zoom,       //缩放的主菜单
        /// <summary>
        /// 放大
        /// </summary>
        [Description("放大,ZoomIn")]
        zoomIn,     //放大
        /// <summary>
        /// 缩小
        /// </summary>
        [Description("缩小,ZoomOut")]
        zoomOut,    //缩小
        /// <summary>
        /// 重置XY
        /// </summary>
        [Description("重置XY,ResetXY")]
        resetXY,    //重置XY
        /// <summary>
        /// 重置Y
        /// </summary>
        [Description("重置Y,ResetY")]
        resetY,     //重置Y
        /// <summary>
        /// 移动
        /// </summary>
        [Description("移动,Pan")]
        Pan,        //移动
        /// <summary>
        /// 显示的主菜单
        /// </summary>
        [Description("显示,Display")]
        Display,        //显示的主菜单
        /// <summary>
        /// 修改颜色
        /// </summary>
        [Description("修改颜色,Colors")]
        Colors,         //修改颜色
        /// <summary>
        /// 隐藏图形
        /// </summary>
        [Description("隐藏图形,Hide")]
        Hide,           //隐藏图形
        /// <summary>
        /// 显示网格
        /// </summary>
        [Description("显示网格,Show GridLine")]
        showGridLine,   //显示网格
        /// <summary>
        /// 显示信息
        /// </summary>
        [Description("显示信息,Show Information")]
        showInformation,    //显示信息
        /// <summary>
        /// 标志峰位的主菜单
        /// </summary>
        [Description("标注峰位,Peak Pick")]
        peakPick,       //标志峰位的主菜单
        /// <summary>
        /// 向上的峰位
        /// </summary>
        [Description("向上峰位,Up Peak")]
        upPeakPick,     //向上的峰位
        /// <summary>
        /// 向下的峰位
        /// </summary>
        [Description("向下峰位,Down Peak")]
        downPeakPick    //向下的峰位 
    }

    /// <summary>
    /// 图形标注的类型
    /// </summary>
    public enum EnumAnnotationType
    {
        /// <summary>
        /// Arrow
        /// </summary>
        Arrow,
        /// <summary>
        /// Ellipse
        /// </summary>
        Ellipse,
        /// <summary>
        /// Line
        /// </summary>
        Line,
        /// <summary>
        /// Point
        /// </summary>
        Point,
        /// <summary>
        /// Polygon
        /// </summary>
        Polygon,
        /// <summary>
        /// Polyline
        /// </summary>
        Polyline,
        /// <summary>
        /// Rectangle
        /// </summary>
        Rectangle,
        /// <summary>
        /// Text
        /// </summary>
        Text,
    }
}
