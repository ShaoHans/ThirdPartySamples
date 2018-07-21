using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqDemo.Consumer.Receive
{
    public class _1_HelloWorld
    {
        public void Receive()
        {
            using (var connection = RabbitMqDemo.Common.Helper.CreateClusterConnection()) // 1.建立连接
            using (var channel = connection.CreateModel())      // 2.创建信道
            {
                // 3.声明队列
                channel.QueueDeclare(queue: "hello_world", durable: false, exclusive: false, autoDelete: false, arguments: null);

                // 4.定义消费者
                Console.WriteLine("等待接受消息：");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                {
                    Console.WriteLine($"收到消息：{Encoding.UTF8.GetString(e.Body)}");
                };

                // 5.消费者获取消息
                channel.BasicConsume(queue: "hello_world", autoAck: true, consumer: consumer);

                // 等待用户输入，防止using语句执行完毕后channel关闭，获取不到消息
                Console.ReadLine();
            }
        }

        
    }
}
