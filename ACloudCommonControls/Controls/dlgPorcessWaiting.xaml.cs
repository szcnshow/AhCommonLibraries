using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading;
using System.Windows.Threading;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// 长时间操作等待窗口
    /// </summary>
    public partial class dlgPorcessWaiting : Window
    {
        /// <summary>
        /// 长时间操作函数
        /// </summary>
        /// <param name="message">操作提示</param>
        /// <param name="maxvalue">滚动条最大值</param>
        /// <param name="curvalue">滚动条当前值</param>
        /// <param name="cancel">是否取消操作</param>
        /// <param name="sucessed">操作是否成功</param>
        /// <param name="threadParameter">线程运行参数</param>
        public delegate void ProcessRoutineDelegate(ref string message, ref double maxvalue, ref double curvalue, ref bool cancel, ref bool sucessed, object threadParameter);
        private Thread processThread = null;

        /// <summary>
        /// 操作函数
        /// </summary>
        ProcessRoutineDelegate processRoutine = null;
        DispatcherTimer timer = null;

        /// <summary>
        /// 操作过程的提示
        /// </summary>
        string message = null;

        /// <summary>
        /// 滚动条最大值
        /// </summary>
        double maxvalue = 1;

        /// <summary>
        /// 滚动条当前值
        /// </summary>
        double curvalue = 0;

        /// <summary>
        /// 用户取消操作
        /// </summary>
        public bool userCanceled = false;

        bool sucessed = true;

        /// <summary>
        /// 现场参数
        /// </summary>
        object threadParameter = null;

        /// <summary>
        /// 刷新频率
        /// </summary>
        double refreshRate = 1.0;

        /// <summary>
        /// 操作等待窗口
        /// </summary>
        /// <param name="routine">操作调用</param>
        /// <param name="title">提示</param>
        /// <param name="canCancel">是否用户可以取消</param>
        /// <param name="IsIndeterminate">是否为连续模式</param>
        /// <param name="refreshRate">刷新频率</param>
        /// <param name="threadParameter">运行参数(线程自己解析)</param>
        public dlgPorcessWaiting(ProcessRoutineDelegate routine, string title, bool canCancel, bool IsIndeterminate=false, double refreshRate=1.0, object threadParameter = null)
        {
            InitializeComponent();
            this.Title = title;
            processRoutine = routine;
            this.threadParameter = threadParameter;
            processingBar.IsIndeterminate = IsIndeterminate;
            this.refreshRate = refreshRate;

            if (!canCancel)
                btnCancel.Visibility = System.Windows.Visibility.Collapsed;
            Closing += new System.ComponentModel.CancelEventHandler(dlgPorcessWaiting_Closing);
        }

        /// <summary>
        /// 如果还在操作，不能关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dlgPorcessWaiting_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (processThread != null && processThread.IsAlive)
                e.Cancel = false;
        }

        /// <summary>
        /// 刷新操作状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            if (processThread != null && processThread.IsAlive)
            {
                if (!processingBar.IsIndeterminate)
                {
                    promptTitle.Text = message;
                    processingBar.Maximum = maxvalue;
                    processingBar.Value = curvalue;
                }
            }
            else
            {
                timer.Stop();
                DialogResult = sucessed;
                this.Close();
            }
        }

        /// <summary>
        /// 窗口启动后立即开始工作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int ms = (int)(refreshRate * 1000);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, ms);
            timer.Tick += timer_Tick;
            timer.Start();

            ThreadStart threadDelegate = new ThreadStart(StartThread);
            processThread = new Thread(threadDelegate);
            processThread.Start();
        }

        /// <summary>
        /// 开始操作
        /// </summary>
        private void StartThread()
        {
            processRoutine(ref message, ref maxvalue, ref curvalue, ref userCanceled, ref sucessed, threadParameter);
        }

        /// <summary>
        /// 取消操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            userCanceled = true;
        }
    }
}
