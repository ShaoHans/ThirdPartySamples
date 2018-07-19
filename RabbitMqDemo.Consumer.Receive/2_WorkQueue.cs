using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqDemo.Consumer.Receive
{
    public class _2_WorkQueue
    {
        public void Receive()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "shz", Password = "123456" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "work_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // 当有多个消费者时，默认使用Round-robin轮询分发策略，即挨个给消费者分发消息，不管消费者是忙碌还是空闲，
                // 这就有可能造成有的消费者很闲，有的消费者很忙，为了避免这种情况，可以使用以下代码来公平分发消息，
                // 即：指定prefetchCount 参数为1，表示消费者在一个时间内最多只能处理一个消息，直到消费者发送确认标志处理完了当前消息
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine("等待接受消息：");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                  {
                      var message = Encoding.UTF8.GetString(e.Body);
                      int dots = message.Split('.').Length - 1;
                      Console.WriteLine($"收到消息：{message}。需要处理{dots}秒");
                      Thread.Sleep(dots * 1000);
                      Console.WriteLine("处理完毕！");
                      Console.WriteLine("=========================================");

                      // 当消费者正在处理该消息的过程中时，因为意外而退出，RabbitMQ服务没有收到该消息的确认回复，会重发该消息给其他消费者
                      channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                  };

                channel.BasicConsume(queue: "work_queue",
                    autoAck: false, // 关闭自动确认模式，使用手动确认模式
                    consumer: consumer);

                Console.ReadLine();
            }
        }
    }
}
