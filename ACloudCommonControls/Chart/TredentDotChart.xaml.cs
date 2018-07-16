using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms.DataVisualization.Charting;
using Ai.Hong.Controls;

namespace Ai.Hong.Charts
{
    /// <summary>
    /// 绘制带数据点的趋势图
    /// </summary>
    public partial class TredentDotChart : UserControl
    {
        /// <summary>
        /// 设置横坐标的时间格式, dd-HH:mm, yyyy-MM-dd等等
        /// </summary>
        public string XAxisFormatString
        {
            get { return graphicArea.AxisX.LabelStyle.Format; }
            set { graphicArea.AxisX.LabelStyle.Format = value; }
        }

        /// <summary>
        /// 所有的数据信息
        /// </summary>
        private class DataNodeInfo
        {
            public DateTime time;      //时间
            public float[] datas;     //值
            public object Tag;         //标识，用于删除等操作

            public DataNodeInfo(float[] datas, DateTime time, object tag)
            {
                this.datas = datas;
                this.time = time;
                this.Tag = tag;
            }
        }

        /// <summary>
        /// 当前图形信息
        /// </summary>
        private class ChartInfo
        {
            public string title;       //当前图形的名称
            public Series chart;       //图形
           
            public ChartInfo(string title)
            {
                this.title = title;
                chart = new Series(title);
                chart.MarkerStyle = MarkerStyle.Circle;
                chart.MarkerSize = 5;                
            }
        }

        List<DataNodeInfo> dataList = new List<DataNodeInfo>();        //所有数据
        List<ChartInfo> chartList = new List<ChartInfo>();                  //数据所代表的内容
        ChartArea graphicArea = new ChartArea();

        private string lableFormat = "dd-HH:mm";
        /// <summary>
        /// Constructor
        /// </summary>
        public TredentDotChart()
        {
            InitializeComponent();
            trendChart.ChartAreas.Add(graphicArea);
            trendChart.Visible = false;
            InitChart();

            trendChart.AntiAliasing = AntiAliasingStyles.Text;   //图形清晰显示，文字反锯齿
        }

        private void InitChart()
        {
            graphicArea.AxisX.LabelStyle.Format = lableFormat;
            //不显示网格线
            graphicArea.AxisX.MajorGrid.Enabled = false;
            graphicArea.AxisY.MajorGrid.Enabled = false;
        }

        private System.Drawing.Color ConvertColor(SolidColorBrush color)
        {
            return System.Drawing.Color.FromArgb(color.Color.A, color.Color.R, color.Color.G, color.Color.B);
        }

        /// <summary>
        /// 设置X轴显示格式
        /// </summary>
        public void SetXAxisValueFormat(string format)
        {
            lableFormat = format;
            foreach (DataNodeInfo info in dataList)
            {
                for (int i = 0; i < chartList.Count; i++)
                {
                    chartList[i].chart.Points[i].AxisLabel = info.time.ToString(lableFormat);
                }
            }            
        }

        /// <summary>
        /// 显示的图形列表，titles中不应该有重复项
        /// </summary>
        public void SetTitles(List<string> titles)
        {
            //清除原先的列表和图形
            listFiles.RemoveAllItems();
            chartList.Clear();
            dataList.Clear();
            trendChart.Series.Clear();
            trendChart.Visible = false;

            if (titles == null)
                return;

            foreach (string str in titles)
            {
                chartList.Add(new ChartInfo(str));
                Series sr = chartList[chartList.Count - 1].chart;
                sr.ChartType = SeriesChartType.Line;
                trendChart.Series.Add(sr);
            }

            List<object> tagList = new List<object>();
            tagList.AddRange(titles);
            listFiles.AddItem(titles, true, tagList, true);

            //将图形与列表项的颜色对应起来
            foreach (ChartInfo info in chartList)
            {
                CheckboxNameColorList.StringAndColorList dispItem = listFiles.FindListItem(info.title);
                if (dispItem != null)
                {
                    info.chart.Color = ConvertColor(dispItem.Color);
                }
            }
        }

