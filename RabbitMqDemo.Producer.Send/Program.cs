using System;

namespace RabbitMqDemo.Producer.Send
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1.Hello World
            //new _1_HelloWorld().Send();

            // 2.Work Queue
            //new _2_WorkQueue().Send();

            // 3.Publish/Subscribe
            //new _3_Publish_Subscribe().Send();

            // 4.Routing
            //new _4_Routing().Send();

            // 5.Topics
            //new _5_Topics().Send();

            // 6.Rpc
            new _6_RpcClient().Start();

            Console.ReadKey();
        }
    }
}
