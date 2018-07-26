using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Ai.Hong.Driver
{

    /// <summary>
    /// 信息的基础类
    /// </summary>
    public class BasePropertyInfo : INotifyPropertyChanged
    {
        #region notifyevent
        /// <summary>
        /// 属性变更消息
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// DoPropertyChange
        /// </summary>
        /// <param name="propertyName">propertyName</param>
        public void DoPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region properties
        /// <summary>
        /// 内部名称
        /// </summary>
        [XmlAttribute]
        public string InnerName { get; set; }

        private string _value;
        /// <summary>
        /// 属性的值
        /// </summary>
        [XmlAttribute]
        public string Value { get { return _value; } set { _value = value; DoPropertyChange("Value"); } }

        /// <summary>
        /// 属性值类型
        /// </summary>
        [XmlIgnore]
        public Type ValueType { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        [XmlIgnore]
        public string ChineseName { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        [XmlIgnore]
        public string EnglishName { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [XmlIgnore]
        public bool IsValid { get; set; }

        /// <summary>
        /// 选项列表
        /// </summary>
        [XmlIgnore]
        public Dictionary<dynamic, string> Selections { get; set; }

        /// <summary>
        /// 是否可以录入
        /// </summary>
        [XmlIgnore]
        public bool Inputable { get; set; }

        /// <summary>
        /// 是否为列表项
        /// </summary>
        [XmlIgnore]
        public bool IsSelection { get { return Selections != null; } }

        /// <summary>
        /// 属性最小值
        /// </summary>
        [XmlIgnore]
        public float MinValue { get; set; } = float.MinValue;

        /// <summary>
        /// 属性最大值
        /// </summary>
        [XmlIgnore]
        public float MaxValue { get; set; } = float.MaxValue;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public BasePropertyInfo()
        {

        }

        /// <summary>
        /// 构造函数(用于定义固定的属性)
        /// </summary>
        /// <param name="innerName">内部名称</param>
        /// <param name="chineseName">中文名称</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="valueType">值得类型</param>
        /// <param name="value">当前值</param>
        /// <param name="inputable">是否允许用户录入</param>
        /// <param name="selections">选项列表</param>
        public BasePropertyInfo(string innerName, string chineseName, string englishName, Type valueType, string value = null, bool inputable = false, Dictionary<dynamic, string> selections = null)
        {
            this.InnerName = innerName;
            this.ChineseName = chineseName;
            this.EnglishName = englishName;
            this.ValueType = valueType;
            this.Value = value;
            this.IsValid = true;
            this.Inputable = inputable;
            this.Selections = selections;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public BasePropertyInfo Clone()
        {
            return this.MemberwiseClone() as BasePropertyInfo;
        }

        /// <summary>
        /// 获取属性的显示名称
        /// </summary>
        /// <param name="language">使用的语言</param>
        /// <returns></returns>
        public string PropertyDispalayName(Common.EnumLanguage language)
        {
            return language == Common.EnumLanguage.Chinese ? ChineseName : EnglishName;
        }
    };
}
