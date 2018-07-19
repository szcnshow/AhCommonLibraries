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
    public class HardwareProperty
    {
        /// <summary>
        /// 硬件错误信息
        /// </summary>
        public EnumHardwareError ErrorCode;
        /// <summary>
        /// 设备种类
        /// </summary>
        public EnumDeviceCategory DeviceCategory { get; protected set; }
        /// <summary>
        /// 设备型号
        /// </summary>
        public EnumDeviceModel DeviceModel { get; protected set; }

        #region Hardware functions

        /// <summary>
        /// 硬件属性的值类型和是否可读列表(const)
        /// </summary>
        public List<HardwarePropertyInfo> PropertyInfoList { get; set; }

        /// <summary>
        /// 硬件所包含的属性列表
        /// </summary>
        public Dictionary<EnumHardware, List<EnumHardwareProperties>> AllHardwareProperties { get; protected set; }

        /// <summary>
        /// 初始化（必须最先调用）
        /// </summary>
        /// <param name="category">设备种类</param>
        /// <param name="model">设备型号</param>
        public virtual void Initialize(EnumDeviceCategory category, EnumDeviceModel model)
        {
            DeviceCategory = category;
            DeviceModel = model;
        }

        /// <summary>
        /// 获取仪器包含的所有部件
        /// </summary>
        /// <returns>设备部件列表</returns>
        public virtual List<EnumHardware> HardwareList()
        {
            return AllHardwareProperties.Keys.ToList();
        }

        /// <summary>
        /// 获取部件可用的属性列表
        /// </summary>
        /// <param name="hardwareID">硬件ID</param>
        /// <returns></returns>
        public virtual List<EnumHardwareProperties> HardwarePropertyList(EnumHardware hardwareID)
        {
            if (AllHardwareProperties.ContainsKey(hardwareID))
                return AllHardwareProperties[hardwareID];
            else
                return new List<EnumHardwareProperties>();
        }

        /// <summary>
        /// 设备连接后调用
        /// 获取所有的部件，部件的所有属性，以及属性的取值范围或选项列表
        /// </summary>
        /// <param name="category">设备类型FTNIR, FTIR...</param>
        /// <param name="model">设备型号Sephere, Fiber...</param>
        public void HardwareGetAllProperties()
        {
            //var device = DeviceCategoryProperties.FirstOrDefault(p => p.category == category && p.model == model);
            //Trace.Assert(device != null, "Device not found");

            //var deviceprops = device.properties;
            //if (deviceprops != null && deviceprops.Count > 0)
            //    return;

            //device.properties = new List<HardwarePropertyInfo>();
            //deviceprops = device.properties;

            ////通用属性，每个硬件都有
            //var commprop = new List<EnumHardwareProperties>() { EnumHardwareProperties.Status, EnumHardwareProperties.SerialNo,
            //                                                EnumHardwareProperties.Temperature, EnumHardwareProperties.ProduceDate};

            //foreach (var hardware in device.hardwares)
            //{
            //    //加入每个设备都有的通用属性,肯定是只读的
            //    foreach (var prop in commprop)
            //    {
            //        //获取预定义属性的基本信息
            //        var info = PropertyInfoList.FirstOrDefault(p => p.PropertyID == prop);
            //        Trace.Assert(info != null, "Property not found");

            //        //Clone属性，准备加入部件属性列表中
            //        var newinfo = info.Clone();
            //        newinfo.HardwareID = hardware.Key;
            //        deviceprops.Add(newinfo);
            //    }

            //    //查看设备的专用属性是否存在
            //    foreach (var prop in hardware.Value)
            //    {
            //        //获取预定义属性的基本信息
            //        var info = PropertyInfoList.FirstOrDefault(p => p.PropertyID == prop);
            //        Trace.Assert(info != null, "Property not found");

            //        //Clone属性，准备加入部件属性列表中
            //        var newinfo = info.Clone();
            //        newinfo.HardwareID = hardware.Key;

            //        //只读属性,本属性没有取值范围或者可选列表
            //        if (info.IsReadonly == false)   //读写属性，表示本属性有取值范围或者可选列表
            //        {
            //            //属性是整数,表示有列表选择
            //            if (info.ValueType == typeof(int))
            //            {
            //                var sels = InnerHardwarePropertySelections(category, model, hardware.Key, prop);
            //                Trace.Assert(sels != null, "Get Property Error");
            //                newinfo.Selections = sels;
            //            }
            //            //属性是小数,表示有取值范围
            //            else if (info.ValueType == typeof(float))
            //            {
            //                float min, max;
            //                Trace.Assert(InnerHardwareGetPropertyRange(category, model, hardware.Key, prop, out min, out max), "Get Property Error");
            //                newinfo.MinValue = min;
            //                newinfo.MaxValue = max;
            //            }
            //        }
            //        deviceprops.Add(newinfo);
            //    }
            //}
        }

        /// <summary>
        /// 获取设备属性的选择项列表
        /// </summary>
        /// <param name="category">设备类型FTNIR, FTIR...</param>
        /// <param name="model">设备型号Sephere, Fiber...</param>
        /// <param name="deviceID">设备ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <returns>Null=出现错误</returns>
        protected virtual  List<dynamic> InnerHardwarePropertySelections(EnumDeviceCategory category, EnumDeviceModel model, EnumHardware deviceID, EnumHardwareProperties propertyID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取浮点属性的取值范围
        /// </summary>
        /// <param name="category">设备类型FTNIR, FTIR...</param>
        /// <param name="model">设备型号Sephere, Fiber...</param>
        /// <param name="hardwareID">部件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        protected virtual  bool InnerHardwareGetPropertyRange(EnumDeviceCategory category, EnumDeviceModel model, EnumHardware hardwareID, EnumHardwareProperties propertyID, out float min, out float max)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 检查部件是否包含了指定属性，并获取本属性的类型，只读信息
        /// 使用deviceHasProperties检查PropertyID是否在DeviceID中
        /// 用propertyInfoList来获取PropertyID的类型，只读信息
        /// </summary>
        /// <param name="category">设备类型FTNIR, FTIR...</param>
        /// <param name="model">设备型号Sephere, Fiber...</param>
        /// <param name="hardwareID">部件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <param name="forRead">True=获取属性, False=设置属性</param>
        /// <returns></returns>
        public HardwarePropertyInfo HardwarePropertyDetail(EnumDeviceCategory category, EnumDeviceModel model, EnumHardware hardwareID, EnumHardwareProperties propertyID, bool forRead = true)
        {
            ////确保仪器类型正确
            //var device = DeviceCategoryProperties.FirstOrDefault(p => p.category == category && p.model == model);
            //Trace.Assert(device != null, "Device not found");

            ////刷新属性
            //HardwareGetAllProperties(category, model);

            //switch (category)
            //{
            //    case EnumDeviceCategory.FTNIR:
            //    case EnumDeviceCategory.FTIR:
            //        var info = device.properties.FirstOrDefault(p => p.HardwareID == hardwareID && p.PropertyID == propertyID);
            //        if (info.IsReadonly == true && forRead == false)
            //            return null;

            //        return info;
            //}
            return null;
        }

        #endregion

        #region hardware functions

        /// <summary>
        /// 获取部件的属性（需要修改）
        /// </summary>
        /// <param name="category">设备类型</param>
        /// <param name="model">设备型号</param>
        /// <param name="hardwareID">部件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <returns>状态的值, Int, float都转为string返回，由调用程序解读, NULL表示错误</returns>
        public virtual dynamic HardwareGetProperty(EnumDeviceCategory category, EnumDeviceModel model, EnumHardware hardwareID, EnumHardwareProperties propertyID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 设置部件的属性（需要修改）
        /// </summary>
        /// <param name="category">设备类型</param>
        /// <param name="model">设备型号</param>
        /// <param name="hardwareID">部件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <param name="propertyValue">属性的值</param>
        /// <returns>错误信息</returns>
        public virtual EnumHardwareError HardwareSetProperty(EnumDeviceCategory category, EnumDeviceModel model, EnumHardware hardwareID, EnumHardwareProperties propertyID, dynamic propertyValue)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
