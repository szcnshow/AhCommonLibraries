using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ai.Hong.Driver
{

    /// <summary>
    /// 硬件属性信息
    /// </summary>
    public class HardwarePropertyInfo : BasePropertyInfo
    {
        #region properties
        /// <summary>
        /// 属性所属的类型
        /// </summary>
        [XmlIgnore]
        public EnumPropCategory PropCategory;

        /// <summary>
        /// 部件ID
        /// </summary>
        [XmlIgnore]
        public EnumHardware HardwareID;

        /// <summary>
        /// 属性ID
        /// </summary>
        [XmlIgnore]
        public EnumHardwareProperties PropertyID;

        /// <summary>
        /// 是否只读
        /// </summary>
        [XmlIgnore]
        public bool IsReadonly;

        /// <summary>
        /// 是否为列表项
        /// </summary>
        [XmlIgnore]
        public bool IsSelection;

        /// <summary>
        /// 属性最小值
        /// </summary>
        [XmlIgnore]
        public float MinValue;

        /// <summary>
        /// 属性最大值
        /// </summary>
        [XmlIgnore]
        public float MaxValue;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public HardwarePropertyInfo()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hardwareID">硬件ID</param>
        /// <param name="propID">属性ID</param>
        /// <param name="propCategory">属性所属类别</param>
        /// <param name="valueType">属性值类型</param>
        /// <param name="isReadonly">是否只读</param>
        /// <param name="isSelection">是否为列表项</param>
        /// <param name="inputable">是否可以输入</param>
        public HardwarePropertyInfo(EnumHardware hardwareID, EnumHardwareProperties propID, EnumPropCategory propCategory,
            Type valueType = null, bool isReadonly = true, bool isSelection = false, bool inputable = false)
        {
            this.HardwareID = hardwareID;
            this.PropertyID = propID;
            this.PropCategory = propCategory;
            this.ValueType = valueType ?? typeof(int);
            this.IsReadonly = isReadonly;
            this.IsSelection = isSelection;
            this.Inputable = inputable;
            this.InnerName = propID.ToString();
            this.EnglishName = propID.ToString();
            this.ChineseName = Common.Extenstion.EnumExtensions.GetEnumDescription(propID, Common.EnumLanguage.Chinese);

            int pos = 0;
            //获取注释里面'（'之前的描述，作为中文名称
            if ((pos = ChineseName.IndexOf("（")) > 0)
            {
                ChineseName = ChineseName.Substring(0, pos - 1);
            }
        }

        /// <summary>
        /// 构造函数(用于定义固定的属性)
        /// </summary>
        /// <param name="hardwareID">硬件ID</param>
        /// <param name="propID">属性ID</param>
        /// <param name="propCategory">属性所属类别</param>
        /// <param name="innerName">内部名称</param>
        /// <param name="chineseName">中文名称</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="valueType">值得类型</param>
        /// <param name="value">当前值</param>
        /// <param name="inputable">是否允许用户录入</param>
        public HardwarePropertyInfo(EnumHardware hardwareID, EnumHardwareProperties propID, EnumPropCategory propCategory,
            string innerName, string chineseName, string englishName, Type valueType, string value, bool inputable = false) :
            base(innerName, chineseName, englishName, valueType, value, true, inputable)
        {
            this.HardwareID = hardwareID;
            this.PropertyID = propID;
            this.PropCategory = propCategory;
        }

        /// <summary>
        /// Clone当前类的值(浅层复制选项列表）
        /// </summary>
        /// <returns></returns>
        public new HardwarePropertyInfo Clone()
        {
            HardwarePropertyInfo retData = this.MemberwiseClone() as HardwarePropertyInfo;
            return retData;
        }
    }
}
