using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading;
using System.Windows.Threading;
using Ai.Hong.Controls.Common;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// 长时间操作等待窗口
    /// </summary>
    public partial class PorcessWaitWindow : Window
    {
        /// <summary>
        /// 长时间操作函数
        /// </summary>
        public delegate void ProcessRoutineDelegate(WaitProcessParameter parameter);
        private Thread processThread = null;

        /// <summary>
        /// 现场运行参数
        /// </summary>
        public WaitProcessParameter threadParameter = null;

        /// <summary>
        /// 操作函数
        /// </summary>
        ProcessRoutineDelegate processRoutine = null;
        DispatcherTimer timer = null;

        /// <summary>
        /// 操作等待窗口
        /// </summary>
        /// <param name="routine">操作调用</param>
        /// <param name="title">提示</param>
        /// <param name="canCancel">是否用户可以取消</param>
        /// <param name="IsIndeterminate">是否为连续模式</param>
        /// <param name="parameter">运行参数(线程自己解析)</param>
        public PorcessWaitWindow(ProcessRoutineDelegate routine, string title, bool canCancel, bool IsIndeterminate = false, object parameter = null)
        {
            InitializeComponent();
            promptTitle.Text = title;
            processRoutine = routine;
            this.threadParameter = new Common.WaitProcessParameter()
                {
                    threadParameter = parameter
                };
            processingBar.IsIndeterminate = IsIndeterminate;

            if (!canCancel)
                btnCancel.Visibility = System.Windows.Visibility.Collapsed;
            Closing += new System.ComponentModel.CancelEventHandler(dlgPorcessWaiting_Closing);

            rootGrid.DataContext = threadParameter;
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
                return;
            }
            else
            {
                timer.Stop();
                DialogResult = threadParameter.sucessed;
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
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
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
            processRoutine(threadParameter);
        }

        /// <summary>
        /// 取消操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            threadParameter.cancel = true;
        }

        /// <summary>
        /// 判断处理是否失败
        /// </summary>
        /// <returns></returns>
        public bool ProcessFailed()
        {
            if(threadParameter == null)
                return true;
            return !threadParameter.sucessed;
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public string GetErrorMessage()
        {
            if (threadParameter == null)
                return null;
            return threadParameter.ErrorMessage;
        }
    }
}
