using System;
using StackExchange.Redis;


namespace RedisTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using ConnectionMultiplexer muxer = ConnectionMultiplexer.Connect("redisServer:6379,password=password");

            IDatabase conn = muxer.GetDatabase();

            conn.StringSet("foo", "bar");
            conn.HashSet("test1", new HashEntry[] { new HashEntry(new RedisValue("prop_1"), new RedisValue("value 1")) });
            var value = conn.StringGet("test1");
            Console.WriteLine(value);
        }
    }
}
