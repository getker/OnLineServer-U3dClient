using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFrame
{
    //长度的编码解码
    public delegate byte[] LengthEncode(byte[] value);
    public delegate byte[] LengthDecode(ref List<byte> value);
    
    //消息体的编码解码
    public delegate byte[] Encode(object value);
    public delegate byte[] Decode(byte[] value);
}
