using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMqDemo.Producer.Send
{
    public enum LogType
    {
        Info = 1,
        Warning = 2,
        Error = 3
    }

    public class _4_Routing
    {
        public void Send()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "shz", Password = "123456" };
            using (var connection = factory.CreateConnection()) // 1.创建连接
            using (var channel = connection.CreateModel())      // 2.建立通道
            {
                // 消息生产者把消息发送到指定的交换器，再由交换器发给与之绑定的队列
                // 3.定义交换器
                string exchangeName = "logs_direct";
                channel.ExchangeDeclare(exchange: exchangeName,
                    type: "direct", // 交换器类别：direct表示消息会被发送到指定的队列，由BasicPublish的routingKey决定
                    durable: true,
                    autoDelete: false,
                    arguments: null);

                Console.WriteLine("请输入要发送的日志信息（1：info，2：warning，3：error），格式（3$这是错误日志），输入exit退出！");
                string message = string.Empty;
                while (!"exit".Equals(message = Console.ReadLine(), StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(message) || message.IndexOf('$') != 1)
                    {
                        Console.WriteLine("无效的日志信息，请重新输入");
                        continue;
                    }

                    if(!int.TryParse(message[0].ToString(), out int result))
                    {
                        Console.WriteLine("无效的日志信息，请以日志类别数字开头，然后紧跟$字符");
                        continue;
                    }

                    LogType logType = (LogType)result;
                    var body = Encoding.UTF8.GetBytes(message);
                    // 4.发送
                    channel.BasicPublish(exchange: exchangeName, routingKey: logType.ToString(), basicProperties: null, body: body);
                    Console.WriteLine($"已发送消息内容：{message}");
                    Console.WriteLine("==================================");
                }
            }
        }
    }
}
