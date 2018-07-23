using RedisDemo.Common;
using System;

namespace Redis订阅
{
    class Program
    {
        static void Main(string[] args)
        {
            RedisHelper redisHelper = new RedisHelper();

            redisHelper.Subscribe("mq", (chanel, msg) => {
                Console.WriteLine($"订阅客户端收到消息：{msg}");
            });

            Console.ReadKey();
        }
    }
}
