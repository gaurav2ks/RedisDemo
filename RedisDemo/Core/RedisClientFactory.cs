using StackExchange.Redis;
using System;
using System.Configuration;

namespace RedisDemo.Core
{
    public class RedisClientFactory
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        public RedisClientFactory()
        {
            //var redisConnectionString = ConfigurationManager.["RedisConnectionString"];
            //_lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            //{
            //    return ConnectionMultiplexer.Connect(redisConnectionString);
            //});
        }

        public IDatabase GetDatabase()
        {
            return _lazyConnection.Value.GetDatabase();
        }
    }
}

