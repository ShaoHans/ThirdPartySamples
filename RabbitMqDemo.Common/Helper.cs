using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace RabbitMqDemo.Common
{
    public enum ConnectionType
    {
        Single,
        Cluster,
        Proxy
    }

    public class Helper
    {
        public static IConnection CreateConnection(ConnectionType type)
        {
            switch (type)
            {
                case ConnectionType.Single:
                    return CreateSingleConnection();
                case ConnectionType.Cluster:
                    return CreateClusterConnection();
                case ConnectionType.Proxy:
                    return CreateProxyConnection();
                default:
                    throw new Exception("未知的ConnectionType");
            }
        }

        /// <summary>
        /// RabbitMQServer单节点
        /// </summary>
        /// <returns></returns>
        public static IConnection CreateSingleConnection()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "shz", Password = "123456" };
            return factory.CreateConnection();
        }

        /// <summary>
        /// RabbitMQServer集群
        /// </summary>
        /// <returns></returns>
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
                new AmqpTcpEndpoint(new Uri("amqp://10.0.0.10/")),
                new AmqpTcpEndpoint(new Uri("amqp://10.0.0.11/"))
            };
            return factory.CreateConnection(endpoints);
        }

        /// <summary>
        /// 通过HAProxy反向代理访问RabbitMQServer集群
        /// </summary>
        /// <returns></returns>
        public static IConnection CreateProxyConnection()
        {
            var factory = new ConnectionFactory() { HostName = "node1", Port = 8101, UserName = "shz", Password = "123456" };
            return factory.CreateConnection();
        }
    }
}
