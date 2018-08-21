using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Ai.Hong.Driver
{
    /// <summary>
    /// 仪器信息类
    /// </summary>
    public class DeviceInfo : INotifyPropertyChanged
    {
        #region notifyevent
        /// <summary>
        /// 属性变更消息
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性变更消息
        /// </summary>
        /// <param name="propertyName"></param>
        protected void DoPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// 仪器厂商
        /// </summary>
        [XmlAttribute]
        public string Factor { get; set; }

        /// <summary>
        /// 仪器种类
        /// </summary>
        [XmlAttribute]
        public EnumDeviceCategory Category { get; set; }

        /// <summary>
        /// 仪器型号
        /// </summary>
        [XmlAttribute]
        public EnumDeviceModel Model { get; set; }

        /// <summary>
        /// 仪器商标
        /// </summary>
        [XmlAttribute]
        public string Brand { get; set; }

        /// <summary>
        /// 仪器序列号
        /// </summary>
        [XmlAttribute]
        public string SerialNumber { get; set; }

        /// <summary>
        /// 设备采集到数据的消息
        /// </summary>
        [XmlIgnore]
        public Action<int> OnDataReceived { get; set; }

        /// <summary>
        /// 设备是否已经连接
        /// </summary>
        [XmlIgnore]
        public bool IsConnected { get; set; } = false;

        /// <summary>
        /// 是否模拟采集
        /// </summary>
        [XmlAttribute]
        public bool IsSimulate { get; set; } = false;

        /// <summary>
        /// Device name
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Device address to connect
        /// </summary>
        [XmlAttribute]
        public string Address { get; set; }

        /// <summary>
        /// Wave length of laser
        /// </summary>
        [XmlAttribute]
        public double LaserWavelength { get; set; }

        /// <summary>
        /// 设备附加信息
        /// </summary>
        [XmlAttribute]
        public string Tag { get; set; } = null;

        /// <summary>
        /// 所有部件及其所有属性的列表
        /// </summary>
        [XmlIgnore]
        public List<HardwarePropertyInfo> AllHardwarePropertyInfos = null;

        private EnumHardwareStatus _status = EnumHardwareStatus.NotFound;
        /// <summary>
        /// 设备状态
        /// </summary>
        [XmlIgnore]
        public EnumHardwareStatus Status { get { return _status; } set { value = _status; DoPropertyChange("Status"); } }
    }

}
