using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMqDemo.Producer.Send
{
    public class _1_HelloWorld
    {
        public void Send()
        {
            using (var connection = RabbitMqDemo.Common.Helper.CreateClusterConnection()) // 1.建立连接
            using (var channel = connection.CreateModel())      // 2.创建信道
            {
                // 3.声明队列
                channel.QueueDeclare(queue: "hello_world", durable: false, exclusive: false, autoDelete: false, arguments: null);

                Console.WriteLine("请输入要发送的文字信息，输入exit退出！");
                string message = string.Empty;
                while (!"exit".Equals(message = Console.ReadLine(), StringComparison.OrdinalIgnoreCase))
                {
                    var body = Encoding.UTF8.GetBytes(message);
                    // 4.发送（使用RabbitMQ默认交换器(AMQP default)）
                    channel.BasicPublish(exchange: "", routingKey: "hello_world", basicProperties: null, body: body);
                    Console.WriteLine($"已发送消息内容：{message}");
                    Console.WriteLine("==================================");
                }
            }
        }
    }
}
