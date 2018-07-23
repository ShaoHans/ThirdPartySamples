using Polly;
using RedisDemo.Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisDemo
{
    public class DataTypeTest
    {
        private TimeSpan _ts;
        private RedisHelper redisHelper;

        public DataTypeTest()
        {
            _ts = TimeSpan.FromHours(4);
            redisHelper = new RedisHelper();
        }

        public void StringTest()
        {
            // 字符串操作
            redisHelper.StringSet("city", "九江", _ts);
            redisHelper.StringSet("stu_shz", new Student { Id = 1, Name = "shz", Birthday = DateTime.Now }, _ts);
            redisHelper.StringIncrement("get_count", 0);
            redisHelper.StringSet("get_count_notInc", 0);
            List<KeyValuePair<RedisKey, RedisValue>> kvs = new List<KeyValuePair<RedisKey, RedisValue>>();
            kvs.Add(new KeyValuePair<RedisKey, RedisValue>("test_string_1", "hello"));
            kvs.Add(new KeyValuePair<RedisKey, RedisValue>("test_string_2", "redis"));
            redisHelper.StringSet(kvs);

            Console.WriteLine($"city:{redisHelper.StringGet("city")}");
            Student stu = redisHelper.StringGet<Student>("stu_shz");
            Console.WriteLine($"{stu.Name}:{stu.Birthday}");

            foreach (var v in redisHelper.StringGet(new List<string> { "test_string_1", "test_string_2" }))
            {
                Console.WriteLine(v);
            }
            Console.WriteLine($"get_count:{redisHelper.StringGet("get_count")}");
            Console.WriteLine($"get_count_notInc:{redisHelper.StringGet("get_count_notInc")}");

            ThreadPool.SetMinThreads(100, 100);
            ThreadPool.GetMinThreads(out int workerThreadCount, out int completionPortThreadCount);
            Console.WriteLine($"workerThreadCount:{workerThreadCount},completionPortThreadCount:{completionPortThreadCount}");
            TestLockTake();
        }

        private void TestLockTake()
        {
            List<Task> tasks = new List<Task>();
            RedisValue token = Environment.MachineName;
            RedisKey key = "locktake";
            for (int i = 0; i < 15; i++)
            {
                Task task = Task.Run(() =>
                {
                    //redisHelper.StringIncrement("get_count");线程安全

                    // 通过Polly组件，一直重试，直到LockTake返回true
                    Policy.HandleResult(false)
                    .RetryForever()
                    .Execute(() => SetIncCount());
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            //Console.WriteLine($"get_count:{redisHelper.StringGet("get_count")}");

            Console.WriteLine($"get_count_notInc:{redisHelper.StringGet("get_count_notInc")}");
        }

        private bool SetIncCount()
        {
            RedisValue token = Environment.MachineName;
            RedisKey key = "locktake";
            var db = redisHelper.GetDatabase();
            bool result = db.LockTake(key, token, TimeSpan.FromSeconds(100));
            if (result)
            {
                try
                {
                    int count = redisHelper.StringGet<int>("get_count_notInc");
                    count++;
                    Console.WriteLine($"count:{count}");
                    redisHelper.StringSet("get_count_notInc", count);
                }
                catch
                {

                }
                finally
                {
                    db.LockRelease(key, token);
                }
            }
            return result;
        }

        public void HashTest()
        {
            // Hash操作
            Student stu1 = new Student { Id = 1, Name = "shz", Birthday = DateTime.Now };
            Student stu2 = new Student { Id = 2, Name = "tom", Birthday = DateTime.Now.AddDays(-10) };
            redisHelper.HashSet<Student>("cookie", stu1.Id.ToString(), stu1);
            redisHelper.HashSet<Student>("cookie", stu2.Id.ToString(), stu2);
            stu1 = redisHelper.HashGet<Student>("cookie", "1");
            stu2 = redisHelper.HashGet<Student>("cookie", "2");
            Console.WriteLine($"name:{stu1.Name}");
            Console.WriteLine($"name:{stu2.Name}");

            stu2.Name = "tom2";
            stu2.Birthday = DateTime.Now.AddYears(-10);
            redisHelper.HashSet<Student>("cookie", stu2.Id.ToString(), stu2);
            List<string> keys = redisHelper.HashKeys<string>("cookie");
            keys.ForEach(s => { Console.WriteLine($"key:{s}"); });
            List<Student> stus = redisHelper.HashValues<Student>("cookie");
            stus.ForEach(s => { Console.WriteLine($"name:{s.Name},id:{s.Id}"); });
        }

        public void ListTest()
        {
            // List操作
            for (int i = 0; i < 10; i++)
            {
                redisHelper.ListLeftPush("stus", new Student { Id = i + 1, Name = $"tom{i + 1}", Birthday = DateTime.Now });
            }
            redisHelper.ListRange<Student>("stus").ForEach(s => { Console.WriteLine($"name:{s.Name},id:{s.Id}"); });
        }

        public void SortedSetTest()
        {
            // SortedSet操作
            redisHelper.SortedSetAdd("sortedset.sample", new Student { Id = 1, Name = "aa", Birthday = DateTime.Now }, 10);
            redisHelper.SortedSetAdd("sortedset.sample", new Student { Id = 2, Name = "bb", Birthday = DateTime.Now }, 9);
            redisHelper.SortedSetAdd("sortedset.sample", new Student { Id = 3, Name = "cc", Birthday = DateTime.Now }, 15);
            redisHelper.SortedSetRangeByRank<Student>("sortedset.sample").ForEach(s => { Console.WriteLine($"name:{s.Name},id:{s.Id}"); });
        }

        public void HyperLogLogTest()
        {
            redisHelper.HyperLogLogAdd("ip_20180723", new RedisValue[5] { "192.168.1.1", "192.168.1.2", "192.168.1.3", "192.168.1.3", "192.168.1.10" });
            redisHelper.HyperLogLogAdd("ip_20180724", new RedisValue[3] { "192.168.1.3", "192.168.1.4", "192.168.1.5" });
            redisHelper.HyperLogLogAdd("ip_20180725", new RedisValue[3] { "192.168.1.6", "192.168.1.7", "192.168.1.8" });

            //合集
            redisHelper.HyperLogLogMerge("ip_201807", new List<string> { "ip_20180723", "ip_20180724", "ip_20180725" });

            Console.WriteLine($"ip_20180723的数量:{redisHelper.HyperLogLogLength("ip_20180723")}");
            Console.WriteLine($"ip_20180724的数量:{redisHelper.HyperLogLogLength("ip_20180724")}");
            Console.WriteLine($"ip_20180725的数量:{redisHelper.HyperLogLogLength("ip_20180725")}");
            Console.WriteLine($"ip_20180723与ip_20180724合集的数量:{redisHelper.HyperLogLogLength(new List<string> { "ip_20180723" , "ip_20180724" })}");
            Console.WriteLine($"ip_201807的数量:{redisHelper.HyperLogLogLength("ip_201807")}");
        }
    }
}
