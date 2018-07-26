using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ai.Hong.Driver
{

    /// <summary>
    /// 录入信息字段属性
    /// </summary>
    public class SampleFieldInfo : BasePropertyInfo
    {
        private bool _forFilename;
        /// <summary>
        /// 是否作为文件名
        /// </summary>
        [XmlAttribute]
        public bool ForFilename { get { return _forFilename; } set { _forFilename = value; DoPropertyChange("forFilename"); } }

        /// <summary>
        /// 是否需要预定义属性
        /// </summary>
        [XmlAttribute]
        public bool HasPreDefines { get; set; }

        /// <summary>
        /// 预定义的值
        /// </summary>
        [XmlArray("preDefines")]
        [XmlArrayItem("preDefine")]
        public List<string> PreDefines { get; set; }

        private bool _isSelected = false;
        /// <summary>
        /// 是否选中
        /// </summary>
        [XmlAttribute]
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; DoPropertyChange("isSelected"); } }

        private bool _selectOnly = false;
        /// <summary>
        /// 是否只能选择
        /// </summary>
        [XmlAttribute]
        public bool SelectOnly { get { return _selectOnly; } set { _selectOnly = value; DoPropertyChange("selectOnly"); } }

        /// <summary>
        /// 系统自动创建的信息
        /// </summary>
        [XmlAttribute]
        public bool AutoGenerate { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SampleFieldInfo()
        {

        }

        /// <summary>
        /// 构造函数(用于定义固定的属性)
        /// </summary>
        /// <param name="innerName">内部名称</param>
        /// <param name="chineseName">中文名称</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="valueType">值得类型</param>
        /// <param name="hasPreDefines">是否可以预定义</param>
        /// <param name="isValid">是否选中</param>
        /// <param name="forFilename">是否作为文件名</param>
        /// <param name="autoGenerate">是否系统自动生成</param>
        public SampleFieldInfo(string innerName, string chineseName, string englishName, Type valueType, bool hasPreDefines = true, bool isValid = false, bool forFilename = false, bool autoGenerate = false) :
            base(innerName, chineseName, englishName, valueType, null, true)
        {
            this.ForFilename = forFilename;
            this.HasPreDefines = hasPreDefines;
            this.AutoGenerate = autoGenerate;
        }

        /// <summary>
        /// Clone this class
        /// </summary>
        /// <returns></returns>
        public new SampleFieldInfo Clone()
        {
            var retData = this.MemberwiseClone() as SampleFieldInfo;
            retData.PreDefines = new List<string>();
            if (this.PreDefines == null)
                this.PreDefines = new List<string>();

            retData.PreDefines.AddRange(this.PreDefines);
            return retData;
        }
    }
}
