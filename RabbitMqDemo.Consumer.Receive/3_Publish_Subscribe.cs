using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqDemo.Consumer.Receive
{
    public class _3_Publish_Subscribe
    {
        public void Receive()
        {
            using (var connection = RabbitMqDemo.Common.Helper.CreateClusterConnection())
            using (var channel = connection.CreateModel())
            {
                // 消费者之所以也要和生产者定义同样的exchange，是因为当消费者先启动时，如果没有exchange就进行queue绑定会出错，
                // 如果能确定生产者首先启动，可以注释以下代码。
                // 注：生产者和消费者声明的exchange是一样的，如果一方更改了某一参数值将会导致异常，双方必须保持一致
                string exchangeName = "sms";
                channel.ExchangeDeclare(exchange: exchangeName,
                    type: "fanout", // 交换器类别：fanout表示多播，消息会被发送到所有队列
                    durable: true,
                    autoDelete: false,
                    arguments: null);

                // 声明一个采用默认参数的队列，队列名称随机产生；应用程序一旦停止，队列自动删除
                string queueName = channel.QueueDeclare().QueueName;
                // 将交换器exchange和队列queue进行绑定
                channel.QueueBind(queue: queueName,
                    exchange: exchangeName,
                    routingKey: "", // 当exchange的type为fanout时，会忽略此参数
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                  {
                      Console.WriteLine($"接收到消息：{Encoding.UTF8.GetString(e.Body)}");
                  };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer:consumer);
                Console.ReadLine();
            }
        }
    }
}
