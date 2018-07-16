using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Ai.Hong.Common;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// Enum类型的选择列表，根据设置的Language显示对应的文字
    /// </summary>
    public partial class EnumTextList :ComboBox
    {
        /// <summary>
        /// 构建函数
        /// </summary>
        public EnumTextList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化Combox
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="InitValue">选择的值</param>
        /// <param name="language">显示语言</param>
        public void InitList<T>(T InitValue, EnumLanguage language = EnumLanguage.Chinese) where T : struct, IConvertible
        {
            if (typeof(T).IsEnum == false)
                return;

            var itemDict = Ai.Hong.Common.Extenstion.EnumExtensions.EnumTypeToDescriptionList<T>(language);
            this.ItemsSource = itemDict;
            this.SelectedValue = InitValue;
            Items.Refresh();
        }

    }
}
