using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ai.Hong
{
    public static class CommonMethod
    {
        public static string ErrorString = null;

        //将类序列化到字符串
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

        //从字符串中反序列化类
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
    }
}
