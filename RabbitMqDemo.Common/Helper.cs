using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace RabbitMqDemo.Common
{
    public class Helper
    {
        public static IConnection CreateLocalConnection()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "shz", Password = "123456" };
            return factory.CreateConnection();
        }

        public static IConnection CreateClusterConnection()
        {
            var factory = new ConnectionFactory()
            {
                UserName = "shz",
                Password = "123456"
            };

            var endpoints = new List<AmqpTcpEndpoint>
            {
                new AmqpTcpEndpoint(new Uri("amqp://10.0.0.9/")),
                new AmqpTcpEndpoint(new Uri("amqp://10.0.0.10/"))
            };
            return factory.CreateConnection(endpoints);
        }
    }
}
