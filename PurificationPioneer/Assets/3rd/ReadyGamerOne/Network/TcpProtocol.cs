using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.Network
{
    public static class TcpProtocol
    {
        private const int HeadSize = 2;
        
        /// <summary>
        /// 将byte数组打包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Pack(byte[] data)
        {
            var len = data.Length;

            Assert.IsTrue(len <= 65535 - 2);

            var cmdLen = len + HeadSize;
            var cmd = new byte[cmdLen];
            cmd.WriteUShortLe((ushort)cmdLen);
            cmd.WriteBytes(data, HeadSize);

            return cmd;
        }

        /// <summary>
        /// 读取Tcp包头信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataLen"></param>
        /// <param name="pkgSize"></param>
        /// <param name="headSize"></param>
        /// <returns></returns>
        public static bool ReadHeader(byte[] data, int dataLen, out int pkgSize, out int headSize, int offset=0)
        {
            pkgSize = 0;
            headSize = 0;
            if (dataLen < 2)
                return false;

            pkgSize = data.ReadUShortLe(offset);
            headSize = 2;
            return true;
        }
    }
}