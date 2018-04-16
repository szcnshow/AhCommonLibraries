using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Reflection;

namespace Ai.Hong.Common
{
    /// <summary>
    /// 资源操作类
    /// </summary>
    public static class ResourceOperator
    {
        public static string ErrorString = null;

        /// <summary>
        /// 字符串资源
        /// </summary>
        /// <param name="app">当前执行程序</param>
        /// <param name="key">资源的Key</param>
        /// <returns></returns>
        public static string ResourceString(Application app, string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return null;

                return (string)app.Resources[key];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 图像资源BitmapImage
        /// </summary>
        /// <param name="app">当前执行程序</param>
        /// <param name="key">资源的Key</param>
        /// <param name="imagePath">图像的根目录</param>
        public static BitmapImage ResourceBitmap(Application app, string key, string imagePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return null;
                string imagefile = ResourceString(app, key);

                BitmapImage bmp = new BitmapImage(new Uri(imagePath + imagefile, UriKind.RelativeOrAbsolute));

                return bmp;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 资源中的DrawingGroup
        /// </summary>
        /// <param name="app">当前执行程序</param>
        /// <param name="key">资源的Key</param>
        /// <returns></returns>
        public static DrawingGroup ResourceVector(Application app, string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return null;

                return (DrawingGroup)app.Resources[key];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 资源中的Brush
        /// </summary>
        /// <param name="app">当前执行程序</param>
        /// <param name="key">资源的Key</param>
        /// <returns></returns>
        public static Brush ResourceBrush(Application app, string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return null;
                return (Brush)app.Resources[key];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 资源中的SolidColorBrush
        /// </summary>
        /// <param name="app">当前执行程序</param>
        /// <param name="key">资源的Key</param>
        /// <returns></returns>
        public static SolidColorBrush ResourceSolidBrush(Application app, string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return null;
                return (SolidColorBrush)app.Resources[key];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 资源中的Style
        /// </summary>
        /// <param name="app">当前执行程序</param>
        /// <param name="key">资源的Key</param>
        /// <returns></returns>
        public static Style ResourceStyle(Application app, string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return null;
                return (Style)app.Resources[key];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取嵌入的资源
        /// </summary>
        /// <param name="owner">资源的所有者</param>
        /// <param name="resourcePath">资源路径</param>
        /// <returns></returns>
        public static System.IO.Stream EmbededResourceStream(Assembly owner, string resourcePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resourcePath) || owner == null)
                    return null;

                return owner.GetManifestResourceStream(resourcePath);
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 加载Xaml对象
        /// </summary>
        /// <param name="assemb">当前调用进程的Assembly</param>
        /// <param name="resourceKey">资源路径</param>
        /// <returns>Xaml资源对象</returns>
        public static object EmbededResourceElement(Assembly assemb, string resourceKey)
        {
            System.IO.Stream xamlStream = null;

            try
            {
                xamlStream = EmbededResourceStream(assemb, resourceKey);
                if (xamlStream == null)
                    return null;

                var el = System.Windows.Markup.XamlReader.Load(xamlStream);
                xamlStream.Close();

                return el;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
            finally
            {
                if (xamlStream != null)
                    xamlStream.Close();
            }
        }

        /// <summary>
        /// 加载Binary对象
        /// </summary>
        /// <param name="assemb">当前调用进程的Assembly</param>
        /// <param name="resourceKey">资源路径</param>
        /// <returns>Binary资源对象</returns>
        public static byte[] EmbededResourceBinary(Assembly assemb, string resourceKey)
        {
            System.IO.Stream binStream = null;

            try
            {
                binStream = EmbededResourceStream(assemb, resourceKey);
                if (binStream == null)
                    return null;

                byte[] retData = new byte[binStream.Length];

                if (binStream.Read(retData, 0, retData.Length) != retData.Length)
                    retData = null;

                binStream.Close();

                return retData ;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
            finally
            {
                if (binStream != null)
                    binStream.Close();
            }
        }

        /// <summary>
        /// 加载Text对象
        /// </summary>
        /// <param name="assemb">当前调用进程的Assembly</param>
        /// <param name="resourceKey">资源路径</param>
        /// <returns>Text资源对象</returns>
        public static string EmbededResourceText(Assembly assemb, string resourceKey)
        {
            System.IO.Stream binStream = null;

            try
            {
                binStream = EmbededResourceStream(assemb, resourceKey);
                if (binStream == null)
                    return null;

                string retData = null;

                System.IO.StreamReader reader = new System.IO.StreamReader(binStream);
                if (reader != null)
                {
                    retData = reader.ReadToEnd();
                    reader.Close();
                }
                binStream.Close();

                return retData;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
            finally
            {
                if (binStream != null)
                    binStream.Close();
            }
        }
    }
}
