using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFrame.auto
{
    public class LengthEncoding
    {
        /// <summary>
        /// 粘包长度编码
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static byte[] Encode(byte[] buff)
        {
            MemoryStream ms = new MemoryStream();//创建内存流对象
            BinaryWriter sw = new BinaryWriter(ms);//写入二进制对象流
            sw.Write(buff.Length);
            sw.Write(buff);
            byte[] result = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, (int)ms.Length);
            sw.Close();
            ms.Close();
            return result;
        }

        /// <summary>
        /// 粘包长度解码
        /// </summary>
        /// <returns></returns>
        public static byte[] Decode(ref List<byte> cache)
        {
            //粘包的时候规定了长度为int型，占四个字节
            if (cache.Count < 4) return null;

            //创建内存流对象，并将缓存数据写入进去
            MemoryStream ms = new MemoryStream(cache.ToArray());
            //二进制读取流
            BinaryReader br = new BinaryReader(ms);
            int length = br.ReadInt32();//从缓存中读取int型消息长度
            //如果消息体长度 大于缓存中数据长度 说明消息没有都去完 等待下次消息到达后再次处理
            if (length > ms.Length - ms.Position)
            {
                return null;
            }
            //通过以上的循环后，读取正确长度的数据
            byte[] result = br.ReadBytes(length);
            //清空缓存
            cache.Clear();
            //将读取后的剩余数据写入缓存
            cache.AddRange(br.ReadBytes((int)(ms.Length - ms.Position)));
            br.Close();
            ms.Close();
            return result;
        }
    }
}
