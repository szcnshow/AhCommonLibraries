using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Ai.Hong.Common;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 仪器性能测试
    /// </summary>   
    public partial class InstrumentTestPanel : UserControl
    {
        /// <summary>
        /// 测试项目参数
        /// </summary>
        private PerformanceTestGroupInfo TestParameter = null;

        /// <summary>
        /// 扫描进度条
        /// </summary>
        private Controls.ClockScannerPanel scanProgress;

        /// <summary>
        /// 连接的仪器
        /// </summary>
        private FTDriver scanner = null;

        /// <summary>
        /// 使用语言
        /// </summary>
        private EnumLanguage language = EnumLanguage.Chinese;

        /// <summary>
        /// 取消测试
        /// </summary>
        private bool AbortTestMark = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public InstrumentTestPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// pqParas和oqParas只能选择一个
        /// </summary>
        /// <param name="scanner">测试仪器</param>
        /// <param name="testParameter">测试项目</param>
        /// <param name="language">系统语言</param>
        public void Init(FTDriver scanner, PerformanceTestGroupInfo testParameter, EnumLanguage language)
        {
            System.Diagnostics.Debug.Assert(scanner != null && testParameter != null);

            InitializeComponent();

            this.scanner = scanner;
            this.language = language;
            this.TestParameter = testParameter;

            InitTestItems();
        }

        /// <summary>
        /// 检查参考文件是否存在
        /// </summary>
        public bool CheckReferenceFiles(string FilePath)
        {
            foreach(var item in TestParameter.GetAllTestItems())
            {
                if (!string.IsNullOrWhiteSpace(item.ReferenceFile))
                {
                    string file = System.IO.Path.Combine(FilePath, item.ReferenceFile);
                    if (!System.IO.File.Exists(file))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 如果正在检测，不能关闭窗口
        /// </summary>
        public  bool CanClose()
        {
            return (testingThread != null && testingThread.IsAlive);
        }

        /// <summary>
        /// 开始测试
        /// </summary>
        /// <param name="scanProgress">扫描进度条</param>
        public void StartTest(Controls.ClockScannerPanel scanProgress)
        {
            this.scanProgress = scanProgress;
            this.scanProgress.Notify += ScanProgress_Notify;
            AbortTestMark = false;
            testingThread = new System.Threading.Thread(ThreadTesting);
            testingThread.Start();
        }

        /// <summary>
        /// 取消测试
        /// </summary>
        public void AbortTest()
        {
            AbortTestMark = true;
        }

        private void InitTestItems()
        {
            foreach (var group in TestParameter.TestGroups)
            {
                AddOneGroup(group);
            }
        }

        private void ThreadTesting()
        {
            foreach (var group in TestParameter.TestGroups)
            {
                currentTestItem = group.TestItems[0];
                ScanParameter para = currentTestItem.AcquireParameter;
                SetOneGroupState(group, 1);

                bool successed = false;

                if (AbortTestMark)
                    break;

                switch (group.InnerName)
                {
                    case PerformanceTestGroupInfo.PQLaserCalibrate:
                        successed = LaserWavelengthTest(group, para);
                        break;

                    case PerformanceTestGroupInfo.PQEnergyTest:
                        successed = EnergyTest(group, para);
                        break;

                    case PerformanceTestGroupInfo.PQWaveAccuracyTest:
                        successed = WavelengthAccuracyTest(group);
                        break;

                    case PerformanceTestGroupInfo.PQPhotometricTest:
                        successed = PhotometriAccuracyTest(group);
                        break;
                    case PerformanceTestGroupInfo.OQLaserCalibrate:
                        successed = OnlyBackgroundReferenceTest(group.TestItems[0]);
                        break;
                    case PerformanceTestGroupInfo.OQResolutionTest:
                        successed = OnlyBackgroundReferenceTest(group.TestItems[0]);
                        break;
                    case PerformanceTestGroupInfo.OQPhotometricTest:
                        successed = OQphotometricTest(group);
                        break;
                    case PerformanceTestGroupInfo.OQWaveAccuracyTest:
                        successed = OQWaveAccuracyTest(group);
                        break;
                    case PerformanceTestGroupInfo.OQScanRefereceData:
                        successed = false;
                        break;
                }
                SetOneGroupState(group, successed ? 2 : 3);
            }

            if (AbortTestMark)
                return;

            //完成检测
            Dispatcher.Invoke((Action)delegate ()
            {
                //scanProgress.Visibility = System.Windows.Visibility.Collapsed;
                //btnStartScan.Visibility = System.Windows.Visibility.Visible;

                //需要修改
                //CloudManager.WindowStyle.WindowStyle.EnableCommandButton(this, CloudManager.WindowStyle.WindowStyle.enumWindowButton.Close, true);

                ////临时保存到Config目录下
                //string xpsfile = PQTestingParas != null ? "PQ_" : "OQ_";
                //xpsfile += DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".xps";
                //xpsfile = System.IO.Path.Combine(Common.AppEnvironment.appConfigPath, xpsfile);

                //XPSReport report = new XPSReport(scanner, "LongLight.Resource.ReportTotal.xaml", "LongLight.Resource.ReportDetail.xaml");
                //report.CreateAndSaveXPSFile(xpsfile, allGroups, PQTestingParas != null ? "PQ测试" : "OQ测试");

                //System.Diagnostics.Process.Start(xpsfile);
            });
        }

        /// <summary>
        /// 添加测试类
        /// </summary>
        /// <param name="testGroup"></param>
        private void AddOneGroup(SelTestGroup testGroup)
        {
            AddItemToGrid(0, testGroup.DisplayName(language), testGroup);
            if (testGroup.TestItems.Count > 1)
            {
                foreach (var item in testGroup.TestItems)
                {
                    AddItemToGrid(1, item.DisplayName(language), item);
                }
            }
        }

        /// <summary>
        /// 在rootGrid中添加Image和Textblock
        /// </summary>
        /// <param name="beginCol">起始列</param>
        /// <param name="displayName">显示名称</param>
        /// <param name="item">当前项</param>
        private void AddItemToGrid(int beginCol, string displayName, object item)
        {
            RowDefinition row = new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) };
            rootGrid.RowDefinitions.Add(row);

            Ellipse ellipse = new Ellipse()
            {
                Width = 12,
                Height = 12,
                Fill = Brushes.Gray,
                Margin = new Thickness(4.0),
                Tag = item
            };
            Grid.SetRow(ellipse, rootGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(ellipse, beginCol);
            rootGrid.Children.Add(ellipse);

            TextBlock txt = new TextBlock()
            {
                Text = displayName,
                Margin = new Thickness(4.0),
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Tag = item
            };
            Grid.SetRow(txt, rootGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(txt, beginCol + 1);

            //跨两列
            if (beginCol == 0)
                Grid.SetColumnSpan(txt, 2);

            rootGrid.Children.Add(txt);
        }

        /// <summary>
        /// 设置测试组合的状态
        /// </summary>
        /// <param name="group">测试组合</param>
        /// <param name="state">状态</param>
        private void SetOneGroupState(SelTestGroup group, int state)
        {
            Dispatcher.Invoke((Action)delegate()
            {
                SetOneItemState(group, state); ;
                foreach (var item in group.TestItems)
                    SetOneItemState(item, state);
            });
        }

        /// <summary>
        /// 设置检测项目的状态
        /// </summary>
        /// <param name="target"></param>
        /// <param name="state"></param>
        private void SetOneItemState(object target, int state)
        {
            Ai.Hong.Controls.VectorImage img = null;
            TextBlock txt = null;

            foreach(var item in rootGrid.Children)
            {
                if (item is Ai.Hong.Controls.VectorImage && (item as Ai.Hong.Controls.VectorImage).Tag == target)
                    img = item as Ai.Hong.Controls.VectorImage;
                else if (item is TextBlock && (item as TextBlock).Tag == target)
                    txt = item as TextBlock;
            }

            if (img == null || txt == null)
                return;

            if(state == 1)  //正在处理
            {
                img.DrawColor = Brushes.Gold;
                txt.Foreground = Brushes.Gold;
            }
            else if(state == 2) //成功完成
            {
                img.DrawColor = Brushes.GreenYellow;
                txt.Foreground = Brushes.GreenYellow;
            }
            else if(state == 3)     //失败完成
            {
                img.DrawColor = Brushes.Red;
                txt.Foreground = Brushes.Red;
            }
        }

        #region Testing Thread

        /// <summary>
        /// 检测线程
        /// </summary>
        System.Threading.Thread testingThread = null;

        /// <summary>
        /// 当前检测项
        /// </summary>
        BaseSelfTestInfo currentTestItem = null;

        /// <summary>
        /// 当前检测状态
        /// </summary>
        EnumScanNotifyState scanState = EnumScanNotifyState.Idel;

        /// <summary>
        /// 响应扫描消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ScanProgress_Notify(object sender, RoutedEventArgs e)
        {
            ScanNotifyArgs args = e as ScanNotifyArgs;
            if (args == null)
                return;

            scanState = args.State;
        }

        /// <summary>
        /// 一次扫描
        /// </summary>
        private bool ScanAndWaiting(FTDriver scanner, ScanParameter parameter)
        {
            //扫描背景
            scanState = scanProgress.StartScan(scanner, parameter);
            while (scanState == EnumScanNotifyState.Scanning)
            {
                System.Threading.Thread.Sleep(100);
            }
            return scanState == EnumScanNotifyState.OneFinished || scanState == EnumScanNotifyState.RepeateFinished;
        }

        /// <summary>
        /// 测试并校准激光波数
        /// </summary>
        /// <param name="group"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool LaserWavelengthTest(SelTestGroup group, ScanParameter parameter)
        {
            var realItem = group.TestItems[0] as LaserWavelengthTestInfo;
            int index;

            //根据重复次数来测试
            for (index = 0; index < realItem.AcquireParameter.RepeatCount; index++)
            {
                //扫描背景
                if (ScanAndWaiting(scanner, parameter) == false)
                    break;

                //背景单通道
                realItem.SpectraDatas.Clear();
                realItem.SpectraDatas.Add(scanProgress.BackSingleBeam);

                if (realItem.CalculateResult())
                {
                    scanProgress.UserAbort = true;
                    break;
                }
            }

            return index < realItem.AcquireParameter.RepeatCount;
        }

        /// <summary>
        /// 能量测试组合
        /// </summary>
        /// <param name="group">测试组合</param>
        /// <param name="parameter">测试参数</param>
        /// <returns></returns>
        private bool EnergyTest(SelTestGroup group, ScanParameter parameter)
        {
            var realItem = group.TestItems[0] as LineNoiseTestInfo;

            for (int i = 0; i < currentTestItem.AcquireParameter.RepeatCount; i++)
            {
                //扫描背景单通道
                parameter.ResultSpectrum = EnumResultSpectrum.BackSingleBeam;
                if (ScanAndWaiting(scanner, parameter) == false)
                    return false;

                var backData = scanProgress.BackSingleBeam;

                //扫描样品单通道
                parameter.BackgroundSpectrum = backData;
                parameter.BackgroundTime = DateTime.Now;
                parameter.ResultSpectrum = EnumResultSpectrum.Transmittance;
                if (ScanAndWaiting(scanner, parameter) == false)
                    break;

                //获取透射谱
                var transdata = scanProgress.Transmission;

                //100%线噪声测试使用透射谱
                realItem.SpectraDatas.Add(transdata);

                //斜率测试使用透射谱
                var item = group.TestItems.FirstOrDefault(p => p is DeviationTestInfo);
                if (item != null)
                    item.SpectraDatas.Add(transdata);

                //稳定性测试使用背景干涉图
                item = group.TestItems.FirstOrDefault(p => p is InterferPeakTestInfo);
                if (item != null)
                    item.SpectraDatas.Add(scanProgress.SampleInterferogram);

                //能量测试使用背景单通道图
                item = group.TestItems.FirstOrDefault(p => p is EnergyTestInfo);
                if (item != null)
                    item.SpectraDatas.Add(parameter.BackgroundSpectrum);
            }

            //计算结果
            bool result = true;
            foreach (var item in group.TestItems)
            {
                if (item.CalculateResult() == false)
                    result = false;
            }

            return result;
        }

        /// <summary>
        /// 波数精度测试
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private bool WavelengthAccuracyTest(SelTestGroup group)
        {
            //只扫描一次背景单通道光谱
            bool retvalue = true;
            foreach(var item in group.TestItems)
            {
                var parameter = item.AcquireParameter;
                //扫描背景单通道
                if (ScanAndWaiting(scanner, parameter) == false)
                    return false;

                item.SpectraDatas.Add(scanProgress.BackSingleBeam);
                retvalue = retvalue ==false ? retvalue : item.CalculateResult();
            }

            return retvalue;
        }

        /// <summary>
        /// 光度精度测试
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private bool PhotometriAccuracyTest(SelTestGroup group)
        {
            //只扫描一次背景单通道光谱
            bool retvalue = true;
            foreach (var item in group.TestItems)
            {
                var parameter = item.AcquireParameter;

                //扫描背景单通道
                if (ScanAndWaiting(scanner, parameter) == false)
                    return false;
                var backBeam = scanProgress.BackSingleBeam;

                //移动IVU位置到Class
                parameter.IVUFilter = EnumDeviceIVU.Glass;
                parameter.BackgroundSpectrum = backBeam;
                parameter.BackgroundTime = DateTime.Now;
                //扫描背景单通道
                if (ScanAndWaiting(scanner, parameter) == false)
                    return false;

                item.SpectraDatas.Add(scanProgress.Transmission);

                return retvalue == false ? retvalue : item.CalculateResult();
            }

            return retvalue;
        }

        /// <summary>
        /// 仅仅测试背景参考光谱并计算
        /// </summary>
        /// <param name="testItem"></param>
        /// <returns></returns>
        private bool OnlyBackgroundReferenceTest(BaseSelfTestInfo testItem)
        {
            int index;

            ScanParameter parameter = testItem.AcquireParameter;

            for (index = 0; index < testItem.AcquireParameter.RepeatCount; index++)
            {
                //扫描背景
                if (ScanAndWaiting(scanner, parameter) == false)
                    break;

                //背景单通道和背景干涉图

                var fileDatas = scanProgress.GetResult();
                var backdata = fileDatas.First(p => p.dataInfo.dataType == Ai.Hong.FileFormat.FileFormat.YAXISTYPE.YSCRF);

                testItem.SpectraDatas.Clear();
                testItem.SpectraDatas.Add(backdata);

                if (testItem.CalculateResult())
                    break;
            }

            return index < testItem.AcquireParameter.RepeatCount;
        }

        /// <summary>
        /// 使用透射图的测试
        /// </summary>
        /// <param name="testItem"></param>
        /// <param name="onlyOneBackground"></param>
        /// <returns></returns>
        private bool TransmitSpectrumTest(BaseSelfTestInfo testItem, bool onlyOneBackground)
        {
            var parameter = testItem.AcquireParameter;
            testItem.SpectraDatas.Clear();
            testItem.results.Clear();

            Ai.Hong.FileFormat.FileFormat backBeamData = null;
            for(int i=0; i<testItem.AcquireParameter.RepeatCount; i++)
            {
                if(!onlyOneBackground || i == 0)    //需要扫描背景
                {
                    //扫描背景单通道
                    if (ScanAndWaiting(scanner, parameter) == false)
                        return false;
                    var scanedBackDatas = scanProgress.GetResult();
                    backBeamData = scanedBackDatas.FirstOrDefault(p => p.dataInfo.dataType == Ai.Hong.FileFormat.FileFormat.YAXISTYPE.YSCRF);
                    //testItem.SpectraDatas.Add(backBeamData);
                }

                //将背景单通道作为样品单通道,扫描背景单通道
                if (ScanAndWaiting(scanner, parameter) == false)
                    return false;
                var scanedSampleDatas = scanProgress.GetResult();
                var sampleBeam = scanedSampleDatas.FirstOrDefault(p => p.dataInfo.dataType == Ai.Hong.FileFormat.FileFormat.YAXISTYPE.YSCRF);
                sampleBeam.dataInfo.dataType = Ai.Hong.FileFormat.FileFormat.YAXISTYPE.YSCSM;

                //需要改正 testItem.SpectraDatas.Add(BaseSelfTestInfo.CalculateTransimit(backBeamData, sampleBeam));
            }

            return testItem.CalculateResult();
        }

        /// <summary>
        /// OQ光路稳定性测试
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private bool OQphotometricTest(SelTestGroup group)
        {
            //只扫描一次背景单通道光谱
            List<bool> allret = new List<bool>();
            bool successed = true;
            foreach (var item in group.TestItems)
            {
                if(item is LineNoiseTestInfo)
                {
                    successed = TransmitSpectrumTest(item, false);
                }
                else if (item is LineSlopeTestInfo)
                {
                    successed = TransmitSpectrumTest(item, true);
                }
                else if (item is EnergyDistributeTestInfo)
                {
                    successed = OnlyBackgroundReferenceTest(item);
                }
                else if (item is TransmitReproductTestInfo)
                {
                    successed = TransmitSpectrumTest(item, true);
                }

                allret.Add(successed);
            }

            return allret.FindAll(p => p == false).Count == 0;
        }

        /// <summary>
        /// OQ波数精度测试
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private bool OQWaveAccuracyTest(SelTestGroup group)
        {
            //只扫描一次背景单通道光谱
            List<bool> allret = new List<bool>();
            bool successed = true;
            foreach (var item in group.TestItems)
            {
                if (item is VaporAccuracyTestInfo || 
                    item is PolyAccuracyTestInfo ||
                    item is WavenumberReproductTestInfo)
                {
                    successed = OnlyBackgroundReferenceTest(item);
                }

                allret.Add(successed);
            }

            return allret.FindAll(p => p == false).Count == 0;
        }


        #endregion
    }
}
