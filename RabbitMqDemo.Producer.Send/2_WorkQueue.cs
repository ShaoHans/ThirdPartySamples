using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMqDemo.Producer.Send
{
    public class _2_WorkQueue
    {
        public void Send()
        {
            using (var connection = RabbitMqDemo.Common.Helper.CreateConnection(Common.ConnectionType.Cluster))
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "work_queue",
                    durable: true,  // 此参数为true表示队列会持久化，RabbitMQ服务崩溃或重启之后，队列不会丢失
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                for (int i = 1; i <= 20; i++)
                {
                    string message = $"第{i}个消息" ;
                    if(i% 2 == 0)
                    {
                        message += "".PadRight(i, '.');
                    }

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true; // 消息持久化，RabbitMQ服务崩溃或重启之后，消息不会丢失
                    channel.BasicPublish(exchange: "", //使用RabbitMQ默认交换器(AMQP default)
                        routingKey: "work_queue",
                        basicProperties: properties,
                        body: Encoding.UTF8.GetBytes(message));

                    int dots = message.Split('.').Length - 1;
                    Console.WriteLine($"发送的消息内容：{message}，含{i}个“.”");
                    Console.WriteLine("======================================");
                }
                
            }
        }
    }
}
