using System;
using System.Windows;
using System.Windows.Controls;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// simpleInputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleInputWindow : Window
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prompty">提示</param>
        /// <param name="title">窗口标题</param>
        /// <param name="orgText">原有字符串</param>
        public SimpleInputWindow(string prompty, string orgText = null, string title = "输入窗口")
        {
            InitializeComponent();
            this.Title = title;
            txtPrompt.Text = prompty;
            txtInput.Text = orgText;
            this.Loaded += new RoutedEventHandler(SimpleInputWindow_Loaded);
        }

        void SimpleInputWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Ai.Hong.Common.CommonMethod.HideWindowSystemButton(this);
            txtInput.Focus();
        }

        /// <summary>
        /// 获取录入的字符串
        /// </summary>
        /// <returns></returns>
        public string InputedText()
        {
            return txtInput.Text.Trim();
        }

        private void btnOkCancel_OKClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
                return;

            DialogResult = true;
            this.Close();
        }

        private void btnOkCancel_CancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void txtInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
                btnOkCancel_OKClicked(null, null);
        }
    }
}
