using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame
{
    public class ServerStart
    {
        Socket server;//服务器socket监听对象
        int maxClient;//最大客户端连接数
        UserTokenPool pool;
        Semaphore acceprClients;//为了防止两个连接一起进来的时候冲突

        public LengthEncode LE;
        public LengthDecode LD;
        public Encode encode;
        public Decode decode;

        /// <summary>
        /// 消息处理中心，由外部应用传入
        /// </summary>
        public AbsHandlerCenter center;

        /// <summary>
        /// 初始化通信监听
        /// </summary>
        /// <param name="max">最大连接数</param>
        public ServerStart(int max)
        {
            //实例化监听对象
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //设定服务器最大连接人数
            maxClient = max;
            
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="port">端口</param>
        public void Start(int port)
        {
            //创建连接池
            pool = new UserTokenPool(maxClient);
            //连接信号量--这个的作用不是很清楚
            acceprClients = new Semaphore(maxClient, maxClient);
            for (int i = 0; i < maxClient; i++)//把max最大的连接数初始化出来
            {
                UserToken token = new UserToken();

                //初始化token信息
                token.receiveSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Comleted);
                token.sendSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Comleted);
                token.LD = LD;
                token.LE = LE;
                token.encode = encode;
                token.decode = decode;
                token.sendProcess = ProcessSend;
                token.closeProcess = ClientClosse;
                token.center = center;
                pool.Ppush(token);
            }
            //监听当前服务器网卡所有可用IP地址的port端口
            //外网IP 内网IP192.168.x.x 本机IP127.0.0.1
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            //全满了再监听10个，仅置于监听状态
            server.Listen(10);
        }

        /// <summary>
        /// 开始客户端连接监听
        /// </summary>
        /// <param name="e">.net封装好的异步监听事件</param>
        public void StartAccept(SocketAsyncEventArgs e)
        {
            //如果当前传入为空 说明调用新的客户端连接监听事件 否则移除当前客户端连接
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Comleted);
            }
            else
            {
                e.AcceptSocket = null;
            }
            //信号量-1
            acceprClients.WaitOne();
            bool result= server.AcceptAsync(e);
            //判断异步事件是否挂起 没挂起说明立刻执行完成 直接处理事件 否则会在处理完成后触发Accept_Comleted事件
            if (!result)
            {
                ProcessAccept(e);
            }
        }

        //处理连接事件
        public void ProcessAccept(SocketAsyncEventArgs e)
        {
            //从对象池取出连接对象 供新用户使用
            UserToken token = pool.Ppop();
            token.conn = e.AcceptSocket;
            //TODO 通知应用层 有客户端连接
            center.ClientConnect(token);
            //开启消息到达监听
            StartReceive(token);
            //释放当前异步对象 其中的e.AcceptSocket = null;可以释放
            StartAccept(e);
        }

        //异步监听完成事件
        public void Accept_Comleted(object sender,SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        //消息到达监听
        public void StartReceive(UserToken token)
        {
            //用户连接对象 开启异步数据接收
            bool result= token.conn.ReceiveAsync(token.receiveSAEA);
            //异步事件是否挂起
            if (!result)//没有的话 直接挂起
            {
                ProcessReceive(token.receiveSAEA);
            }
        }

        //收发完成事件
        public void IO_Comleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                ProcessReceive(e);
            }
            else
            {
                ProcessSend(e);
            }
        }

        public void ProcessReceive(SocketAsyncEventArgs e)
        {
            UserToken token = e.UserToken as UserToken;
            //判断网络消息接收是否成功
            if (token.receiveSAEA.BytesTransferred > 0 && token.receiveSAEA.SocketError == SocketError.Success)
            {
                byte[] message = new byte[token.receiveSAEA.BytesTransferred];
                //把接收到的消息拷贝到数组中
                Buffer.BlockCopy(token.receiveSAEA.Buffer, 0, message, 0, token.receiveSAEA.BytesTransferred);
                //处理接收到的消息
                token.Receive(message);
                //继续监听接收事件
                StartReceive(token);
            }
            else
            {   //满足下面条件为客户端异常断开
                if(token.receiveSAEA.SocketError!=SocketError.Success)
                {
                    ClientClosse(token, token.receiveSAEA.SocketError.ToString());
                }
                else
                {
                    //消息长度为0，没有报错，说明客户端主动断开连接了
                    ClientClosse(token, "客户端主动断开连接");
                }
            }
        }

        public void ProcessSend(SocketAsyncEventArgs e)
        {
            UserToken token = e.UserToken as UserToken;
            if (e.SocketError != SocketError.Success)
            {//如果报错，则直接关闭客户端
                ClientClosse(token, e.SocketError.ToString());
            }
            else
            {
                //消息发送成功，回调成功
            }
        }

        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <param name="token">断开连接的用户对象</param>
        /// <param name="error">断开连接的错误编码</param>
        public void ClientClosse(UserToken token,string error)
        {
            if (token.conn != null)//不为空时为有效信号量
            {
                lock (token)
                {
                    //通知应用层面 客户端端口连接
                    center.ClientClose(token, error);
                    token.Close();
                    //先塞回去，再加一个信号量，供其它用户使用
                    pool.Ppush(token);
                    acceprClients.Release();
                }
            }
        }
    }
}
