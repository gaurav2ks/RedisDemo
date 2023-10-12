using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

public class RedisHandler 
{
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly ILogger _logger;

    public RedisHandler(IConnectionMultiplexer connection, ILogger logger)
    {
        _redisConnection = connection;
        _logger = logger;
    }

    // Store a key-value pair in Redis with an optional expiration time
    public bool Set(string key, Object value, TimeSpan? expiry = null)
    {
        try
        {
            var db = _redisConnection.GetDatabase();
            string jsonString = JsonConvert.SerializeObject(value);
            return db.StringSet(key, jsonString, expiry);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., Redis server down)
            _logger.LogError($"Redis Set Error: {ex.Message}");
            return false;
        }
    }

    // Retrieve the value associated with a key from Redis
    public string Get(string key)
    {
        if (string.IsNullOrEmpty(key))
            return string.Empty;
        try
        {
            var db = _redisConnection.GetDatabase();
            return db.StringGet(key);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., Redis server down)
            _logger.LogError($"Redis Get Error: {ex.Message}");
            return null;
        }
    }

    public T GetDataFromCache<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            return default(T);
        try
        {
            var db = _redisConnection.GetDatabase();
            string value =  db.StringGet(key);
            if (string.IsNullOrEmpty(value))
                return default(T);

            return JsonConvert.DeserializeObject<T>(value);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., Redis server down)
            _logger.LogError($"Redis Get Error: {ex.Message}");
            return default(T);
        }
    }

    internal void DeleteKey(string key)
    {
        try
        {
            var db = _redisConnection.GetDatabase();
            db.KeyDelete(key);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., Redis server down)
           _logger.LogError($"Redis KeyDelete Error: {ex.Message}");
        }
    }

    // Optionally implement other Redis operations as needed
}
