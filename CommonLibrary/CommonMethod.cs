using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Documents;
using System.Xaml;
using System.Xml.Serialization;
using System.Printing;
using System.Security.Cryptography;

namespace Ai.Hong.Common
{
    /// <summary>
    /// 通用功能
    /// </summary>
    public static class CommonMethod
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern int WritePrivateProfileString(string section, string key, string writeVal, string filePath);
        private const int GWL_STYLE = -16; private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = null;
        /// <summary>
        /// GBCode2312
        /// </summary>
        public const string GBCode2312 = "GB2312";
        /// <summary>
        /// UTF8
        /// </summary>
        public const string UTF8 = "utf-8";
        /// <summary>
        /// 时间的长格式
        /// </summary>
        public const string LongDateTimeString = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 时间短格式
        /// </summary>
        public const string ShortDateTimeString = "yyyy-MM-dd";

        /// <summary>
        /// 将类序列化到字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="serialObj">序列化的对象</param>
        /// <returns>序列化得到的XML字符串</returns>
        public static string Serialize<T>(T serialObj)
        {
            ErrorString = null;
            try
            {
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(serialObj.GetType());
                StringWriter textWriter = new StringWriter();
                xs.Serialize(textWriter, serialObj);
                return textWriter.ToString();
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 从字符串中反序列化类
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="serialString">序列化的XML字符串</param>
        /// <returns>反序列化的对象</returns>
        public static T Deserialize<T>(string serialString)
        {
            ErrorString = null;
            try
            {
                StringReader textReader = new StringReader(serialString);

                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                T retData = (T)xs.Deserialize(textReader);

                return retData;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return default(T);
            }
        }

        /// <summary>
        /// 从文件中反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filename">文件名</param>
        /// <returns>反序列化的对象</returns>
        public static T DeserializeFromFile<T>(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                    throw new Exception("文件不存在:"+filename);

                System.IO.StreamReader sr = new System.IO.StreamReader(filename, true);
                string fileContent = sr.ReadToEnd();
                sr.Close();

                return Deserialize<T>(fileContent);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return default(T);
            }
        }

        /// <summary>
        /// 序列化对象到文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filename">文件名</param>
        /// <param name="serializeObject">序列化对象</param>
        public static bool SerializeToFile<T>(T serializeObject, string filename)
        {
            try
            {
                string contentStr = Serialize<T>(serializeObject);

                System.IO.StreamWriter stream = new System.IO.StreamWriter(filename, false, Encoding.GetEncoding(UTF8));
                stream.WriteLine(contentStr);
                stream.Close();

                return true;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }

        }

        /// <summary>
        /// 隐藏窗口最大、最小、关闭按钮
        /// </summary>
        /// <param name="dstWnd"></param>
        public static void HideWindowSystemButton(Window dstWnd)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(dstWnd).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        /// <summary>
        /// ErrorMsgBox
        /// </summary>
        /// <param name="errstr"></param>
        public static void ErrorMsgBox(string errstr)
        {
            MessageBox.Show(errstr, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// InfoMsgBox
        /// </summary>
        /// <param name="infostr"></param>
        public static void InfoMsgBox(string infostr)
        {
            MessageBox.Show(infostr, "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// QuestionMsgBox
        /// </summary>
        /// <param name="questionStr"></param>
        /// <returns></returns>
        public static bool QuestionMsgBox(string questionStr)
        {
            if (MessageBox.Show(questionStr, "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                return true;

            return false;
        }

        /// <summary>
        /// 从资源文件中创建Stream
        /// </summary>
        /// <param name="filename">资源文件名</param>
        /// <param name="assemb">当前调用进程的Assembly</param>
        public static Stream StreamFromResource(Assembly assemb, string filename)
        {
            if (assemb == null)
                return null;

            foreach (string resname in assemb.GetManifestResourceNames())
            {
                if (resname.IndexOf(filename) > 0)
                {
                    return assemb.GetManifestResourceStream(resname);
                }
            }
            return null;
        }

        /// <summary>
        /// 字符串转换到UTF-8编码
        /// </summary>
        public static bool ConvertToUTF_8(string methodFile)
        {
            try
            {
                StreamReader reader = new StreamReader(methodFile, Encoding.GetEncoding(GBCode2312));
                string fileContent = reader.ReadToEnd();
                reader.Close();

                StreamWriter writer = new StreamWriter(methodFile, false, Encoding.GetEncoding(UTF8));
                writer.Write(fileContent);
                writer.Close();

                return true;
            }
            catch (Exception ex)
            {
                ErrorMsgBox(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 加载图像
        /// </summary>
        /// <param name="imageFile">图像路径</param>
        public static BitmapImage LoadBitmapImage(string imageFile)
        {
            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new System.Uri(imageFile);
                bi.EndInit();

                return bi;
            }
            catch (Exception)
            {
                return null;   
            }
        }

        /// <summary>
        /// 加载资源图像
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        public static BitmapImage LoadImageFromInnerResource(string resourceName)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetCallingAssembly();
            System.IO.Stream stream = asm.GetManifestResourceStream(resourceName);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            return bitmap;
        }

        /// <summary>
        /// 设置Image Control的图像
        /// </summary>
        /// <param name="imgCtrl">Image Control</param>
        /// <param name="imgPath">图像在文件中的路径例如：pack://application:,,/Images</param>
        /// <param name="imgFile">图形文件名(在Images目录下)</param>
        public static void SetImageSource(Image imgCtrl, string imgPath, string imgFile)
        {
            string tempimgfile = imgPath + imgFile;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new System.Uri(tempimgfile);
            bi.EndInit();
            imgCtrl.Source = bi;
        }

        /// <summary>
        /// 字符串是否为空或者NULL
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(string str)
        {
            return str == null || str.Trim() == "";
        }

        /// <summary>
        /// 获取Enum的描述
        /// </summary>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        /// <summary>
        /// 从描述中获取Enum的值
        /// </summary>
        /// <param name="enumtype"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static string GetEnumFromDescription(System.Type enumtype, string description)
        {
            FieldInfo[] fieldInfos = enumtype.GetFields();

            foreach (FieldInfo info in fieldInfos)
            {
                object[] attribArray = info.GetCustomAttributes(false);
                if (attribArray.Length > 0)
                {
                    DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                    if (attrib.Description == description)
                        return info.Name;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取Enum描述列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static System.Collections.IList EnumDescriptionToList(this Type type)
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            Array enumValues = Enum.GetValues(type);

            foreach (Enum value in enumValues)
            {
                list.Add(CommonMethod.GetEnumDescription(value));
            }

            return list;
        }

        /// <summary>
        /// 拷贝文件夹中所有文件到新文件夹
        /// </summary>
        public static void DirectoryCopy(string SourcePath, string DestinationPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            //Copy all the files
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
        }

        /// <summary>
        /// 添加到log文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="msg"></param>
        public static void AddToLogFile(string filename, string msg)
        {
            try
            {
                StreamWriter writer = new StreamWriter(filename, true, System.Text.Encoding.GetEncoding(GBCode2312));
                writer.WriteLine(DateTime.Now.ToString(LongDateTimeString) + msg);
                writer.Close();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 移除DataGrid中的选定项
        /// </summary>
        /// <param name="allItems"></param>
        /// <param name="removeItems"></param>
        public static void RemoveDataGridItems(System.Collections.IList allItems, System.Collections.IList removeItems)
        {
            List<object> tempList = new List<object>();
            foreach (object item in removeItems)
                tempList.Add(item);

            foreach (object item in tempList)
            {
                if(allItems.IndexOf(item) >= 0)
                    allItems.Remove(item);
            }
        }

        /// <summary>
        /// 删除进程
        /// </summary>
        /// <param name="processName">进程名称</param>
        public static void KillProcess(string processName)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                for (int i = 0; i < processes.Length; i++)
                    processes[i].Kill();
            }
        }

        /// <summary>
        /// 读取Ini配置文件
        /// </summary>
        public static string ReadIniFile(string iniFile, string section, string key)
        {
            StringBuilder retstr = new StringBuilder(256);

            GetPrivateProfileString(section, key, "", retstr, 256, iniFile);
            return retstr.ToString();
        }

        /// <summary>
        /// 写入Ini配置文件
        /// </summary>
        public static void WriteIniFile(string iniFile, string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, iniFile);
        }

        /// <summary>
        /// 获取C:\Users\Public\Documents\???\setting  文件夹路径
        /// </summary>
        /// <param name="programName">应用程序文件名，缺省为运行程序名称</param>
        /// <returns></returns>
        public static string GetProgramDataPath(string programName=null)
        {
            string exeFilename = System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            exeFilename = System.IO.Path.GetFileNameWithoutExtension(exeFilename);
            string commonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            return programName == null ? Path.Combine(commonPath, exeFilename) : Path.Combine(commonPath, programName);
        }

        /// <summary>
        /// 加密一个字符串
        /// </summary>
        /// <param name="source">需要加密的字符串</param>
        /// <param name="getTemplate">True=加密, False=解密</param>
        public static string GetDatabaseTemplate(string source, bool getTemplate)
        {
            PrintDatabaseTemplate template = new PrintDatabaseTemplate();
            return getTemplate ? template.Encrypto(source) : template.Decrypto(source);
        }

        /// <summary>
        /// 加密一个字符串
        /// </summary>
        /// <param name="source">需要加密的字符串</param>
        /// <param name="destionation">密码</param>
        /// <param name="getTemplate">True=加密, False=解密</param>
        public static string GetDatabaseTemplate(string source, string destionation, bool getTemplate)
        {
            PrintDatabaseTemplate template = new PrintDatabaseTemplate(destionation, null);
            return getTemplate ? template.Encrypto(source) : template.Decrypto(source);
        }

        /// <summary>
        /// 是否为正确的路径名称（不包含非法字符）
        /// </summary>
        /// <param name="pathname"></param>
        /// <returns></returns>
        public static bool IsValidPathName(string pathname)
        {
            foreach (char st in Path.GetInvalidPathChars())
            {
                if (pathname.IndexOf(st) >= 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 是否为正确的文件名称（不包含非法字符）
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsValidFileName(string filename)
        {
            foreach (char st in Path.GetInvalidFileNameChars())
            {
                if (filename.IndexOf(st) >= 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 替换不能在目录和文件名中使用的非法字符
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <returns>替换后的字符串</returns>
        public static string ReplaceInvalidFileCharactor(string str)
        {
            if (str == null)
                return str;

            //去除文件名中非法字符，防止错误
            char[] badchar = new char[] { '"', '/', ':', '?', '<', '>', '*', '\\' };
            
            bool found = false;
            foreach (char st in badchar)
            {
                if (str.IndexOf(st) >= 0)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                foreach (char st in badchar)
                    str = str.Replace(st, '-');
            }

            return str;
        }

        /// <summary>
        /// 获取唯一的文件名
        /// </summary>
        public static string GetUniqueFilename(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return null;

            if (!File.Exists(filename))
                return filename;

            string ext = Path.GetExtension(filename);
            string path = Path.GetDirectoryName(filename);
            string file = Path.GetFileNameWithoutExtension(filename);

            //如果文件名已经存在，则在后面添加-1, -2, -3等等
            int index = 1;
            while (File.Exists(filename))
            {
                filename = Path.Combine(path, file + "_" + index + ext);
                index++;
            }

            return filename;
        }
    

    /// <summary>
    /// 获取GB2312的文字编码格式
    /// </summary>
    /// <returns></returns>
    public static Encoding GetGBCodeEncoding()
        {
            return Encoding.GetEncoding(GBCode2312);
        }

        /// <summary>
        /// 获取单个硬件信息
        /// </summary>
        /// <param name="hardwareType">硬件类型</param>
        /// <param name="propertyType">硬件属性</param>
        /// <returns>硬件信息</returns>
        public static string GetComputerHardwareID(string hardwareType, string propertyType)
        {
            try
            {
                string hardID = null;
                System.Management.ManagementClass cimobject = new System.Management.ManagementClass(hardwareType);
                System.Management.ManagementObjectCollection moc = cimobject.GetInstances();

                foreach (System.Management.ManagementObject mo in moc)
                {
                    if (hardwareType == "Win32_NetworkAdapterConfiguration")
                    {
                        if (mo["IPEnabled"].ToString() == "True")
                            hardID = (string)mo.Properties[propertyType].Value;
                    }
                    else
                        hardID = (string)mo.Properties[propertyType].Value;

                    if(hardID != null)
                        break;
                }

                return hardID;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 获取硬件指纹
        /// </summary>
        /// <returns></returns>
        public static string GetComputerFinger()
        {
            //string diskID = GetComputerHardwareID("Win32_DiskDrive", "Model");              //磁盘信息
            ////string processorID = GetComputerHardwareID("Win32_Processor", "ProcessorId");   //处理器信息
            //string macID = GetComputerHardwareID("Win32_NetworkAdapterConfiguration", "MacAddress");   //网卡Mac地址
            //string boardID = GetComputerHardwareID("Win32_BaseBoard", "SerialNumber");      //主板信息

            string diskID = GetComputerHardwareID("Win32_DiskDrive", "Model");              //磁盘信息
            //string processorID = GetComputerHardwareID("Win32_Processor", "ProcessorId");   //处理器信息
            //string macID = GetComputerHardwareID("Win32_baseboard", "Manufacturer");   //主板制造商
            string macID = GetComputerHardwareID("Win32_PhysicalMedia", "SerialNumber");   //磁盘系列号
            string boardID = GetComputerHardwareID("Win32_BaseBoard", "SerialNumber");      //主板信息

            if (diskID == null || macID == null || boardID == null)
                return null;

            //至少4位，否则填零
            diskID = diskID.PadRight(4, '0');
            if (macID != null)  //去掉':'
                macID = macID.Replace(":", "");
            macID = macID.PadRight(4, '0');
            boardID = boardID.PadRight(4, '0');

            //最后4个字符
            diskID = diskID.Substring(diskID.Length - 4, 4) + "-" + macID.Substring(macID.Length - 4, 4) + "-" + boardID.Substring(boardID.Length - 4, 4);
            diskID = diskID.Replace('.', '0');

            return diskID;
        }

        /// <summary>
        /// 读取全部文件内容
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>文件内容</returns>
        public static byte[] ReadFileBinnaryData(string filename)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                byte[] registerData = new byte[stream.Length];
                if (stream.Read(registerData, 0, registerData.Length) != registerData.Length)
                {
                    Ai.Hong.Common.CommonMethod.ErrorMsgBox("File read error");
                    registerData = null;
                }
                stream.Close();

                return registerData;
            }
            catch (Exception ex)
            {
                if (stream != null)
                    stream.Close();

                Ai.Hong.Common.CommonMethod.ErrorMsgBox(ex.Message);

                return null;
            }
        }

        /// <summary>
        /// 将数据写入文件
        /// </summary>
        /// <param name="filename">写入文件名</param>
        /// <param name="fileData">文件内容</param>
        public static bool SaveBinnaryDataToFile(string filename, byte[] fileData)
        {
            if (fileData == null || filename == null)
                return false;

            FileStream stream = null;
            try
            {
                stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                stream.Write(fileData, 0, fileData.Length);
                stream.Close();

                return true;
            }
            catch (Exception ex)
            {
                if (stream != null)
                    stream.Close();
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 比较两个文件名是否相同
        /// </summary>
        /// <returns></returns>
        public static bool IsSameFile(string file1, string file2)
        {
            return string.Compare(Path.GetFullPath(file1), Path.GetFullPath(file2), true) == 0;
        }

        /// <summary>
        /// 判断是否为中文操作系统
        /// </summary>
        /// <returns></returns>
        public static bool IsChineseOS()
        {
            return System.Globalization.CultureInfo.InstalledUICulture.Name.ToString(System.Globalization.CultureInfo.InvariantCulture) == "zh-CN";
        }

        /// <summary>
        /// 通过Ping命令测试仪器是否开机
        /// <param name="ipAddress">IP地址</param>
        /// <param name="timeout">timeout时间(毫秒)</param>
        /// </summary>
        public static bool PingDevice(string ipAddress, int timeout)
        {
            //通过Ping命令测试仪器是否开机
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
            options.DontFragment = true;
            string data = "Test!";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            ErrorString = null;
            try
            {
                System.Net.NetworkInformation.PingReply reply = p.Send(ipAddress, timeout, buffer, options);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)     //能够Ping通，返回True
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 通过名称查找ItemsControl包含的子节点
        /// </summary>
        /// <param name="currentNode">当前结点</param>
        /// <param name="name">查找的名称</param>
        /// <returns>找到的节点</returns>
        public static ItemsControl FindChildByName(ItemsControl currentNode, string name)
        {
            if (currentNode == null || string.IsNullOrEmpty(name))
                return null;

            if (currentNode.Name == name)
                return currentNode;

            if (currentNode.Items == null || currentNode.Items.Count == 0)
                return null;

            ItemsControl foundItem = null;
            foreach (var item in currentNode.Items)
            {
                foundItem = FindChildByName(item as ItemsControl, name);
                if (foundItem != null)
                    return foundItem;
            }

            return foundItem;
        }

        /// <summary>
        /// 调整curItem在allItems列表中的顺序
        /// </summary>
        /// <param name="allItems">项目列表</param>
        /// <param name="curItem">要调整的项目</param>
        /// <param name="moveUp">True:上移，False:下移</param>
        /// <returns>调整后curItem的序号</returns>
        public static int AdjustItemOrderInList(System.Collections.IList allItems, object curItem, bool moveUp)
        {
            if (allItems == null || curItem == null || allItems.Count == 0)
                return -1;

            int index = allItems.IndexOf(curItem);
            if (index == -1)
                return -1;

            if(moveUp)
            {
                if (index < 1)
                    return -1;
                allItems.Remove(curItem);
                allItems.Insert(index - 1, curItem);

                return index - 1;
            }
            else
            {
                if (index >= allItems.Count - 1)
                    return -1;
                allItems.Remove(curItem);
                allItems.Insert(index + 1, curItem);

                return index + 1;
            }
        }
    }

    /// <summary>
    /// 加密程序
    /// </summary>
    class PrintDatabaseTemplate
    {
        //  Call this function to remove the key from memory after use for security(以后再考虑)
        [System.Runtime.InteropServices.DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);
        
        private SymmetricAlgorithm _objCryptoService;
        private string _strKey = "Guz(%wif89469#(*^@_0+=499chk&fvHUFCy76*h%(HilJKIEIYLM#($&kP87jH7";
        private string _strIV = "EEIJGK*&49CVo04VW:{CUY86GfghUb#er57HBh(LQ@E*&:SD{WOCK)#(9qw0chjk";
        /// <summary>   
        /// Conctructor
        /// </summary>   
        public PrintDatabaseTemplate(string key, string IV)
        {
            _objCryptoService = new RijndaelManaged();
            _objCryptoService = new TripleDESCryptoServiceProvider();

            if(key != null)
                _strKey = key;
            if(IV != null)
                _strIV = IV;
        }
        public PrintDatabaseTemplate()
        {
            _objCryptoService = new RijndaelManaged();
            _objCryptoService = new TripleDESCryptoServiceProvider();
        }
        /// <summary>   
        /// Get the key   
        /// </summary>   
        /// <returns>key</returns>   
        private byte[] GetLegalKey()
        {
            string sTemp = _strKey;
            _objCryptoService.GenerateKey();
            byte[] bytTemp = _objCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>   
        /// Initialize IV   
        /// </summary>   
        /// <returns>Initialize IV</returns>   
        private byte[] GetLegalIV()
        {
            _objCryptoService.GenerateIV();
            byte[] bytTemp = _objCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (_strIV.Length > IVLength)
                _strIV = _strIV.Substring(0, IVLength);
            else if (_strIV.Length < IVLength)
                _strIV = _strIV.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(_strIV);
        }

        /// <summary>   
        /// Encrypto string
        /// </summary>   
        /// <param name="Source">The source string need to encrypto</param>   
        /// <returns>The string after crypto</returns>   
        public string Encrypto(string Source)
        {
            if (string.IsNullOrWhiteSpace(Source))
                return null;

            //string base64String = "PASSWORD"+ Source;
            //while (base64String.Length % 8 != 0)
            //    base64String = "A" + base64String;

            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
            string strRet = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    _objCryptoService.Key = GetLegalKey();
                    _objCryptoService.IV = GetLegalIV();

                    ICryptoTransform encrypto = _objCryptoService.CreateEncryptor();
                    using (CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write))
                    {
                        cs.Write(bytIn, 0, bytIn.Length);
                        cs.FlushFinalBlock();
                        byte[] bytOut = ms.ToArray();
                        strRet = System.Convert.ToBase64String(bytOut);
                        cs.Clear();
                        cs.Close();
                    }
                    ms.Close();
                }
                return strRet;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>   
        /// Encrypto byte array
        /// </summary>   
        /// <param name="source">The source bytes need to encrypto</param>   
        /// <returns>The bytes after crypto</returns>   
        public byte[] Encrypto(byte[] source)
        {
            if (source == null || source.Length == 0)
                return null;

            byte[] bytIn = source;
            byte[] bytOut = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    _objCryptoService.Key = GetLegalKey();
                    _objCryptoService.IV = GetLegalIV();
                    ICryptoTransform encrypto = _objCryptoService.CreateEncryptor();
                    using (CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write))
                    {
                        cs.Write(bytIn, 0, bytIn.Length);
                        cs.FlushFinalBlock();
                        bytOut = ms.ToArray();
                        cs.Clear();
                        cs.Close();
                    }
                    ms.Close();
                }
                return bytOut;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>   
        /// Decrypto string
        /// </summary>   
        /// <param name="Source">The srouce need to decrypto</param>   
        /// <returns>The result string after decrypto</returns>   
        public string Decrypto(string Source)
        {
            if(string.IsNullOrWhiteSpace(Source))
                return null;

            byte[] bytIn = System.Convert.FromBase64String(Source);
            try
            {
                string strRet = null;
                using (MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length))
                {
                    _objCryptoService.Key = GetLegalKey();
                    _objCryptoService.IV = GetLegalIV();
                    ICryptoTransform encrypto = _objCryptoService.CreateDecryptor();
                    using(CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            strRet = sr.ReadToEnd();
                            cs.Clear();
                            sr.Close();
                        }
                        cs.Close();
                    }
                    ms.Close();
                }
                return strRet;
            }
            catch (Exception )
            {
                return null;
            }
        }

        /// <summary>   
        /// Decrypto byte array
        /// </summary>   
        /// <param name="source">The srouce need to decrypto</param>   
        /// <returns>The result string after decrypto</returns>   
        public byte[] Decrypto(byte[] source)
        {
            if (source == null || source.Length == 0)
                return null;

            byte[] bytIn = source;
            byte[] bytOut = null;
            try
            {
                using (MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length))
                {
                    _objCryptoService.Key = GetLegalKey();
                    _objCryptoService.IV = GetLegalIV();
                    ICryptoTransform encrypto = _objCryptoService.CreateDecryptor();
                    using (CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read))
                    {
                        bytOut = new byte[bytIn.Length];
                        cs.Read(bytOut, 0, (int)bytOut.Length);
                        cs.Close();
                    }
                    ms.Close();
                }
                return bytOut;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

    /// <summary>
    /// 树形节点
    /// </summary>
    public class TreeViewNode : INotifyPropertyChanged
    {
        #region Properties

        [XmlIgnore]
        private string _name;
        /// <summary>
        /// 节点名称
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.OnPropertyChanged("Name");
            }
        }

        [XmlIgnore]
        private bool _isSelected = false;
        /// <summary>
        /// 是否选中
        /// </summary>
        [XmlAttribute]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        [XmlIgnore]
        private bool _isExpanded = false;
        /// <summary>
        /// 是否展开
        /// </summary>
        [XmlAttribute]
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");

                    // Expand all the way up to the root.
                    if (_isExpanded && _parent != null)
                        _parent.IsExpanded = true;
                }
            }
        }

        [XmlIgnore]
        private TreeViewNode _parent = null;
        /// <summary>
        /// 父节点
        /// </summary>
        [XmlIgnore]
        public TreeViewNode Parent { get { return _parent; } set { _parent = value; } }

        /// <summary>
        /// 子节点
        /// </summary>
        [XmlArray(ElementName = "Children")]     //Regions下面包含很多region
        [XmlArrayItem("Child")]
        public List<TreeViewNode> Children { get; private set; }

        [XmlIgnore]
        private BitmapImage _iconImage;
        /// <summary>
        /// 图标文件
        /// </summary>
        [XmlIgnore]
        public BitmapImage IconImage { get { return _iconImage; } set { _iconImage = value; OnPropertyChanged("IconImage"); } }

        #endregion // Properties

        #region TreeViewNode

        /// <summary>
        /// Treeview 构造函数
        /// </summary>
        public TreeViewNode()
        {
            Name = "TREEVIEW_ROOT";
            //IconImage = CommonMethod.LoadBitmapImage(@"/CommonLibrary;component/Images/TreeNodeImage_16.png");
            IconImage = CommonMethod.LoadImageFromInnerResource("CommonLibrary.Images.TreeNodeImage_16.png");
            Children = new List<TreeViewNode>();
            _isExpanded = false;
            _isSelected = false;
        }

        /// <summary>
        /// Treeview 构造函数
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        public TreeViewNode(TreeViewNode parent, string name)  //增加种类
        {
            this.Name = name;
            Children = new List<TreeViewNode>();
            _isExpanded = false;
            _isSelected = false;
            this.Parent = parent;            
            //IconImage  = CommonMethod.LoadBitmapImage(@"/CommonLibrary;component/Images/TreeNodeImage_16.png");
            IconImage = CommonMethod.LoadImageFromInnerResource("CommonLibrary.Images.TreeNodeImage_16.png");
            
            //IconImage.BeginInit();
            //BitmapImage bi = new BitmapImage();
            //bi.BeginInit();
            //IconImage.UriSource = new System.Uri(@"/CommonLibrary;component/Images/TreeNodeImage_16.png");
            //IconImage.EndInit();

        }

        /// <summary>
        /// 查找的字符串
        /// </summary>
        /// <param name="comparestr">查找的字符串</param>
        public TreeViewNode FindItem(string comparestr)
        {
            if (string.Compare(Name, comparestr, true) == 0)
                return this;

            if (Children == null)
                return null;

            foreach (TreeViewNode item in Children)
            {
                TreeViewNode found = item.FindItem(comparestr);
                if (found != null)
                    return found;
            }
            return null;
        }

        /// <summary>
        /// 重置当前节点下面子节点的父节点，构造树形列表
        /// </summary>
        /// <param name="curNode">当前节点(从根节点开始)</param>
        public void RefreshParent(TreeViewNode curNode)
        {
            if (curNode.Children == null || curNode.Children.Count == 0)
                return;

            foreach (var item in curNode.Children)
            {
                item.Parent = curNode;
                RefreshParent(item);
            }
        }

        /// <summary>
        /// 查找节点
        /// </summary>
        /// <param name="curNode">当前节点</param>
        /// <param name="name">查找的节点名称</param>
        /// <param name="fullMatch">是否全字匹配</param>
        /// <returns>查找到的节点</returns>
        virtual public TreeViewNode FindItem(TreeViewNode curNode, string name, bool fullMatch=true)
        {
            //全字匹配
            if (fullMatch ==true && String.Compare(curNode.Name, name, true) == 0)
                return curNode;

            //包含，为了忽略大小写，全部变成大写后再比较
            if (fullMatch == false && curNode.Name.ToUpper().Contains(name.ToUpper()))
                return curNode;

            if(curNode.Children == null || curNode.Children.Count == 0)
                return null;

            //在Children中查找
            foreach (var item in curNode.Children)
            {
                TreeViewNode tempnode = FindItem(item, name, fullMatch);
                if (tempnode != null)
                    return tempnode;
            }

            return null;
        }
        
        #endregion // MethodTreeNode

        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// PropertyChangedEventHandler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }

    /// <summary>
    /// 光谱区间
    /// </summary>
    public class region
    {
        /// <summary>
        /// firstX
        /// </summary>
        [XmlAttribute("firstX")]
        public double firstX { get; set; }

        /// <summary>
        /// lastX
        /// </summary>
        [XmlAttribute("lastX")]
        public double lastX { get; set; }

        /// <summary>
        /// region
        /// </summary>
        /// <param name="firstx"></param>
        /// <param name="lastx"></param>
        public region(double firstx, double lastx)
        {
            firstX = firstx;
            lastX = lastx;
        }

        /// <summary>
        /// region
        /// </summary>
        public region()
        {
        }

        /// <summary>
        /// value in this region
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool inRegion(double value)
        {
            if (firstX < lastX)
                return value < firstX || value > lastX;
            else
                return value < lastX || value > firstX;
        }
    }

    /// <summary>
    /// 阈值区间
    /// </summary>
    public class thresold
    {
        /// <summary>
        /// minimum
        /// </summary>
        [XmlAttribute("minimum")]
        public double minimum { get; set; }

        /// <summary>
        /// maximum
        /// </summary>
        [XmlAttribute("maximum")]
        public double maximum { get; set; }

        /// <summary>
        /// thresold
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public thresold(double min, double max)
        {
            minimum = min;
            maximum = max;
        }

        /// <summary>
        /// thresold
        /// </summary>
        public thresold()
        {
            minimum = double.MinValue;
            maximum = double.MaxValue;
        }

        /// <summary>
        /// value is ok
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool valueOk(double value)
        {
            return value >= minimum && value <= maximum;
        }

        /// <summary>
        /// the min and max is validate
        /// </summary>
        /// <returns></returns>
        public bool validate()
        {
            return minimum < maximum;
        }
    }

}
