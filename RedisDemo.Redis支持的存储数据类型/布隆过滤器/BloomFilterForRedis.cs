using RedisDemo.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisDemo
{
    public class BloomFilterForRedis<T> : BloomFilterBase<T>
    {
        private RedisHelper redisHelper;

        public BloomFilterForRedis(int capacity) : base(capacity, null) { }

        public BloomFilterForRedis(int capacity, int errorRate) : base(capacity, errorRate, null) { }

        public BloomFilterForRedis(int capacity, HashFunction hashFunction) : base(capacity, bestErrorRate(capacity), hashFunction) { }

        public BloomFilterForRedis(int capacity, float errorRate, HashFunction hashFunction) : this(capacity, errorRate, hashFunction, bestM(capacity, errorRate), bestK(capacity, errorRate)) { }

        public BloomFilterForRedis(int capacity, float errorRate, HashFunction hashFunction, int m, int k) : base(capacity, errorRate, hashFunction, m, k)
        {
            redisHelper = new RedisHelper();
        }

        public override void Add(T item)
        {
            if (!redisHelper.KeyExists("BloomFilter"))
            {
                redisHelper.StringSetBit("BloomFilter", 0, false);
            }
            int primaryHash = item.GetHashCode();
            int secondaryHash = getHashSecondary(item);
            for (int i = 0; i < hashFunctionCount; i++)
            {
                int hash = computeHash(primaryHash, secondaryHash, i);
                redisHelper.StringSetBit("BloomFilter", hash, true);
            }
        }


        public override bool Contains(T item)
        {
            int primaryHash = item.GetHashCode();
            int secondaryHash = getHashSecondary(item);
            for (int i = 0; i < hashFunctionCount; i++)
            {
                int hash = computeHash(primaryHash, secondaryHash, i);
                if (!redisHelper.StringGetBit("BloomFilter", hash))
                    return false;
            }
            return true;
        }

        public long BitCount { get { return redisHelper.StringBitCount("BloomFilter"); } }
    }
}