        /// <summary>
        /// 判断是否具有相同的显示项目
        /// </summary>
        public bool SameTitles(List<string> fileList)
        {
            if (fileList == null || chartList.Count == 0 || fileList.Count != chartList.Count)
                return false;

            for (int i = 0; i < fileList.Count; i++)
            {
                if (chartList[i].title != fileList[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 新增一个数据节点
        /// </summary>
        /// <param name="datas">数据序列</param>
        /// <param name="time">产生数据序列的时间</param>
        /// <param name="tag">改数据序列的标识</param>
        public void AddData(float[] datas, DateTime time, object tag)
        {
            if (datas.Length != chartList.Count)        //数据的个数必须要和图形数量一致
                return;

            DataNodeInfo info = new DataNodeInfo(datas, time, tag);

            if (dataList.Count == 0 || dataList[dataList.Count-1].time < time)     //时间比前面的都要靠后，直接添加
            {
                dataList.Add(info);
                for (int i = 0; i < chartList.Count; i++)
                {
                    //chartList[i].chart.Points.AddXY(time, datas[i]);
                    chartList[i].chart.Points.AddXY(chartList[i].chart.Points.Count, datas[i]);
                    chartList[i].chart.Points[chartList[i].chart.Points.Count - 1].AxisLabel = info.time.ToString(lableFormat);
                }
                AdjustAxisValue();
            }
            else        //需要清除后重新排序,再显示
            {
                dataList.Add(info);
                ResetChartData();
            }
        }

        /// <summary>
        /// 设置X轴和Y轴的最大最小值
        /// </summary>
        private void AdjustAxisValue()
        {
            if (trendChart.Series.Count == 0 || trendChart.Series[0].Points.Count == 0)
                return;

            double maxY = double.MinValue;
            double minY = double.MaxValue;
            foreach (Series sr in trendChart.Series)
            {
                double tempmax = sr.Points.Max(item=>(double)item.YValues[0]);
                double tempmin = sr.Points.Min(item => (double)item.YValues[0]);

                maxY = maxY < tempmax ? tempmax : maxY;
                minY = minY > tempmin ? tempmin : minY;
            }
            if (maxY == 0)
                maxY = 0.1;
            if (minY == 0)
                minY = -0.1;
            maxY = maxY + Math.Pow(10, Math.Truncate(Math.Log10(Math.Abs(maxY)))) / 5;
            minY = minY-Math.Pow(10, Math.Truncate(Math.Log10(Math.Abs(minY)))) / 5;
            graphicArea.AxisY.Maximum = Math.Round(maxY, 1);
            graphicArea.AxisY.Minimum = Math.Round(minY, 1);

            //显示10个标签
            int interval = (int)Math.Round((float)trendChart.Series[0].Points.Count / 10.0f);
            if (interval < 1)
                interval = 1;
            graphicArea.AxisX.LabelStyle.Interval = interval;
        }

        /// <summary>
        /// 时间排序过程
        /// </summary>
        private static int CompareChartTime(DataNodeInfo prev, DataNodeInfo next)
        {
            if (prev.time < next.time)
                return -1;
            else if (prev.time == next.time)
                return 0;
            else
                return 1;
        }

        /// <summary>
        /// 有删除数据，需要重新生成图形
        /// </summary>
        private void ResetChartData()
        {
            //先清除所有图形数据
            foreach (ChartInfo info in chartList)
                info.chart.Points.Clear();

            if (dataList.Count > 0)
            {
                dataList.Sort(CompareChartTime);    //按时间排序
                
                //重新添加数据
                foreach (DataNodeInfo info in dataList)
                {
                    for (int i = 0; i < chartList.Count; i++)
                    {
                        //chartList[i].chart.Points.AddXY(info.time, info.datas[i]);
                        chartList[i].chart.Points.AddXY(chartList[i].chart.Points.Count, info.datas[i]);
                        chartList[i].chart.Points[chartList[i].chart.Points.Count - 1].AxisLabel = info.time.ToString(lableFormat);

                    }
                }
            }

            AdjustAxisValue();
        }

        /// <summary>
        /// 删除一个数据
        /// </summary>
        /// <param name="itemTag">序列标识</param>
        public void RemoveData(object itemTag)
        {
            DataNodeInfo info = dataList.Find(item => item.Tag == itemTag);
            if (info != null)
            {
                dataList.Remove(info);
                ResetChartData();
            }
        }

        /// <summary>
        /// 删除一批数据
        /// </summary>
        /// <param name="itemTags">序列标识列表</param>
        public void RemoveData(List<object> itemTags)
        {
            dataList.RemoveAll(item => itemTags.Exists(inItem => inItem == item));
            ResetChartData();
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        public void RemoveAllDatas()
        {
            //先清除所有图形数据
            foreach (ChartInfo info in chartList)
                info.chart.Points.Clear();

            dataList.Clear();
        }

        private void listFiles_ItemChecked(object sender, RoutedEventArgs e)
        {
            List<CheckboxNameColorList.StringAndColorList> selList = listFiles.GetSelectedFiles();
            if (selList.Count == 0)
            {
                trendChart.Series.Clear();
                trendChart.Visible = false;
                return;
            }

            foreach (ChartInfo info in chartList)
            {
                if (!selList.Exists(selitem => (string)selitem.Tag == info.title))      //去掉没有选择的图形
                    trendChart.Series.Remove(info.chart);
            }

            foreach(CheckboxNameColorList.StringAndColorList selitem in selList)         //增加新选择的图形
            {
                ChartInfo info = chartList.Find(chartitem=>chartitem.title == (string)selitem.Tag);     //在chartList中查找所选内容

                if (trendChart.Series.FindByName((string)info.title) == null)       //如果该图形没有增加，新增
                    trendChart.Series.Add(info.chart);
            }

            AdjustAxisValue();

            trendChart.Visible = true;
        }

        System.Drawing.Point prevPosition = new System.Drawing.Point(-1, -1);
        ToolTip tooltip = new ToolTip();
        /// <summary>
        /// 显示指定显示坐标的光谱数值
        /// </summary>
        private void trendChart_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var pos = e.Location;
            if (pos.X == prevPosition.X && pos.Y==prevPosition.Y)
                return;
            prevPosition = pos;

            txtDateTime.Text = null;
            txtValue.Text = null;

            var results = trendChart.HitTest(pos.X, pos.Y, false, ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    Series ser = result.Series;
                    int index = chartList.FindIndex(item=>item.chart == ser);
                    if (index >= 0)
                    {
                        txtDateTime.Text = dataList[result.PointIndex].time.ToString(XAxisFormatString);
                        txtValue.Text = dataList[result.PointIndex].datas[index].ToString();
                    }
                }
            }
            /*
            return;

            double selX, selY;
            
            //selX = trendChart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X);
            if (trendChart.ChartAreas[0].AxisY.Enabled == AxisEnabled.True)
                selY = trendChart.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y);
            else
                selY = trendChart.ChartAreas[0].AxisY2.PixelPositionToValue(e.Location.Y);

            //txtDateTime.Text = selX.ToString() ;
            txtValue.Text = selY.ToString();
             * */
        }
    }
}
