using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Text.RegularExpressions;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// 浏览Text/CSV文件结构
    /// </summary>
    public partial class TxtDataBrowser : UserControl
    {
        /// <summary>
        /// Text/CSV decoded datas
        /// </summary>
        private class DecodedDataInfo
        {
            /// <summary>
            /// X Data
            /// </summary>
            public double XData { get; set; }
            /// <summary>
            /// Y Data
            /// </summary>
            public double[] YDatas { get; set; }
        }

        /// <summary>
        /// 分割符（Comma，Spcace，Tab）
        /// </summary>
        public string Seperator { get; set; } = "Comma";

        /// <summary>
        /// X轴单位（cm-1，nm，count）
        /// </summary>
        public string XUnit { get; set; } = "nm";

        /// <summary>
        /// Y轴单位（Intensity，Absorbance，Transmission）
        /// </summary>
        public string YUnit { get { return YAxisUnit?.Text; } set { if(YAxisUnit != null) YAxisUnit.Text = value; } }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorString = null;

        /// <summary>
        /// 解析到数据
        /// </summary>
        List<DecodedDataInfo> DecodedDatas = new List<DecodedDataInfo>();

        //按行读入的文本内容
        List<string> TextLines = new List<string>();

        /// <summary>
        /// 绑定的数据
        /// </summary>
        List<DecodedDataInfo> BindedDatas = new List<DecodedDataInfo>();

        /// <summary>
        /// 数字开始的行
        /// </summary>
        int FirstNumberRow = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public TxtDataBrowser()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置Text/CSV文件
        /// </summary>
        /// <param name="filename"></param>
        public bool SetTextFile(string filename)
        {
            if (filename == null || !System.IO.File.Exists(filename))
                return false;

            //把Text文件按照行读出来
            try
            {
                using (var rs = new System.IO.StreamReader(filename))
                {
                    var cur = rs.ReadLine();
                    while (!string.IsNullOrEmpty(cur))
                    {
                        TextLines.Add(cur);
                        cur = rs.ReadLine();
                    }
                    rs.Close();
                }


                string temptxt = null;
                for(int i=0; i<20 && i<TextLines.Count; i++)
                {
                    temptxt += TextLines[i] + "\r\n";
                }
                textDataViewer.Text = temptxt;
                DecodeTextData();
                BindToDatagrid();
                return true;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 分隔符号变化，需要重新解析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listSeparator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DecodeTextData();
        }

        private void numFirstRow_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            BindToDatagrid();
        }

        private void numFirstCol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            BindToDatagrid();
        }

        private void numYCols_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            BindToDatagrid();
        }

        /// <summary>
        /// 解析文件中的数据（加载文件或者改变分隔符号才解析）
        /// </summary>
        private void DecodeTextData()
        {
            if (TextLines == null || TextLines.Count == 0)
                return;

            DecodedDatas.Clear();

            string spChar = null;
            switch(listSeparator.SelectedIndex)
            {
                case 0:
                    spChar = "[,]+";
                    break;
                case 1:
                    spChar = "[\\s]+";
                    break;
                case 2:
                    spChar = "[\\t]+";
                    break;
            }

            FirstNumberRow = -1;
            for (int row=0; row < TextLines.Count; row++)
            {
                if (string.IsNullOrWhiteSpace(TextLines[row]))
                    continue;

                var cols = Regex.Split(TextLines[row], spChar);
                if (cols == null || cols.Length == 0)
                    continue;

                if (double.TryParse(cols[0], out double tempvalue) == false)
                    continue;


                DecodedDataInfo curDatas = new DecodedDataInfo()
                {
                    XData = tempvalue,
                    YDatas = new double[cols.Length - 1]
                };

                for(int col =1; col<cols.Length; col++)
                {
                    if (double.TryParse(cols[col], out tempvalue) == false)
                        curDatas.YDatas[col - 1] = double.NaN;
                    else
                        curDatas.YDatas[col - 1] = tempvalue;
                }

                //有数字的行开始
                if (FirstNumberRow == -1)
                    FirstNumberRow = row;

                DecodedDatas.Add(curDatas);
            }

            //判断有没有数据
            numYCols.IsEnabled = DecodedDatas.Count > 0;
            numFirstCol.IsEnabled = numYCols.IsEnabled;
            numFirstRow.IsEnabled = numYCols.IsEnabled;

            if (numYCols.IsEnabled == false)
                return;

            //最大的Y轴数据量
            numYCols.Value = (from p in DecodedDatas select p.YDatas.Length).Max();
            numYCols.Maximum = numYCols.Value;
            numYCols.Minimum = 1;
            numYCols.IsEnabled = numYCols.Maximum > 1;

            numFirstCol.Value = 0;
            numFirstCol.Maximum = numYCols.Maximum - 1;
            numFirstCol.IsEnabled = numFirstCol.Maximum > numFirstCol.Value;

            if ((int)numFirstRow.Value < FirstNumberRow)
                numFirstRow.Value = FirstNumberRow;
            numFirstRow.Maximum = TextLines.Count - 1;
            numFirstRow.Minimum = FirstNumberRow;
        }

        /// <summary>
        /// 绑定数据到Datagrid中，需要将数据设置为统一的列数
        /// </summary>
        private void BindToDatagrid()
        {
            if (textDataList == null)
                return;

            textDataList?.Columns.Clear();
            if (DecodedDatas == null || DecodedDatas.Count == 0)
            {
                textDataList.Columns.Clear();
                textDataList.ItemsSource = null;
                textDataList.Items.Refresh();
                return;
            }

            BindedDatas = new List<DecodedDataInfo>();
            for(int row = (int)numFirstRow.Value - FirstNumberRow ; row < DecodedDatas.Count; row++)
            {
                if (DecodedDatas[row].YDatas.Length < (int)numYCols.Value)  //X+ Y cols < 数据列数量
                {
                    var tempYDatas = new double[(int)numYCols.Value];
                    for(int i=0; i< tempYDatas.Length; i++)
                    {
                        tempYDatas[i] = i < DecodedDatas[row].YDatas.Length ? DecodedDatas[row].YDatas[i] : double.NaN;
                    }
                    DecodedDatas[row].YDatas = tempYDatas;
                }

                BindedDatas.Add(DecodedDatas[row]);
            }

            Binding bind = new Binding("XData");
            DataGridTextColumn col = new DataGridTextColumn();
            col.Binding = bind;
            col.Header = "X";
            textDataList.Columns.Add(col);
            for (int i=0; i<(int)numYCols.Value; i++)
            {
                bind = new Binding($"YDatas[{i}]");
                col = new DataGridTextColumn();
                col.Header = $"Y[{i}]";
                col.Binding = bind;
                textDataList.Columns.Add(col);
            }
            textDataList.ItemsSource = BindedDatas;
            textDataList.Items.Refresh();
        }

        /// <summary>
        /// 获取解码后的数据
        /// </summary>
        /// <returns></returns>
        public List<double[]> GetDecodedDatas()
        {
            if(BindedDatas == null || BindedDatas.Count == 0)
            {
                ErrorString = "No data decoded";
                return null;
            }

            //判断有没有NaN数据
            if(BindedDatas.FirstOrDefault(p=>double.IsNaN(p.XData)|| p.YDatas.Select(q=>double.IsNaN(q)).Count()>0) != null)
            {
                ErrorString = "Has invalid data format";
                return null;
            }

            //把X，Y数据按列存放
            List<double[]> retDatas = new List<double[]>();
            retDatas.Add(BindedDatas.Select(p => p.XData).ToArray());
            for (int i = 0; i < BindedDatas[0].YDatas.Length; i++)
                retDatas.Add(BindedDatas.Select(p => p.YDatas[i]).ToArray());

            return retDatas;
        }
    }
}
