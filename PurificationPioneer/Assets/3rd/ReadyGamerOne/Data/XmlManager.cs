using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ReadyGamerOne.Data
{
    /// <summary>
    /// Xml序列化管理类
    /// </summary>
    public static class XmlManager
    {

        
        public static T LoadData<T>(string fullPath) where T : class
        {
            StreamReader r = File.OpenText(fullPath);//_FileLocation是unity3D当前project的路径名，_FileName是xml的文件名。定义为成员变量了
                                                     //当然，你也可以在前面先判断下要读取的xml文件是否存在
            var _data = r.ReadLine();
            //			Debug.Log(_data);
            var myData = DeserializeObject<T>(Encoding.UTF8.GetBytes(_data));//myData是上面自定义的xml存取过程中要使用的数据结构UserData
            r.Close();
            return myData;
        }

        public static void SaveData<T>(T data, string fullPath) where T : class
        {
            StreamWriter writer;
            FileInfo t = new FileInfo(fullPath);
            t.Delete();
            writer = t.CreateText();
            var _data = SerializeObject(data); //序列化这组数据
            writer.WriteLine(Encoding.UTF8.GetString(_data)); //写入xml
            writer.Close();
        }


        // Here we serialize our UserData object of myData
        public static byte[] SerializeObject<T>(T pObject) where T : class
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(typeof(T));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xs.Serialize(xmlTextWriter, pObject);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream; // (MemoryStream)
            return memoryStream.ToArray();
        }

        // Here we deserialize it back into its original form
        public static T DeserializeObject<T>(byte[] pXmlizedString) where T : class
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream((pXmlizedString));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            return xs.Deserialize(memoryStream) as T;
        }

    }
}
