using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace FTNirInterface.WaitDialog
{
    public partial class ProcessWaitDialog : Window
    {
        private const int GWL_STYLE = -16; private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayMsg">主要进行的任务名称</param>
        /// <param name="maxProcessValue">int.max</param>
        /// <param name="curProcessValue">int.max</param>
        /// <param name="status">任务详细</param>
        /// <param name="IsClose"></param>
        public delegate void SetProcessAndMsgDeletage(string displayMsg, int maxProcessValue, int curProcessValue, string status, bool IsClose);
        /// <summary>
        /// Process task
        /// </summary>
        /// <param name="callBack">process callback</param>
        /// <returns></returns>
        public delegate bool ProcessTask(SetProcessAndMsgDeletage callBack);

        ProcessTask curTask = null;
        Thread taskThread = null;
        bool IsClose = false;
        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="task"></param>
        public ProcessWaitDialog(ProcessTask task)
        {
            InitializeComponent();
            curTask = task;
        }

        /// <summary>
        /// 设置进度条信息
        /// </summary>
        // Microsoft.WindowsAPICodePack.Shell.Taskbar.Taskbar        
        public void SetProcessAndMsg(string displayMsg, int maxProcessValue, int curProcessValue, string status, bool IsClose)
        {
            var result = Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new SetMessageDelegate(SetMsg), displayMsg, maxProcessValue, curProcessValue, status, IsClose);
            //return (List<string>)result;
            // CoreHelpers.ThrowIfNotWin7();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayMsg"></param>
        /// <param name="maxProcessValue"></param>
        /// <param name="curProcessValue"></param>
        /// <param name="status"></param>
        /// <param name="IsClose"></param>
        public delegate void SetMessageDelegate(string displayMsg, int maxProcessValue, int curProcessValue, string status, bool IsClose);
        private void SetMsg(string displayMsg, int maxProcessValue, int curProcessValue, string status, bool IsClose)
        {
            ProcessingStatus.Text = status;
            //string[] te = status.Split('/');
            //string[] tt = te[1].Split(')');
            //string[] er = status.Split('(');
            //string[] re = er[1].Split('/');
            //tx.FontSize = 16;
            //if (int.Parse(re[0]) > 0)
            txtProcess.Text = displayMsg;
            if (maxProcessValue == int.MaxValue && curProcessValue == int.MaxValue)
            {
                barProcess.IsIndeterminate = true;
            }
            else
            {
                barProcess.Maximum = maxProcessValue;
                barProcess.Value = curProcessValue;
                tx.Text = (100 * curProcessValue / maxProcessValue).ToString("F2") + "%";
            }
            this.IsClose = IsClose;
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        private void RunTask()
        {
            taskSucessed = curTask(SetProcessAndMsg);
        }

        DispatcherTimer checkTimer = null;
        bool taskSucessed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dstWnd"></param>
        public static void HideWindowSystemButton(Window dstWnd)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(dstWnd).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HideWindowSystemButton(this);

            //启动扫描线程
            taskThread = new Thread(new ThreadStart(RunTask));
            taskThread.IsBackground = false;
            taskThread.Start();

            //检查扫描是否完成
            checkTimer = new DispatcherTimer();
            checkTimer.Interval = new TimeSpan(0, 0, 1);
            checkTimer.Tick += new EventHandler(checkTimer_Tick);
            checkTimer.Start();
        }

        /// <summary>
        /// 检查任务是否完成, 完成后自动关闭
        /// </summary>
        void checkTimer_Tick(object sender, EventArgs e)
        {
            if (IsClose)
            {
                try
                {
                    DialogResult = taskSucessed;
                }
                catch { }
                checkTimer.Stop();
                taskThread.Abort();
                this.Close();
            }
            if (taskThread == null || taskThread.IsAlive == false)
            {
                try
                {
                    DialogResult = taskSucessed;
                }
                catch
                {

                }
                checkTimer.Stop();
                taskThread.Abort();
                //  this.Close();
            }
        }

        private void cancel_click(object sender, RoutedEventArgs e)
        {
            checkTimer.Stop();
            //    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new SetMessageDelegate(SetMsg), "", 0, 0, "");
            taskThread.Abort();
            try
            {
                DialogResult = taskSucessed;
            }
            catch { }
            this.Close();
        }
    }
}
