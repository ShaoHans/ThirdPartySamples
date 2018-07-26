using RedisDemo.Common;
using System;
using System.Collections.Generic;

namespace RedisDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            RedisHelper redisHelper = new RedisHelper();
            redisHelper.StringSet("gogo", "www.cggogo.com");
            Console.WriteLine(redisHelper.StringGet("gogo")); 
            //DataTypeTest dataTypeTest = new DataTypeTest();
            //dataTypeTest.HashTest();
            //dataTypeTest.HyperLogLogTest();

            //int capacity = 100;
            //float errorRate = 0.001F; // 0.1%
            //BloomFilterForRedis<string> bloomFilter = new BloomFilterForRedis<string>(capacity, errorRate, null);
            //TestBloomFilter(bloomFilter);
            //Console.WriteLine($"{bloomFilter.Contains("88")}");
            //Console.WriteLine($"{bloomFilter.Contains("23")}");
            //Console.WriteLine($"{bloomFilter.Contains("we")}");
            //Console.WriteLine(bloomFilter.BitCount);

            Console.ReadKey();
        }

        static void TestBloomFilter(BloomFilterForRedis<string> bloomFilter)
        {
            List<String> inputs = new List<string>(bloomFilter.Capacity);
            for (int i = 0; i < bloomFilter.Capacity; i++)
            {
                inputs.Add(i.ToString());
            }
            foreach (string input in inputs)
            {
                bloomFilter.Add(input);
            }
            foreach (string input in inputs)
            {
                if (bloomFilter.Contains(input) == false)
                {
                    Console.WriteLine($"不含{input}");
                }
            }
            Console.WriteLine("遍历结束");
        }
    }
}
