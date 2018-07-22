using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMqDemo.Producer.Send
{
    public class _5_Topics
    {
        public void Send()
        {
            using (var connection = RabbitMqDemo.Common.Helper.CreateConnection(Common.ConnectionType.Cluster)) // 1.创建连接
            using (var channel = connection.CreateModel())      // 2.建立通道
            {
                // 消息生产者把消息发送到指定的交换器，再由交换器发给与之绑定的队列
                // 3.定义交换器
                string exchangeName = "logs_topics";
                channel.ExchangeDeclare(exchange: exchangeName,
                    type: "topic", // 交换器类别：topic表示消息会被发送到与routingKey匹配的队列
                    durable: true,
                    autoDelete: false,
                    arguments: null);

                Console.WriteLine("请输入要发送的日志信息，输入exit退出！");
                string message = string.Empty;
                while (!"exit".Equals(message = Console.ReadLine(), StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        Console.WriteLine("无效的日志信息，请重新输入");
                        continue;
                    }

                    Console.WriteLine("请输入routingKey，如：sso.info");
                    string routingKey = Console.ReadLine();
                    if(string.IsNullOrWhiteSpace(routingKey))
                    {
                        continue;
                    }

                    var body = Encoding.UTF8.GetBytes(message);
                    // 4.发送
                    channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
                    Console.WriteLine($"已发送消息内容：{message}");
                    Console.WriteLine("==================================");
                }
            }
        }
    }
}
