using System;

namespace ReadyGamerOne.Utility
{
    public static class ByteArrayExtension
    {
        /// <summary>
        /// 以小尾巴形式在byte数组写入ushort类型数据
        /// </summary>
        /// <param name="self"></param>
        /// <param name="offset">偏移</param>
        /// <param name="value"></param>
        public static void WriteUShortLe(this byte[] self,ushort value, int offset=0)
        {
            //此函数运行结果是小尾巴，还是大尾巴，取决于系统
            var byteValue = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteValue);
            }

            Array.Copy(byteValue, 0, self, offset, byteValue.Length);

        }

        /// <summary>
        /// 以小尾巴形式在byte数组写入uint类型数据
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void WriteUIntLe(this byte[] self,  uint value, int offset=0)
        {
            //此函数运行结果是小尾巴，还是大尾巴，取决于系统
            var byteValue = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteValue);
            }

            Array.Copy(byteValue, 0, self, offset, byteValue.Length);

        }


        /// <summary>
        /// 将另一个byte数组写入到当前数组
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void WriteBytes(this byte[] self, byte[] value, int offset = 0)
        {
            Array.Copy(value, 0, self, offset, value.Length);
        }

        /// <summary>
        /// 以小尾巴的形式从数组中读取出一个两字节数据（ushort)
        /// </summary>
        /// <param name="self"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ushort ReadUShortLe(this byte[] self, int offset = 0)
        {
            var ret = self[offset] | (self[offset+1] << 8);
            return (ushort) ret;
        }
    }

}