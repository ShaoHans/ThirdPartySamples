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
                Console.WriteLine($"有{clientCount}个客户端接收到该消息");
            }

            Console.ReadKey();
        }
    }
}
