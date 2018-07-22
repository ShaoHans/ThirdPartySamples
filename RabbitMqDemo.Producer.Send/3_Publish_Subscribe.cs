using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMqDemo.Producer.Send
{
    public class _3_Publish_Subscribe
    {
        public void Send()
        {
            using (var connection = RabbitMqDemo.Common.Helper.CreateConnection(Common.ConnectionType.Cluster)) // 1.创建连接
            using (var channel = connection.CreateModel())      // 2.建立通道
            {
                // 消息生产者把消息发送到指定的交换器，再由交换器发给与之绑定的队列
                // 3.定义交换器
                string exchangeName = "sms";
                channel.ExchangeDeclare(exchange: exchangeName,
                    type: "fanout", // 交换器类别：fanout表示多播，消息会被发送到所有队列
                    durable: true,
                    autoDelete: false,
                    arguments: null);

                Console.WriteLine("请输入要发送的文字信息，输入exit退出！");
                string message = string.Empty;
                while (!"exit".Equals(message = Console.ReadLine(), StringComparison.OrdinalIgnoreCase))
                {
                    var body = Encoding.UTF8.GetBytes(message);
                    // 4.发送
                    channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: null, body: body);
                    Console.WriteLine($"已发送消息内容：{message}");
                    Console.WriteLine("==================================");
                }
            }
        }
    }
}
