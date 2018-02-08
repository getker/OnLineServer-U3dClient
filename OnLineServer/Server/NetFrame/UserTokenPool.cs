using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFrame
{
    public class UserTokenPool
    {
        private Stack<UserToken> pool;
        
        public UserTokenPool(int max)//构造函数
        {
            pool = new Stack<UserToken>(max);
        }

        // 取出一个连接对象--创建连接
        public UserToken Ppop()
        {
            return pool.Pop();
        }

        //插入一个连接对象--释放连接
        public void Ppush(UserToken token)
        {
            if (token != null)
                pool.Push(token);
        }

        //用于获取当前剩余池的大小
        public int Size
        {
            get { return pool.Count; }
        }
    }
}
