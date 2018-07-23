using RedisDemo.Common;
using System;

namespace Redis发布
{
    class Program
    {
        static void Main(string[] args)
        {
            RedisHelper redisHelper = new RedisHelper();
            Console.WriteLine("请输入你要发送的消息(输入exit表示退出)：");
            string msg = string.Empty;
            long clientCount = 0;
            while (!(msg = Console.ReadLine()).Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                clientCount = redisHelper.Publish("mq", msg);
                if (clientCount > 0)
                {
                    Console.WriteLine("有客户端接收到了该消息");
                }
                else
                {
                    Console.WriteLine("没有客户端接收该消息");
                }
            }

            Console.ReadKey();
        }
    }
}
