using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ACloudSecurity
{
    
    /// <summary>
    /// 自定义读取流
    /// </summary>
    internal class ZipStreamReader:Stream
    {
        long startPos;//结束位置
        Stream innerStream;

        /// <summary>
        /// 直接使用原始流。
        /// </summary>
        /// <param name="startPos">流的起始位置</param>
        /// <param name="stream">原始流</param>
        public ZipStreamReader(Stream stream, long startPos)
        {
            this.innerStream = stream;
            this.startPos = startPos;
        }

        /// <summary>
        /// 自定义流剩余长度。
        /// </summary>
        public override long Length
        {
            get { return innerStream.Length - startPos; }
        }

        /// <summary>
        /// 自定义流位置，返回原始流的位置
        /// </summary>
        public override long Position
        {
            get
            {
                return innerStream.Position - startPos;
            }
            set
            {
                innerStream.Position = value + startPos;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    return innerStream.Seek(startPos + offset, origin);
                case SeekOrigin.Current:
                    return innerStream.Seek(offset, origin);
                case SeekOrigin.End:
                    return innerStream.Seek(offset, origin);
                default:
                    return startPos;
            }
        }

        public override void SetLength(long value)
        {
            return;
            //throw new NotImplementedException();
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override bool CanRead
        {
            get { return innerStream.CanRead;  }
        }

        public override bool CanSeek
        {
            get { return innerStream.CanSeek; }
        }

        public override bool CanTimeout
        {
            get
            {
                return innerStream.CanTimeout;
            }
        }

        public override bool CanWrite
        {
            get { return innerStream.CanWrite; }
        }

        public override void Close()
        {
            innerStream.Close();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return innerStream.Read(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            innerStream.WriteByte(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            innerStream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            innerStream.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override int ReadByte()
        {
            return innerStream.ReadByte();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return innerStream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            innerStream.EndWrite(asyncResult);
        }
        public override int ReadTimeout
        {
            get
            {
                return innerStream.ReadTimeout;
            }
            set
            {
                innerStream.ReadTimeout = value;
            }
        }
        public override int WriteTimeout
        {
            get
            {
                return innerStream.WriteTimeout;
            }
            set
            {
                innerStream.WriteTimeout = value;
            }
        }
    }
 
    /// <summary>
    /// 压缩文件接口
    /// </summary>
    public class DataCompressFile
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorString;

        private const int fileMark = 0x504D4F43;    //压缩文件标志 COMP

        /// <summary>
        /// 磁盘文件
        /// </summary>
        private FileStream diskFileStream = null;

        /// <summary>
        /// 压缩算法接口
        /// </summary>
        protected Ionic.Zip.ZipFile zipObject = null;

        /// <summary>
        /// 压缩数据起始位置
        /// </summary>
        private int zipDataStartPos = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataCompressFile()
        {
        }

        /// <summary>
        /// 创建压缩文件
        /// </summary>
        /// <param name="diskFile">压缩文件名</param>
        public bool Create(string diskFile)
        {
            if (diskFileStream != null || zipObject != null)
            {
                ErrorString = "Please close current instance before create";
                return false;
            }

            if (string.IsNullOrWhiteSpace(diskFile))
            {
                ErrorString = "Invalid parameter";
                return false;
            }

            try
            {

                diskFileStream = new FileStream(diskFile, FileMode.Create, FileAccess.Write);
                zipObject = new Ionic.Zip.ZipFile();
                zipObject.AlternateEncoding = Encoding.UTF8;
                zipObject.AlternateEncodingUsage = Ionic.Zip.ZipOption.Always;
                zipObject.ParallelDeflateThreshold = -1;    //不需要多线程压缩

                zipDataStartPos = 0;

                //写入压缩文件标志(INT)
                byte[] dataBytes = BitConverter.GetBytes(fileMark);
                diskFileStream.Write(dataBytes, 0, dataBytes.Length);
                zipDataStartPos += dataBytes.Length;

                return true;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 添加文件到压缩文件
        /// </summary>
        /// <param name="sourceFile">要添加的文件名</param>
        /// <param name="zipedName">目标文件名</param>
        /// <param name="encryptKey">加密密码（优先检查）, NULL表示不加密</param>
        /// <param name="licenseKeyType">授权密码类型,0表示不加密</param>
        public bool AddFile(string sourceFile, string zipedName, string encryptKey, int licenseKeyType)
        {
            if (zipObject == null)
            {
                ErrorString = "Create function must called first";
                return false;
            }

            if (string.IsNullOrWhiteSpace(sourceFile) || string.IsNullOrWhiteSpace(zipedName))
            {
                ErrorString = "Invalid parameter";
                return false;
            }

            FileStream itemStream = null;
            try
            {
                //读入原始文件
                FileInfo tempInfo = new FileInfo(sourceFile);
                byte[] srcBytes = new byte[tempInfo.Length];

                //打开文件
                itemStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
                //读取全部数据
                if (itemStream.Read(srcBytes, 0, srcBytes.Length) != srcBytes.Length)
                    throw new Exception("File read error:" + sourceFile);
                itemStream.Close();

                return AddBytes(srcBytes, zipedName, encryptKey, licenseKeyType);
            }
            catch (Exception ex)
            {
                if (itemStream != null)
                    itemStream.Close();

                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 向压缩文件中添加字符串
        /// </summary>
        /// <param name="strContent">要添加的字符串</param>
        /// <param name="zipedName">目标文件名</param>
        /// <param name="encryptKey">加密密码（优先检查）, NULL表示不加密</param>
        /// <param name="licenseKeyType">授权密码类型,0表示不加密</param>
        public bool AddString(string strContent, string zipedName, string encryptKey, int licenseKeyType)
        {
            if (zipObject == null)
            {
                ErrorString = "Create function must called first";
                return false;
            }

            if (strContent == null || string.IsNullOrWhiteSpace(zipedName))
            {
                ErrorString = "Invalid Parameters";
                return false;
            }

            byte[] srcBytes = Encoding.UTF8.GetBytes(strContent);
            if (srcBytes == null)
            {
                ErrorString = "String to Byte Error";
                return false;
            }

            return AddBytes(srcBytes, zipedName, encryptKey, licenseKeyType);
        }

        /// <summary>
        /// 向压缩文件中添加Byte[]
        /// </summary>
        /// <param name="srcBytes">要添加的数据</param>
        /// <param name="zipedName">目标文件名</param>
        /// <param name="encryptKey">加密密码（优先检查）, NULL表示不加密</param>
        /// <param name="licenseKeyType">授权密码类型,0表示不加密</param>
        public bool AddBytes(byte[] srcBytes, string zipedName, string encryptKey, int licenseKeyType)
        {
            if (zipObject == null)
            {
                ErrorString = "Create function must called first";
                return false;
            }
            if (srcBytes == null || string.IsNullOrWhiteSpace(zipedName))
            {
                ErrorString = "Invalid Parameters";
                return false;
            }

            try
            {
                //添加到压缩文件中
                byte[] encodeBytes = null;
                if (!string.IsNullOrWhiteSpace(encryptKey) || licenseKeyType > 0)
                {
                    if (!string.IsNullOrWhiteSpace(encryptKey))
                        encodeBytes = Security.EncryptData(srcBytes, encryptKey);
                    else
                        encodeBytes = Security.EncryptDataByLicenseKey(srcBytes, licenseKeyType);

                    if (encodeBytes == null)
                    {
                        ErrorString = Security.ErrorString;
                        return false;
                    }
                }
                else
                    encodeBytes = srcBytes;

                Ionic.Zip.ZipEntry entry = zipObject.AddEntry(zipedName, srcBytes);
                return true;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 关闭并保存压缩文件
        /// </summary>
        public bool Close()
        {
            try
            {
                if (diskFileStream.CanWrite)
                {
                    ZipStreamReader zipStream = new ZipStreamReader(diskFileStream, zipDataStartPos);
                    zipObject.Save(zipStream);
                    zipStream.Close();
                }

                zipObject.Dispose();
                diskFileStream.Close();

                zipObject = null;
                diskFileStream = null;

                return true;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 打开压缩文件
        /// </summary>
        /// <param name="diskFile">压缩文件名</param>
        /// <returns></returns>
        public bool Open(string diskFile)
        {
            if (diskFileStream != null || zipObject != null)
            {
                ErrorString = "Please close current instance before open.";
                return false;
            }

            if (diskFile == null || !File.Exists(diskFile))
            {
                ErrorString = "Cannot find file:" + diskFile;
                return false;
            }

            ErrorString = null;
            
            try
            {
                //打开压缩文件流
                diskFileStream = new FileStream(diskFile, FileMode.Open, FileAccess.Read);

                zipDataStartPos = 0;
                //读取并检查文件标志（4Byte)
                byte[] dataBytes = new byte[4];
                if (diskFileStream.Read(dataBytes, 0, dataBytes.Length) != dataBytes.Length)
                    throw new Exception("File read error");

                //检查压缩文件标志
                int mark = BitConverter.ToInt32(dataBytes, 0);
                if (mark != fileMark)
                    throw new Exception("Invalid file format");

                zipDataStartPos += 4;

                ZipStreamReader zipStream = new ZipStreamReader(diskFileStream, zipDataStartPos);
                zipObject = Ionic.Zip.ZipFile.Read(zipStream);
                if (zipObject == null)
                    throw new Exception("Invalid file format");

                zipObject.AlternateEncoding = Encoding.UTF8;
                zipObject.AlternateEncodingUsage = Ionic.Zip.ZipOption.Always;

                return true;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 解压一个文件
        /// </summary>
        /// <param name="zipedFileName">压缩文件中的文件名</param>
        /// <param name="encryptKey">加密密码（优先检查）, NULL表示不加密</param>
        /// <param name="licenseKeyType">授权密码类型,0表示不加密</param>
        /// <returns>解压缩的文件内容</returns>
        public byte[] ReadBytes(string zipedFileName, string encryptKey, int licenseKeyType)
        {
            if (zipObject == null)
            {
                ErrorString = "Open function must called first";
                return null;
            }

            try
            {
                //查找指定的文件
                Ionic.Zip.ZipEntry foundEntry = zipObject[zipedFileName];
                if (foundEntry == null || foundEntry.IsDirectory)
                    throw new Exception("Cannot find file");

                //创建MemoryStream，用于解压文件
                byte[] srcBytes = new byte[foundEntry.UncompressedSize];
                MemoryStream stream = new MemoryStream(srcBytes, 0, srcBytes.Length);
                foundEntry.Extract(stream);
                stream.Close();

                //添加到压缩文件中
                byte[] decodeBytes = null;
                if (!string.IsNullOrWhiteSpace(encryptKey) || licenseKeyType > 0)
                {
                    if (!string.IsNullOrWhiteSpace(encryptKey))
                        decodeBytes = Security.DecryptData(srcBytes, encryptKey, null);
                    else
                        decodeBytes = Security.DecryptDataByLicenseKey(srcBytes, licenseKeyType);

                    if (decodeBytes == null)
                    {
                        ErrorString = Security.ErrorString;
                        return null;
                    }
                }
                else
                    decodeBytes = srcBytes;

                return srcBytes;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }

        }

        /// <summary>
        /// 读取压缩文件并保存到磁盘
        /// </summary>
        /// <param name="zipedFile">压缩文件中的文件名</param>
        /// <param name="saveFile">要保存的文件名</param>
        /// <param name="encryptKey">加密密码（优先检查）, NULL表示不加密</param>
        /// <param name="licenseKeyType">授权密码类型,0表示不加密</param>
        public bool ReadFile(string zipedFile, string saveFile, string encryptKey, int licenseKeyType)
        {
            if (zipObject == null)
            {
                ErrorString = "Open function must called first";
                return false;
            }

            if (string.IsNullOrWhiteSpace(zipedFile) || string.IsNullOrWhiteSpace(saveFile))
            {
                ErrorString = "Invalid parameter";
                return false;
            }

            byte[] fileBytes = ReadBytes(zipedFile, encryptKey, licenseKeyType);
            if (fileBytes == null || fileBytes.Length == 0)
                return false;

            try
            {
                FileStream writeStream = new FileStream(saveFile, FileMode.Create, FileAccess.Write);
                writeStream.Write(fileBytes, 0, fileBytes.Length);
                writeStream.Close();

                return true;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 读取压缩文件中的文本文件
        /// </summary>
        /// <param name="zipedFile">压缩文件中的文件名</param>
        /// <param name="encryptKey">加密密码（优先检查）, NULL表示不加密</param>
        /// <param name="licenseKeyType">授权密码类型,0表示不加密</param>
        public string ReadString(string zipedFile, string encryptKey, int licenseKeyType)
        {
            if (zipObject == null)
            {
                ErrorString = "Open function must called first";
                return null;
            }

            if (string.IsNullOrWhiteSpace(zipedFile))
            {
                ErrorString = "Invalid parameter";
                return null;
            }

            byte[] fileBytes = ReadBytes(zipedFile, encryptKey, licenseKeyType);
            if (fileBytes == null || fileBytes.Length == 0)
                return null;

            try
            {
                string retstr = Encoding.UTF8.GetString(fileBytes);
                return retstr;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~DataCompressFile()
        {
            Close();
        }
    }

}
