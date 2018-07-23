using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RedisDemo
{
    /// <summary>
    /// https://gist.github.com/richardkundl/8300092
    /// https://archive.codeplex.com/?p=bloomfilter#BloomFilter/Filter.cs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BloomFilterForBitArray<T>: BloomFilterBase<T>
    {
        private BitArray hashBits;

        public BloomFilterForBitArray(int capacity) : base(capacity, null) { }

        public BloomFilterForBitArray(int capacity, int errorRate) : base(capacity, errorRate, null) { }

        public BloomFilterForBitArray(int capacity, HashFunction hashFunction) : base(capacity, bestErrorRate(capacity), hashFunction) { }

        public BloomFilterForBitArray(int capacity, float errorRate, HashFunction hashFunction) : this(capacity, errorRate, hashFunction, bestM(capacity, errorRate), bestK(capacity, errorRate)) { }

        public BloomFilterForBitArray(int capacity, float errorRate, HashFunction hashFunction, int m, int k) :base(capacity,errorRate,hashFunction,m,k)
        {
            hashBits = new BitArray(m);
        }

        /// <summary>
        /// Adds a new item to the filter. It cannot be removed.
        /// </summary>
        /// <param name="item"></param>
        public override void Add(T item)
        {
            // start flipping bits for each hash of item
            int primaryHash = item.GetHashCode();
            int secondaryHash = getHashSecondary(item);
            for (int i = 0; i < hashFunctionCount; i++)
            {
                int hash = computeHash(primaryHash, secondaryHash, i);
                hashBits[hash] = true;
            }
        }

        /// <summary>
        /// Checks for the existance of the item in the filter for a given probability.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool Contains(T item)
        {
            int primaryHash = item.GetHashCode();
            int secondaryHash = getHashSecondary(item);
            for (int i = 0; i < hashFunctionCount; i++)
            {
                int hash = computeHash(primaryHash, secondaryHash, i);
                if (hashBits[hash] == false)
                    return false;
            }
            return true;
        }
    }
}
