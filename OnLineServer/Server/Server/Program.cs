using NetFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //服务器初始化

            #region 同步的例子
            //Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //server.Bind(new IPEndPoint(IPAddress.Any, 123));
            ////置于监听状态
            //server.Listen(10);
            //Socket client = server.Accept();
            //byte[] buff = new byte[1024];
            //client.Receive(buff);
            //client.Send(buff);
            #endregion

            ServerStart ss = new ServerStart(9000);
            ss.Start(6666);

        }
    }
}
