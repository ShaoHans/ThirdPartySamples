using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RedisDemo.Common
{
    /// <summary>
    /// StackExchangeRedisHelper
    /// 
    /// 在StackExchange.Redis中最重要的对象是ConnectionMultiplexer类， 它存在于StackExchange.Redis命名空间中。
    /// 这个类隐藏了Redis服务的操作细节，ConnectionMultiplexer类做了很多东西， 在所有调用之间它被设计为共享和重用的。
    /// 不应该为每一个操作都创建一个ConnectionMultiplexer 。 ConnectionMultiplexer是线程安全的 ， 推荐使用下面的方法。
    /// 在所有后续示例中 ， 都假定你已经实例化好了一个ConnectionMultiplexer类，它将会一直被重用 ，
    /// 现在我们来创建一个ConnectionMultiplexer实例。它是通过ConnectionMultiplexer.Connect 或者 ConnectionMultiplexer.ConnectAsync，
    /// 传递一个连接字符串或者一个ConfigurationOptions 对象来创建的。
    /// 连接字符串可以是以逗号分割的多个服务的节点.
    /// 
    /// 
    /// 注意 : 
    /// ConnectionMultiplexer 实现了IDisposable接口当我们不再需要是可以将其释放的 , 这里我故意不使用 using 来释放他。 
    /// 简单来讲创建一个ConnectionMultiplexer是十分昂贵的 ， 一个好的主意是我们一直重用一个ConnectionMultiplexer对象。
    /// 一个复杂的的场景中可能包含有主从复制 ， 对于这种情况，只需要指定所有地址在连接字符串中（它将会自动识别出主服务器）
    ///  ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("server1:6379,server2:6379");
    /// 假设这里找到了两台主服务器，将会对两台服务进行裁决选出一台作为主服务器来解决这个问题 ， 这种情况是非常罕见的 ，我们也应该避免这种情况的发生。
    /// 
    /// 
    /// 这里有个和 ServiceStack.Redis 大的区别是没有默认的连接池管理了。没有连接池自然有其利弊,最大的好处在于等待获取连接的等待时间没有了,
    /// 也不会因为连接池里面的连接由于没有正确释放等原因导致无限等待而处于死锁状态。缺点在于一些低质量的代码可能导致服务器资源耗尽。不过提供连接池等阻塞和等待的手段是和作者的设计理念相违背的。StackExchange.Redis这里使用管道和多路复用的技术来实现减少连接
    /// 
    /// 参考：http://www.cnblogs.com/Leo_wl/p/4968537.html
    /// 
    /// 修改记录
    /// 
    ///        2016.04.07 版本：1.0 SongBiao    主键创建。
    ///        
    /// <author>
    ///        <name>SongBiao</name>
    ///        <date>2016.04.07</date>
    /// </author>
    /// </summary>
    public static class StackExchangeRedisHelper
    {
        //系统自定义Key前缀
        public static readonly string SysCustomKey = "Test.Type.";

        //"127.0.0.1:6379,allowadmin=true
        //private static readonly string RedisConnectionString = "192.168.1.49:6379,192.168.1.49:6380,192.168.1.49:6381";
        private static readonly string RedisConnectionString = "127.0.0.1:6379";

        private static readonly object Locker = new object();
        private static ConnectionMultiplexer _instance;
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionCache = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        /// <summary>
        /// 单例获取
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {
                            _instance = GetManager();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 缓存获取
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnectionMultiplexer(string connectionString)
        {
            if (!ConnectionCache.ContainsKey(connectionString))
            {
                ConnectionCache[connectionString] = GetManager(connectionString);
            }
            return ConnectionCache[connectionString];
        }

        private static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            connectionString = connectionString ?? RedisConnectionString;

    //        ConfigurationOptions options = new ConfigurationOptions()
    //        {
    //            EndPoints =
    //            {
    //                { "10.0.0.9", 9001 },
    //                { "10.0.0.9", 9002 },
    //                { "10.0.0.9", 9003 },
    //                //{ "10.0.0.9", 9004 },
    //                //{ "10.0.0.9", 9005 },
    //                //{ "10.0.0.9", 9006 },
    //            },
    //            CommandMap = CommandMap.Create(new HashSet<string>
    //{ // EXCLUDE a few commands
    //    "INFO", "CONFIG", "CLUSTER",
    //    "PING", "ECHO", "CLIENT"
    //}, available: false),
    //        };
            var connect = ConnectionMultiplexer.Connect(connectionString);
            


            //注册如下事件
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;

            return connect;
        }

        #region 事件

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Console.WriteLine("Configuration changed: " + e.EndPoint);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine("ErrorMessage: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("ConnectionRestored: " + e.EndPoint);
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.WriteLine("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine("InternalError:Message" + e.Exception.Message);
        }

        #endregion 事件
    }
}
