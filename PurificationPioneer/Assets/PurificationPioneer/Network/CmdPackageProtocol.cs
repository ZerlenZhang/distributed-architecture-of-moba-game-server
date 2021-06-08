using System;
using System.IO;
using System.Text;
using ProtoBuf;
using ReadyGamerOne.Utility;

namespace PurificationPioneer.Network
{
    /// <summary>
    /// 命令包协议
    ///  2字节 -- serviceType
    ///  2字节 -- cmdType
    ///  4字节 -- userData
    ///  后面  -- body
    /// </summary>
    public static class CmdPackageProtocol
    {
        public class CmdPackage
        {
            public int serviceType;
            public int cmdType;
            public byte[] body;
        }

        private const int HeadSize = 8;
        
        /// <summary>
        /// 打包Protobuf
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="cmdType"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] PackageProtobuf(int serviceType, int cmdType, IExtensible msg)
        {
            var len = HeadSize;
            byte[] body = null;
            if (msg != null)
            {
                body = ProtobufSerizer(msg);
                len += body.Length;
            }

            var cmdPackage = new byte[len];
            
            cmdPackage.WriteUShortLe((ushort)serviceType);
            cmdPackage.WriteUShortLe((ushort) cmdType, 2);
            if (body != null)
                cmdPackage.WriteBytes(body, HeadSize);
            
            return cmdPackage;
        }

        /// <summary>
        /// 从byte数组解析出CmdPackage
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="cmdLen"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool UnpackProtobuf(byte[] data, int start, int cmdLen, out CmdPackage msg)
        {
            msg = new CmdPackage {serviceType = data.ReadUShortLe(start), cmdType = data.ReadUShortLe(start + 2)};

            var bodyLen = cmdLen - HeadSize;
            msg.body = new byte[bodyLen];
            Array.Copy(data, start + HeadSize, msg.body, 0, bodyLen);
            return true;
        }

        public static T ProtobufDeserialize<T>(byte[] data)
        {
            using (var m = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(m);
            }
        }
        
        /// <summary>
        /// 打包Json
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="cmdType"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static byte[] PackageJson(int serviceType, int cmdType, string jsonStr)
        {
            var len = HeadSize;
            byte[] body = null;
            if (!string.IsNullOrEmpty(jsonStr))
            {
                body = Encoding.UTF8.GetBytes(jsonStr);
                len += body.Length;
            }

            var cmdPackage = new byte[len];
            
            cmdPackage.WriteUShortLe((ushort)serviceType);
            cmdPackage.WriteUShortLe((ushort) cmdType, 2);
            if (body != null)
                cmdPackage.WriteBytes(body, HeadSize);
            return cmdPackage;
        }


        private static byte[] ProtobufSerizer(IExtensible data)
        {
            using (var m = new MemoryStream())
            {
                Serializer.Serialize(m, data);
                m.Position = 0;
                var length = (int)m.Length;
                var buffer = new byte[length];
                m.Read(buffer, 0, length);
                return buffer;
            }            
        }
    }
}