using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Ai.Hong.Driver
{
    /// <summary>
    /// 设备类型和型号对应的信息
    /// </summary>
    public class DeviceCategoryProperties
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public EnumDeviceCategory category;

        /// <summary>
        /// 设备型号
        /// </summary>
        public EnumDeviceModel model;

        /// <summary>
        /// 设备列表及其包含的属性信息
        /// </summary>
        public Dictionary<EnumHardware, List<EnumHardwareProperties>> hardwares;

        /// <summary>
        /// 设备属性列表
        /// </summary>
        public List<HardwarePropertyInfo> properties;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="category">设备类型</param>
        /// <param name="model">设备型号</param>
        /// <param name="hardwares">设备列表及其包含的属性信息</param>
        public DeviceCategoryProperties(EnumDeviceCategory category, EnumDeviceModel model, Dictionary<EnumHardware, List<EnumHardwareProperties>> hardwares)
        {
            this.category = category;
            this.model = model;
            this.hardwares = hardwares;
        }
    }

    /// <summary>
    /// 硬件属性操作
    /// </summary>
    public class DeviceHardware
    {
        #region properties
        /// <summary>
        /// 硬件错误信息
        /// </summary>
        public EnumHardwareError ErrorCode;
        /// <summary>
        /// 设备种类
        /// </summary>
        public EnumDeviceCategory DeviceCategory { get; protected set; } = EnumDeviceCategory.Unknown;
        /// <summary>
        /// 设备型号
        /// </summary>
        public EnumDeviceModel DeviceModel { get; protected set; } = EnumDeviceModel.Unknown;

        /// <summary>
        /// 设备使用的语言
        /// </summary>
        public Common.EnumLanguage Language = Common.EnumLanguage.Chinese;

        #endregion

        #region Hardware functions

        /// <summary>
        /// 硬件属性的值类型和是否可读列表
        /// </summary>
        public List<HardwarePropertyInfo> PropertyInfos { get; set; }

        /// <summary>
        /// 初始化（必须最先调用）
        /// </summary>
        /// <param name="category">设备种类</param>
        /// <param name="model">设备型号</param>
        public virtual void Initialize(EnumDeviceCategory category, EnumDeviceModel model)
        {
            DeviceCategory = category;
            DeviceModel = model;
            PropertyInfos = GetHardwarePropertyInfos();
        }

        /// <summary>
        /// 获取本设备的所有硬件属性的详细信息
        /// </summary>
        /// <returns></returns>
        protected virtual List<HardwarePropertyInfo> GetHardwarePropertyInfos()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取仪器包含的所有硬件
        /// </summary>
        /// <returns>设备硬件列表</returns>
        public List<EnumHardware> HardwareList()
        {
            return PropertyInfos.Select(p => p.HardwareID).Distinct().ToList();
        }

        /// <summary>
        /// 获取部件可用的属性列表
        /// </summary>
        /// <param name="hardwareID">硬件ID</param>
        /// <returns></returns>
        public List<EnumHardwareProperties> HardwarePropertyList(EnumHardware hardwareID)
        {
            return PropertyInfos.Where(p => p.HardwareID == hardwareID).Select(q => q.PropertyID).ToList();
        }

        /// <summary>
        /// 从当前设备属性中初始化初始化反序列化后的属性，并将反序列化的属性值填入找到的属性
        /// </summary>
        /// <param name="properties">反序列化后的属性列表</param>
        public List<HardwarePropertyInfo> InitDeserializedProperties(List<HardwarePropertyInfo> properties)
        {
            List<HardwarePropertyInfo> retDatas = new List<HardwarePropertyInfo>();

            //在属性列表中查找反序列化的属性
            foreach(var item in properties)
            {
                //在当前设备中查找本属性
                var curProp = PropertyInfos.FirstOrDefault(p => p.InnerName == item.InnerName);
                if (curProp == null)
                    continue;

                var newProp = curProp.Clone();
                newProp.Value = item.Value;

                retDatas.Add(newProp);
            }

            //查找并加入反序列化中没有包含的属性
            retDatas.AddRange(PropertyInfos.Where(p => properties.FirstOrDefault(q => q.InnerName == p.InnerName) == null));

            return retDatas;
        }

        /// <summary>
        /// 获取指定硬件指定属性的信息
        /// </summary>
        /// <param name="hardwareID">硬件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <returns>属性信息</returns>
        public HardwarePropertyInfo GetHardwarePropertyInfo(EnumHardware hardwareID, EnumHardwareProperties propertyID)
        {
            if (PropertyInfos == null)
                return null;
            return PropertyInfos.FirstOrDefault(p => p.HardwareID == hardwareID && p.PropertyID == propertyID);
        }

        /// <summary>
        /// 获取部件的属性
        /// </summary>
        /// <param name="lowLayerDevice">当前连接的设备</param>
        /// <param name="hardwareID">部件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <returns>状态的值, Int, float都转为string返回，由调用程序解读, NULL表示错误</returns>
        public virtual dynamic HardwareGetProperty(dynamic lowLayerDevice, EnumHardware hardwareID, EnumHardwareProperties propertyID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 设置部件的属性
        /// </summary>
        /// <param name="lowLayerDevice">当前连接的底层设备</param>
        /// <param name="hardwareID">部件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <param name="propertyValue">属性的值（继承类解析）</param>
        /// <returns>错误信息</returns>
        public virtual EnumHardwareError HardwareSetProperty(dynamic lowLayerDevice, EnumHardware hardwareID, EnumHardwareProperties propertyID, dynamic propertyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据当前硬件初始化扫描参数
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void InitScanParameter(ScanParameter parameter) { throw new NotImplementedException(); }

        #endregion
    }
}
