using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ReadyGamerOne.Data
{
    public static class BitManager
    {
        public static T LoadData<T>(string fullPath) where T : class
        {
            StreamReader r = File.OpenText(fullPath);//_FileLocation是unity3D当前project的路径名，_FileName是xml的文件名。定义为成员变量了
                                                     //当然，你也可以在前面先判断下要读取的xml文件是否存在
            String _data = r.ReadLine();
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
                                                              //			writer.WriteLine(_data); //写入xml
            writer.Close();
        }

        public static byte[] SerializeObject<T>(T pObject) where T : class
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(memoryStream, pObject);
            byte[] data = memoryStream.ToArray();
            memoryStream.Close();
            return data;
        }

        // Here we deserialize it back into its original form
        public static T DeserializeObject<T>(byte[] aSerializeData) where T : class
        {
            MemoryStream stream = new MemoryStream(aSerializeData);
            var obj = default(T);
            BinaryFormatter bf = new BinaryFormatter();
            obj = (T)bf.Deserialize(stream);
            stream.Close();

            return obj;
        }
    }
}
