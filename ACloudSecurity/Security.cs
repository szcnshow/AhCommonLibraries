using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace ACloudSecurity
{
    /// <summary>
    /// 授权数据类型
    /// </summary>
    public enum enumDataType
    {
        ///// <summary>
        ///// 测试
        ///// </summary>
        //TESTENTRY = 1,
        ///// <summary>
        ///// 日期
        ///// </summary>
        //DATEENTRY = 2,
        /// <summary>
        /// 文件格式
        /// </summary>
        [Description("文件格式")]
        FILEFORMAT = 3,
        /// <summary>
        /// 算法
        /// </summary>
        [Description("算法")]
        ALGORITHM = 4,
        /// <summary>
        /// 近红外
        /// </summary>
        [Description("近红外")]
        NIR = 5,
        /// <summary>
        /// 中红外
        /// </summary>
        [Description("中红外")]
        IR = 6,
        /// <summary>
        /// 拉曼
        /// </summary>
        [Description("拉曼")]
        RAMAN = 7,
        /// <summary>
        /// 气相
        /// </summary>
        [Description("气相色谱")]
        GC = 8,
        /// <summary>
        /// 液相
        /// </summary>
        [Description("液相色谱")]
        LC = 9,
        /// <summary>
        /// 软件
        /// </summary>
        [Description("软件")]
        SOFTWARE = 10,
        /// <summary>
        /// 密码
        /// </summary>
        [Description("加密")]
        STRENTRY = 0xFF		//字符串类型
    };

    /// <summary>
    /// 支持的数据文件格式
    /// </summary>
    public enum enumFileFormat
    {
        /// <summary>
        /// SPC格式
        /// </summary>
        SPC = 1,
        /// <summary>
        /// OPUS格式
        /// </summary>
        OPUS = 2,
        /// <summary>
        /// SPA格式
        /// </summary>
        SPA = 3,
        /// <summary>
        /// DX格式
        /// </summary>
        DX = 4,
        /// <summary>
        /// SP格式
        /// </summary>
        SP = 5,
        /// <summary>
        /// MATLAB格式
        /// </summary>
        MAT = 6,
        /// <summary>
        /// CSV格式
        /// </summary>
        CSV = 7,
        /// <summary>
        /// EXCEL格式
        /// </summary>
        EXCEL = 8,
        /// <summary>
        /// PLS系数格式
        /// </summary>
        PLSCOEF = 9,
        /// <summary>
        /// 标准化系数格式
        /// </summary>
        STANDCOEF = 10,
    };

    /// <summary>
    /// 支持的仪器厂商
    /// </summary>
    public enum enumFactory
    {
        /// <summary>
        /// VSPEC仪器
        /// </summary>
        VSPEC = 1,
        /// <summary>
        /// BRUKER仪器
        /// </summary>
        BRUKER = 2,
        /// <summary>
        /// THERMO仪器
        /// </summary>
        THERMO = 3,
        /// <summary>
        /// PE仪器
        /// </summary>
        PE = 4,
        /// <summary>
        /// FOSS仪器
        /// </summary>
        FOSS = 5,
        /// <summary>
        /// ABB仪器
        /// </summary>
        ABB = 6,
        /// <summary>
        /// OCEAN仪器
        /// </summary>
        OCEAN = 7,
        /// <summary>
        /// PERTEN仪器
        /// </summary>
        PERTEN = 8,      //波通
        /// <summary>
        /// METROHM仪器
        /// </summary>
        METROHM = 9,    //瑞士万通
        /// <summary>
        /// BWTEK仪器
        /// </summary>
        BWTEK = 10,
        /// <summary>
        /// ENWAVE仪器
        /// </summary>
        ENWAVE = 11,
        /// <summary>
        /// 聚光仪器
        /// </summary>
        FPI = 12,    //聚光
        /// <summary>
        /// 伟创英图仪器
        /// </summary>
        WCYT = 13,    //伟创英图
        /// <summary>
        /// 北分瑞利仪器
        /// </summary>
        BFRL = 14,    //北分瑞利
        /// <summary>
        /// NDC仪器
        /// </summary>
        NDC = 15
    };

    /// <summary>
    /// 算法列表
    /// </summary>
    public enum enumAlogrithm
    {
        /// <summary>
        /// ACloud PLS
        /// </summary>
        ACLOUD_PLS = 1,
        /// <summary>
        /// ACloud仪器标准化
        /// </summary>
        ACLOUD_STANDARD = 2,
        /// <summary>
        /// ACloud光谱预处理
        /// </summary>
        ACLOUD_PREPROCESS = 3,
        /// <summary>
        /// ACloud PCA
        /// </summary>
        ACLOUD_PCA = 4,
        /// <summary>
        /// VSPEC PLS
        /// </summary>
        PLS_VSPEC = 5,
        /// <summary>
        /// BRUKER PLS
        /// </summary>
        PLS_BRUKER = 6,
        /// <summary>
        /// THERMO PLS
        /// </summary>
        PLS_THERMO = 7,
        /// <summary>
        /// PE PLS
        /// </summary>
        PLS_PE = 8
        
    };

    /// <summary>
    /// 错误代码
    /// </summary>
    public enum enumErrorCode
    {
        /// <summary>
        /// 正确
        /// </summary>
        OK = 0,
        /// <summary>
        /// 参数错误
        /// </summary>
        Parameter = -1,
        /// <summary>
        /// 内存错误
        /// </summary>
        Memory = -2,
        /// <summary>
        /// 密码错误
        /// </summary>
        Key = -3,
        /// <summary>
        /// 文件格式错误
        /// </summary>
        Format = -4,
        /// <summary>
        /// 读取注册表错误
        /// </summary>
        RegData = -5,
        /// <summary>
        /// 硬件码错误
        /// </summary>
        Finger = -6,
        /// <summary>
        /// 授权超时
        /// </summary>
        OutDate = -7,
        /// <summary>
        /// 没有授权
        /// </summary>
        NotAuthor = -8
    };

    /// <summary>
    /// 授权软件列表
    /// </summary>
    public enum enumSoftware
    {
        /// <summary>
        /// ACloud服务器版本
        /// </summary>
        NIR_ACLOUD_SERVER = 1,
        /// <summary>
        /// ACloud本地版本
        /// </summary>
        NIR_ACLOUD_STANDALONE = 2,
        /// <summary>
        /// ACloud分布式版本
        /// </summary>
        NIR_ACLOUD_DISTRIBUT = 3,
        /// <summary>
        /// 复烤近红外在线
        /// </summary>
        NIR_REDRY_ONLINE = 3,
        /// <summary>
        /// 注射液快检
        /// </summary>
        RAMAN_RFDI = 11,
        /// <summary>
        /// 拉曼在线
        /// </summary>
        RAMAN_ONLINE = 12,
        /// <summary>
        /// 环保快检
        /// </summary>
        IR_FASTIDENT = 21,
        /// <summary>
        /// PLS建模软件
        /// </summary>
        MODEL_PLS = 31,
        /// <summary>
        /// IDENT建模软件
        /// </summary>
        MODEL_IDENT = 32,
        /// <summary>
        /// 仪器标准化软件
        /// </summary>
        INST_STANDARD=33,
        /// <summary>
        /// 文件转换软件
        /// </summary>
        FILE_CONVERTOR=34
        
    }

    /// <summary>
    /// 加密类型
    /// </summary>
    public enum enumEncryptKeyType
    {
        /// <summary>
        /// 不需要加密
        /// </summary>
        NO_ENCRYPT=0,
        /// <summary>
        /// 外部加密
        /// </summary>
        EXTERNAL_ENCRYPT = 1,
        /// <summary>
        /// NIR PLS Model1
        /// </summary>
        NIR_PLS_MODEL_1 = 2,
        /// <summary>
        /// NIR PLS Model2
        /// </summary>
        NIR_PLS_MODEL_2 = 3,
        /// <summary>
        /// NIR Calibrate Coefficient1
        /// </summary>
        NIR_CAL_COEF_1 = 11,
        /// <summary>
        /// NIR Calibrate Coefficient2
        /// </summary>
        NIR_CAL_COEF_2 = 12,
        /// <summary>
        /// RAMAN PLS Model1
        /// </summary>
        RAMAN_PLS_MODEL_1 = 21,
        /// <summary>
        /// RAMAN PLS Model2
        /// </summary>
        RAMAN_PLS_MODEL_2 = 22,
        /// <summary>
        /// RAMAN Identify Model1
        /// </summary>
        RAMAN_IDENT_MODEL_1 = 31,
        /// <summary>
        /// RAMAN Identify Model2
        /// </summary>
        RAMAN_IDENT_MODEL_2 = 32,
    }

    /// <summary>
    /// 授权加密接口
    /// </summary>
    public class Security
    {
        const int fingerIDLen=4;

        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = null;

 
        #region 32bitDLL

        /// <summary>
        /// 获取硬件码
        /// </summary>
        /// <param name="copysize">一个硬件码的长度</param>
        /// <param name="outDataSize">输出数据长度</param>
        /// <returns>三个硬件码的组合(XXXX-YYYY-ZZZZ)</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetDeviceFinger")]
        private static extern IntPtr GetDeviceFinger32(int copysize, ref int outDataSize);

        /// <summary>
        /// 是否为正确的硬件码（在DLL里面获取硬件码）
        /// </summary>
        /// <param name="fileData">密码文件</param>
        /// <param name="fileSize">密码文件长度</param>
        /// <returns></returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IsRightFinger")]
        private static extern int IsRightFinger32(byte[] fileData, int fileSize);

        /// <summary>
        /// 通过密码加密数据
        /// </summary>
        /// <param name="dataToEncrypt">明文数据</param>
        /// <param name="lenData">数据长度</param>
        /// <param name="encryptKey">密码</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>加密数据</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PublicEncryptData")]
        private static extern IntPtr PublicEncryptData32(byte[] dataToEncrypt, int lenData, string encryptKey, ref int outDataSize);

        /// <summary>
        /// 通过密码文件解码数据
        /// </summary>
        /// <param name="encryptoedData">密文数据</param>
        /// <param name="lenData">密文数据长度</param>
        /// <param name="keyData">密码</param>
        /// <param name="lenKeyData">密码长度</param>
        /// <param name="outDataSize">解码后的明文数据长度</param>
        /// <returns>明文数据</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PrivateDecryptData")]
        private static extern IntPtr PrivateDecryptData32(byte[] encryptoedData, int lenData, byte[] keyData, int lenKeyData, ref int outDataSize);

        /// <summary>
        /// 通过密码解码数据
        /// </summary>
        /// <param name="encryptoedData">密文数据</param>
        /// <param name="lenData">密文数据长度</param>
        /// <param name="decryptKey">密码</param>
        /// <param name="outDataSize">解码后的明文数据长度</param>
        /// <returns>明文数据</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PublicDecryptData")]
        private static extern IntPtr PublicDecryptData32(byte[] encryptoedData, int lenData, string decryptKey, ref int outDataSize);

        /// <summary>
        /// 将授权数据写入注册表
        /// </summary>
        /// <param name="writeData">授权数据</param>
        /// <param name="dataSize">数据长度</param>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WriteToRegister")]
        private static extern bool WriteToRegister32(byte[] writeData, UInt32 dataSize);

        /// <summary>
        /// 获取授权码中的预定义值
        /// </summary>
        /// <param name="mainType">主类型</param>
        /// <param name="subType">子类型</param>
        /// <param name="dataValue">预定义值</param>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseValue")]
        private static extern bool GetLicenseValue32(UInt32 mainType, UInt32 subType, ref UInt32 dataValue);

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns>enumErrorCode</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseErrorCode")]
        private static extern int GetLicenseErrorCode32();

        /// <summary>
        /// 使用授权数据内置密码解密数据
        /// </summary>
        /// <param name="dataToDecrypty">需要解码的数据</param>
        /// <param name="lenData">数据长度</param>
        /// <param name="keyType">密码类型</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>解码后的数据</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DecryptDataByLicenseKey")]
        private static extern IntPtr DecryptDataByLicenseKey32(byte[] dataToDecrypty, UInt32 lenData, UInt32 keyType, ref UInt32 outDataSize);

        /// <summary>
        /// 使用授权数据内置密码加密数据
        /// </summary>
        /// <param name="dataToEncrypt">需要解码的数据</param>
        /// <param name="lenData">数据长度</param>
        /// <param name="keyType">密码类型</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>解码后的数据</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "EncryptDataByLicenseKey")]
        private static extern IntPtr EncryptDataByLicenseKey32(byte[] dataToEncrypt, UInt32 lenData, UInt32 keyType, ref UInt32 outDataSize);

        /// <summary>
        /// 刷新授权码
        /// </summary>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "RefreshLicense")]
        private static extern bool RefreshLicense32();

        /// <summary>
        /// 是否为本机授权码
        /// </summary>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IsRightLicense")]
        private static extern bool IsRightLicense32();

        /// <summary>
        /// 获取授权码中的注册码
        /// </summary>
        /// <param name="ID">输出：注册码</param>
        /// <returns></returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseGuid")]
        private static extern bool GetLicenseGuid32(ref Guid ID);

        /// <summary>
        /// 获取授权码中的用户名
        /// </summary>
        /// <param name="outDataSize">输出：数据长度</param>
        /// <returns>UFT8编码的用户名</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseUser")]
        private static extern IntPtr GetLicenseUser32(ref UInt32 outDataSize);

        /// <summary>
        /// 获取所有授权信息
        /// </summary>
        /// <param name="outDataSize">输出：数据长度(DWORD)</param>
        /// <returns>授权信息DWORD</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetAllLicense")]
        private static extern IntPtr GetAllLicense32(ref UInt32 outDataSize);

        /// <summary>
        /// 获取授权码中的注册单位
        /// </summary>
        /// <param name="outDataSize">输出：数据长度</param>
        /// <returns>UFT8编码的注册单位</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseUnit")]
        private static extern IntPtr GetLicenseUnit32(ref UInt32 outDataSize);

        /// <summary>
        /// 是否为本机授权码
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        /// <param name="licenseSize">数据长度</param>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IsRightLicenseFromData")]
        private static extern bool IsRightLicenseFromData32(byte[] licenseData, UInt32 licenseSize);

        /// <summary>
        /// 获取授权码中的注册码
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        /// <param name="licenseSize">数据长度</param>
        /// <param name="ID">输出：注册码</param>
        /// <returns></returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseGuidFromData")]
        private static extern bool GetLicenseGuidFromData32(byte[] licenseData, UInt32 licenseSize, ref Guid ID);

        /// <summary>
        /// 获取授权码中的用户名
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        /// <param name="licenseSize">数据长度</param>
        /// <param name="outDataSize">输出：数据长度</param>
        /// <returns>UFT8编码的用户名</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseUserFromData")]
        private static extern IntPtr GetLicenseUserFromData32(byte[] licenseData, UInt32 licenseSize, ref UInt32 outDataSize);

        /// <summary>
        /// 获取授权码中的注册单位
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        /// <param name="licenseSize">数据长度</param>
        /// <param name="outDataSize">输出：数据长度</param>
        /// <returns>UFT8编码的注册单位</returns>
        [DllImport("KeyVerifier_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseUnitFromData")]
        private static extern IntPtr GetLicenseUnitFromData32(byte[] licenseData, UInt32 licenseSize, ref UInt32 outDataSize);

        #endregion

        #region 64bitDLL

        /// <summary>
        /// 获取硬件码
        /// </summary>
        /// <param name="copysize">一个硬件码的长度</param>
        /// <param name="outDataSize">输出数据长度</param>
        /// <returns>三个硬件码的组合(XXXX-YYYY-ZZZZ)</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetDeviceFinger")]
        private static extern IntPtr GetDeviceFinger64(int copysize, ref int outDataSize);

        /// <summary>
        /// 是否为正确的硬件码（在DLL里面获取硬件码）
        /// </summary>
        /// <param name="fileData">密码文件</param>
        /// <param name="fileSize">密码文件长度</param>
        /// <returns></returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IsRightFinger")]
        private static extern int IsRightFinger64(byte[] fileData, int fileSize);

        /// <summary>
        /// 刷新授权码
        /// </summary>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "RefreshLicense")]
        private static extern bool RefreshLicense64();

        /// <summary>
        /// 通过密码加密数据
        /// </summary>
        /// <param name="dataToEncrypt">明文数据</param>
        /// <param name="lenData">数据长度</param>
        /// <param name="encryptKey">密码</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>加密数据</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PublicEncryptData")]
        private static extern IntPtr PublicEncryptData64(byte[] dataToEncrypt, int lenData, string encryptKey, ref int outDataSize);

        /// <summary>
        /// 通过密码文件解码数据
        /// </summary>
        /// <param name="encryptoedData">密文数据</param>
        /// <param name="lenData">密文数据长度</param>
        /// <param name="keyData">密码</param>
        /// <param name="lenKeyData">密码长度</param>
        /// <param name="outDataSize">解码后的明文数据长度</param>
        /// <returns>明文数据</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PrivateDecryptData")]
        private static extern IntPtr PrivateDecryptData64(byte[] encryptoedData, int lenData, byte[] keyData, int lenKeyData, ref int outDataSize);

        /// <summary>
        /// 通过密码解码数据
        /// </summary>
        /// <param name="encryptoedData">密文数据</param>
        /// <param name="lenData">密文数据长度</param>
        /// <param name="decryptKey">密码</param>
        /// <param name="outDataSize">解码后的明文数据长度</param>
        /// <returns>明文数据</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PublicDecryptData")]
        private static extern IntPtr PublicDecryptData64(byte[] encryptoedData, int lenData, string decryptKey, ref int outDataSize);

        /// <summary>
        /// 将授权数据写入注册表
        /// </summary>
        /// <param name="writeData">授权数据</param>
        /// <param name="dataSize">数据长度</param>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WriteToRegister")]
        private static extern bool WriteToRegister64(byte[] writeData, UInt32 dataSize);

        /// <summary>
        /// 获取授权码中的预定义值
        /// </summary>
        /// <param name="mainType">主类型</param>
        /// <param name="subType">子类型</param>
        /// <param name="dataValue">预定义值</param>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseValue")]
        private static extern bool GetLicenseValue64(UInt32 mainType, UInt32 subType, ref UInt32 dataValue);

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns>enumErrorCode</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseErrorCode")]
        private static extern int GetLicenseErrorCode64();

        /// <summary>
        /// 使用授权数据内置密码解密数据
        /// </summary>
        /// <param name="dataToDecrypt">需要解码的数据</param>
        /// <param name="lenData">数据长度</param>
        /// <param name="keyType">密码类型</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>解码后的数据</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DecryptDataByLicenseKey")]
        private static extern IntPtr DecryptDataByLicenseKey64(byte[] dataToDecrypt, UInt32 lenData, UInt32 keyType, ref UInt32 outDataSize);

        /// <summary>
        /// 使用授权数据内置密码加密数据
        /// </summary>
        /// <param name="dataToEncrypt">需要解码的数据</param>
        /// <param name="lenData">数据长度</param>
        /// <param name="keyType">密码类型</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>解码后的数据</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "EncryptDataByLicenseKey")]
        private static extern IntPtr EncryptDataByLicenseKey64(byte[] dataToEncrypt, UInt32 lenData, UInt32 keyType, ref UInt32 outDataSize);

        /// <summary>
        /// 是否为本机授权码
        /// </summary>
        [DllImport("KeyVerifier_64dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IsRightLicense")]
        private static extern bool IsRightLicense64();

        /// <summary>
        /// 获取授权码中的注册码
        /// </summary>
        /// <param name="ID">输出：注册码</param>
        /// <returns></returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseGuid")]
        private static extern bool GetLicenseGuid64(ref Guid ID);

        /// <summary>
        /// 获取授权码中的用户名
        /// </summary>
        /// <param name="outDataSize">输出：数据长度</param>
        /// <returns>UFT8编码的用户名</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseUser")]
        private static extern IntPtr GetLicenseUser64(ref UInt32 outDataSize);

        /// <summary>
        /// 获取授权码中的注册单位
        /// </summary>
        /// <param name="outDataSize">输出：数据长度</param>
        /// <returns>UFT8编码的注册单位</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseUnit")]
        private static extern IntPtr GetLicenseUnit64(ref UInt32 outDataSize);

        /// <summary>
        /// 获取所有授权信息
        /// </summary>
        /// <param name="outDataSize">输出：数据长度(DWORD)</param>
        /// <returns>授权信息DWORD</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetAllLicense")]
        private static extern IntPtr GetAllLicense64(ref UInt32 outDataSize);

        /// <summary>
        /// 是否为本机授权码
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        /// <param name="licenseSize">数据长度</param>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IsRightLicenseFromData")]
        private static extern bool IsRightLicenseFromData64(byte[] licenseData, UInt32 licenseSize);

        /// <summary>
        /// 获取授权码中的注册码
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        /// <param name="licenseSize">数据长度</param>
        /// <param name="ID">输出：注册码</param>
        /// <returns></returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseGuidFromData")]
        private static extern bool GetLicenseGuidFromData64(byte[] licenseData, UInt32 licenseSize, ref Guid ID);

        /// <summary>
        /// 获取授权码中的用户名
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        /// <param name="licenseSize">数据长度</param>
        /// <param name="outDataSize">输出：数据长度</param>
        /// <returns>UFT8编码的用户名</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseUserFromData")]
        private static extern IntPtr GetLicenseUserFromData64(byte[] licenseData, UInt32 licenseSize, ref UInt32 outDataSize);

        /// <summary>
        /// 获取授权码中的注册单位
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        /// <param name="licenseSize">数据长度</param>
        /// <param name="outDataSize">输出：数据长度</param>
        /// <returns>UFT8编码的注册单位</returns>
        [DllImport("KeyVerifier_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLicenseUnitFromData")]
        private static extern IntPtr GetLicenseUnitFromData64(byte[] licenseData, UInt32 licenseSize, ref UInt32 outDataSize);

        #endregion

        #region 32/64Bit自适应

        private const int INSTALLSTATE_DEFAULT = 5;  //正确安装
        /// <summary>
        /// 检测VS C++ 2013运行环境是否安装
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [DllImport("msi.dll")]
        private static extern int MsiQueryProductState(string product); 

        //Visual C++ 2013 Redistributable Package (x86)
        //{13A4EE12-23EA-3371-91EE-EFB36DDFFF3E} and {F8CFEB22-A2E7-3971-9EDA-4B11EDEFC185}
        //Visual C++ 2013 Redistributable Package (x64)
        //{929FBD26-9020-399B-9A7A-751D61F0B942} and {A749D8E6-B613-3BE3-8F5F-045C84EBA29B}

        /// <summary>
        /// 检查VC运行环境是否安装了
        /// </summary>
        /// <returns></returns>
        public static bool VCEnvirmentInstalled()
        {
            if(Is64BitVersion())
            {
                //检测64bit
                return MsiQueryProductState("{929FBD26-9020-399B-9A7A-751D61F0B942}") == INSTALLSTATE_DEFAULT &&
                    MsiQueryProductState("{A749D8E6-B613-3BE3-8F5F-045C84EBA29B}") == INSTALLSTATE_DEFAULT;
            }
            else
            {
                //先检测64bit
                if (MsiQueryProductState("{929FBD26-9020-399B-9A7A-751D61F0B942}") == INSTALLSTATE_DEFAULT &&
                    MsiQueryProductState("{A749D8E6-B613-3BE3-8F5F-045C84EBA29B}") == INSTALLSTATE_DEFAULT)
                    return true;

                //再检测32bit
                return MsiQueryProductState("{13A4EE12-23EA-3371-91EE-EFB36DDFFF3E}") == INSTALLSTATE_DEFAULT &&
                    MsiQueryProductState("{F8CFEB22-A2E7-3971-9EDA-4B11EDEFC185}") == INSTALLSTATE_DEFAULT;
            }
        }

        /// <summary>
        /// 是否为正确的密码文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        public static bool IsRightFinger(byte[] fileData)
        {
            try
            {
                if (Is64BitVersion())
                    return IsRightFinger64(fileData, fileData.Length) == 0;
                else
                    return IsRightFinger32(fileData, fileData.Length) == 0;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }
        
        /// <summary>
        /// 刷新授权码
        /// </summary>
        public static bool RefreshLicense()
        {
            try
            {
                if (Is64BitVersion())
                    return RefreshLicense64();
                else
                    return RefreshLicense32();
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取设备硬件码
        /// </summary>
        public static string GetDeviceFinger()
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (Is64BitVersion())
                {
                    retptr = GetDeviceFinger64(fingerIDLen, ref datasize);
                }
                else
                {
                    retptr = GetDeviceFinger32(fingerIDLen, ref datasize);
                }
                return CopyStringFromIntptrAndFree(ref retptr, datasize, Encoding.ASCII);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 使用密码加密数据
        /// </summary>
        /// <param name="dataToEncrypt">需要加密的数据</param>
        /// <param name="encryptKey">密码</param>
        /// <returns></returns>
        public static byte[] EncryptData(byte[] dataToEncrypt, string encryptKey)
        {
            if (dataToEncrypt == null || dataToEncrypt.Length == 0 || string.IsNullOrWhiteSpace(encryptKey))
            {
                ErrorString = "Invalid Paramters";
                return null;
            }
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (Is64BitVersion())
                    retptr = PublicEncryptData64(dataToEncrypt, dataToEncrypt.Length, encryptKey, ref datasize);
                else
                    retptr = PublicEncryptData32(dataToEncrypt, dataToEncrypt.Length, encryptKey, ref datasize);

                return CopyDataArrayFromIntptrAndFree<byte>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 解码数据（使用密码或者密码文件来解码）
        /// </summary>
        /// <param name="dataToDecrypt">需要解码的数据</param>
        /// <param name="encryptKey">密码</param>
        /// <param name="keyData">密码文件</param>
        public static byte[] DecryptData(byte[] dataToDecrypt, string encryptKey, byte[] keyData)
        {
            if (dataToDecrypt == null || dataToDecrypt.Length == 0 || 
                (string.IsNullOrWhiteSpace(encryptKey) && (keyData == null || keyData.Length==0)))
            {
                ErrorString = "Invalid Paramters";
                return null;
            }

            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (Is64BitVersion())
                {
                    if(keyData == null)
                        retptr = PublicDecryptData64(dataToDecrypt, dataToDecrypt.Length, encryptKey, ref datasize);
                    else
                        retptr = PrivateDecryptData64(dataToDecrypt, dataToDecrypt.Length, keyData, keyData.Length, ref datasize);
                }
                else
                {
                    if (keyData == null)
                        retptr = PublicDecryptData32(dataToDecrypt, dataToDecrypt.Length, encryptKey, ref datasize);
                    else
                        retptr = PrivateDecryptData32(dataToDecrypt, dataToDecrypt.Length, keyData, keyData.Length, ref datasize);
                }

                return CopyDataArrayFromIntptrAndFree<byte>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 写入授权数据到注册表
        /// </summary>
        /// <param name="data">授权数据</param>
        /// <returns></returns>
        public static bool WriteLicenseDataToRegister(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                ErrorString = "Invalid Paramters";
                return false;
            }

            try
            {
                if (Is64BitVersion())
                    return WriteToRegister64(data, (UInt32)data.Length);
                else
                    return WriteToRegister32(data, (UInt32)data.Length);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }

        }
        
        /// <summary>
        /// 获取预定义的值
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="subType">子类型</param>
        /// <param name="value">返回值</param>
        public static bool GetLicenseValue(enumDataType dataType, int  subType, ref UInt32 value)
        {
            try
            {
                if (Is64BitVersion())
                    return GetLicenseValue64((UInt32)dataType, (UInt32)subType, ref value);
                else
                    return GetLicenseValue32((UInt32)dataType, (UInt32)subType, ref value);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 使用授权数据中的密码解码数据
        /// </summary>
        /// <param name="dataToDecrypt">需要解码的数据</param>
        /// <param name="keyType">密码类型</param>
        /// <returns>解码后的数据</returns>
        public static byte[] DecryptDataByLicenseKey(byte[] dataToDecrypt, enumEncryptKeyType keyType)
        {
            if (dataToDecrypt == null || dataToDecrypt.Length == 0 || (int)keyType <= (int)enumEncryptKeyType.EXTERNAL_ENCRYPT)
            {
                ErrorString = "Invalid Parameters";
                return null;
            }

            try
            {
                IntPtr retptr = IntPtr.Zero;
                UInt32 datasize = 0;

                if (Is64BitVersion())
                    retptr = DecryptDataByLicenseKey64(dataToDecrypt, (UInt32)dataToDecrypt.Length, (UInt32)keyType, ref datasize);
                else
                    retptr = DecryptDataByLicenseKey32(dataToDecrypt, (UInt32)dataToDecrypt.Length, (UInt32)keyType, ref datasize);

                return CopyDataArrayFromIntptrAndFree<byte>(ref retptr, (int)datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 使用授权数据中的密码加密数据
        /// </summary>
        /// <param name="dataToEncrypt">需要加密的数据</param>
        /// <param name="keyType">密码类型</param>
        /// <returns>加密后的数据</returns>
        public static byte[] EncryptDataByLicenseKey(byte[] dataToEncrypt, enumEncryptKeyType keyType)
        {
            if (dataToEncrypt == null || dataToEncrypt.Length == 0 || (int)keyType <= (int)enumEncryptKeyType.EXTERNAL_ENCRYPT)
            {
                ErrorString = "Invalid Parameters";
                return null;
            }

            try
            {
                IntPtr retptr = IntPtr.Zero;
                UInt32 datasize = 0;

                if (Is64BitVersion())
                    retptr = EncryptDataByLicenseKey64(dataToEncrypt, (UInt32)dataToEncrypt.Length, (UInt32)keyType, ref datasize);
                else
                    retptr = EncryptDataByLicenseKey32(dataToEncrypt, (UInt32)dataToEncrypt.Length, (UInt32)keyType, ref datasize);

                return CopyDataArrayFromIntptrAndFree<byte>(ref retptr, (int)datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public static enumErrorCode GetErrorCode()
        {
            try
            {
                if (Is64BitVersion())
                    return (enumErrorCode)GetLicenseErrorCode64();
                else
                    return (enumErrorCode)GetLicenseErrorCode32();
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return enumErrorCode.NotAuthor;
            }
        }

        /// <summary>
        /// 是否是本机授权数据
        /// </summary>
        public static bool IsRightLicense()
        {
            try
            {
            if (Is64BitVersion())
                return IsRightLicense64();
            else
                return IsRightLicense32();
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 从授权数据中获取授权码
        /// </summary>
        /// <returns>授权码</returns>
        public static Guid GetLicenseGuid()
        {
            try
            {
                Guid ID = Guid.Empty;
                bool ret;
                if (Is64BitVersion())
                    ret = GetLicenseGuid64(ref ID);
                else
                    ret = GetLicenseGuid32(ref ID);

                return ID;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 获取授权用户
        /// </summary>
        /// <returns>用户名</returns>
        public static string GetLicenseUser()
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                UInt32 datasize = 0;
                if (Is64BitVersion())
                    retptr = GetLicenseUser64(ref datasize);
                else
                    retptr = GetLicenseUser32(ref datasize);

                return CopyStringFromIntptrAndFree(ref retptr, (int)datasize, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取授权单位
        /// </summary>
        /// <returns>用户单位</returns>
        public static string GetLicenseUnit()
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                UInt32 datasize = 0;
                if (Is64BitVersion())
                    retptr = GetLicenseUnit64(ref datasize);
                else
                    retptr = GetLicenseUnit32(ref datasize);

                return CopyStringFromIntptrAndFree(ref retptr, (int)datasize, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取所有授权列表
        /// </summary>
        /// <returns>授权列表</returns>
        public static UInt32[] GetAllLicense()
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                UInt32 datasize = 0;
                if (Is64BitVersion())
                    retptr = GetAllLicense64(ref datasize);
                else
                    retptr = GetAllLicense32(ref datasize);

                return CopyDataArrayFromIntptrAndFree<UInt32>(ref retptr, (int)datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 是否是本机授权数据
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        public static bool IsRightLicenseFromData(byte[] licenseData)
        {
            try
            {
                if (licenseData == null || licenseData.Length == 0)
                {
                    ErrorString = "Invalid Parameters";
                    return false;
                }

                if (Is64BitVersion())
                    return IsRightLicenseFromData64(licenseData, (UInt32)licenseData.Length);
                else
                    return IsRightLicenseFromData32(licenseData, (UInt32)licenseData.Length);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 从授权数据中获取授权码
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        /// <returns>授权码</returns>
        public static Guid GetLicenseGuidFromData(byte[] licenseData)
        {
            try
            {
                Guid ID = Guid.Empty;
                if (licenseData == null || licenseData.Length == 0)
                {
                    ErrorString = "Invalid Parameters";
                    return ID;
                }

                bool ret;
                if (Is64BitVersion())
                    ret = GetLicenseGuidFromData64(licenseData, (UInt32)licenseData.Length, ref ID);
                else
                    ret = GetLicenseGuidFromData32(licenseData, (UInt32)licenseData.Length, ref ID);

                return ID;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 从授权数据中获取用户
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        /// <returns>授权码</returns>
        public static string GetLicenseUserFromData(byte[] licenseData)
        {
            if (licenseData == null || licenseData.Length == 0)
            {
                ErrorString = "Invalid Parameters";
                return null;
            }

            try
            {
                IntPtr retptr = IntPtr.Zero;
                UInt32 datasize = 0;
                if (Is64BitVersion())
                    retptr = GetLicenseUserFromData64(licenseData, (UInt32)licenseData.Length, ref datasize);
                else
                    retptr = GetLicenseUserFromData32(licenseData, (UInt32)licenseData.Length, ref datasize);

                return CopyStringFromIntptrAndFree(ref retptr, (int)datasize, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 从授权数据中获取单位
        /// </summary>
        /// <param name="licenseData">授权数据</param>
        /// <returns>授权码</returns>
        public static string GetLicenseUnitFromData(byte[] licenseData)
        {
            if (licenseData == null || licenseData.Length == 0)
            {
                ErrorString = "Invalid Parameters";
                return null;
            }

            try
            {
                IntPtr retptr = IntPtr.Zero;
                UInt32 datasize = 0;
                if (Is64BitVersion())
                    retptr = GetLicenseUnitFromData64(licenseData, (UInt32)licenseData.Length, ref datasize);
                else
                    retptr = GetLicenseUnitFromData32(licenseData, (UInt32)licenseData.Length, ref datasize);

                return CopyStringFromIntptrAndFree(ref retptr, (int)datasize, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        #endregion

        /// <summary>
        /// 判断是否为64为版本调用
        /// </summary>
        public static bool Is64BitVersion()
        {
            return IntPtr.Size == 8;
        }

        /// <summary>
        /// 从内存中拷贝float数据，并且释放内存
        /// </summary>
        /// <param name="ptrData">内存数据</param>
        /// <param name="dataSize">数据大小(float)</param>
        /// <returns></returns>
        public static T[] CopyDataArrayFromIntptrAndFree<T>(ref IntPtr ptrData, int dataSize)
        {
            //拷贝校正系数出来
            if (ptrData == IntPtr.Zero)
                return null;

            T[] retdata = new T[dataSize];
            try
            {
                if (typeof(T) == typeof(byte))
                {
                    byte[] temp = (byte[])((object)retdata);
                    Marshal.Copy(ptrData, temp, 0, dataSize);
                }
                else if (typeof(T) == typeof(int))
                {
                    int[] temp = (int[])((object)retdata);
                    Marshal.Copy(ptrData, temp, 0, dataSize);
                }
                else if (typeof(T) == typeof(char))
                {
                    char[] temp = (char[])((object)retdata);
                    Marshal.Copy(ptrData, temp, 0, dataSize);
                }
                else if (typeof(T) == typeof(float))
                {
                    float[] temp = (float[])((object)retdata);
                    Marshal.Copy(ptrData, temp, 0, dataSize);
                }
                else if (typeof(T) == typeof(double))
                {
                    double[] temp = (double[])((object)retdata);
                    Marshal.Copy(ptrData, temp, 0, dataSize);
                }
                else if (typeof(T) == typeof(UInt32))
                {
                    int[] temp = new int[dataSize];
                    Marshal.Copy(ptrData, temp, 0, dataSize);

                    UInt32[] uinttemp = (UInt32[])((object)retdata);
                    for (int i = 0; i < dataSize; i++)
                        uinttemp[i] = (UInt32)(temp[i]);
                }

                Marshal.FreeCoTaskMem(ptrData);
                ptrData = IntPtr.Zero;

                return retdata;

            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 从内存中拷贝字符串，并释放内存
        /// </summary>
        /// <param name="ptrData">内存地址</param>
        /// <param name="datasize">数据大小</param>
        /// <param name="encode">编码方式</param>
        /// <returns></returns>
        public static string CopyStringFromIntptrAndFree(ref IntPtr ptrData, int datasize, Encoding encode)
        {
            byte[] datas = CopyDataArrayFromIntptrAndFree<byte>(ref ptrData, datasize);
            if(datas == null)
                return null;
            int len = datas.Length - 1;
            while (len > 0 && datas[len] == 0)
                len--;
            if (len == 0)
                return null;

            return encode.GetString(datas, 0, len+1);
        }

        /// <summary>
        /// 将string转换为Byte
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encode">编码方式</param>
        /// <returns></returns>
        public static byte[] StringToByte(string str, Encoding encode)
        {
            byte[] tempbytes = encode.GetBytes(str);
            if (tempbytes == null)
            {
                ErrorString = "Invalid Parameters";
                return null;
            }

            int addlen = 0;
            if (encode == Encoding.ASCII)
                addlen = 1;
            else
                addlen = 2;

            byte[] retbytes = new byte[tempbytes.Length + addlen];
            Array.Copy(tempbytes, retbytes, tempbytes.Length);

            return retbytes;
        }

        /// <summary>
        /// 读取全部文件内容
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>文件内容</returns>
        public static byte[] ReadBinaryFile(string filename)
        {
            try
            {
                using (System.IO.FileStream stream = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    byte[] retdata = new byte[stream.Length];
                    if (stream.Read(retdata, 0, retdata.Length) != retdata.Length)
                    {
                        ErrorString = "File read error";
                        retdata = null;
                    }
                    stream.Close();

                    return retdata;
                }
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 将数据写入文件
        /// </summary>
        /// <param name="filename">写入文件名</param>
        /// <param name="fileData">文件内容</param>
        public static bool SaveBinaryFile(string filename, byte[] fileData)
        {
            if (fileData == null || filename == null)
            {
                ErrorString = "Invalid Parameters";
                return false;
            }

            try
            {
                using (System.IO.FileStream stream = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    stream.Write(fileData, 0, fileData.Length);
                    stream.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

    }
}
