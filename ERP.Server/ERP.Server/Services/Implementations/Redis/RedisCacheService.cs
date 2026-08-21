using System.Text.Json;
using ERP.Server.Services.Interfaces;
using StackExchange.Redis;

namespace ERP.Server.Services.Implementations
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        #region 读取Redis缓存StringGetAsync
        public async Task<T?> StringGetAsync<T>(string key)
        {
            var database = _redis.GetDatabase();
            var cachedValue = await database.StringGetAsync(key);
            if (cachedValue.HasValue)
            {
                return JsonSerializer.Deserialize<T>(cachedValue.ToString());
            }
            return default;
        }
        #endregion
        #region 写入Redis缓存StringSetAsync
        public async Task<bool> StringSetAsync<T>(string key, T value , TimeSpan expiry)
        {
            var database = _redis.GetDatabase();
            var json = JsonSerializer.Serialize<T>(value);
            return await database.StringSetAsync(key, json, expiry);
        }
        #endregion
        #region 删除Redis缓存KeyDeleteAsync
        public async Task<bool> KeyDeleteAsync(string key)
        {
            return await _redis.GetDatabase().KeyDeleteAsync(key);
        }
        #endregion
        #region 尝试获取Redis分布式锁TryAcquireLockAsync
        public async Task<bool> TryAcquireLockAsync(string lockKey, string lockValue, TimeSpan expiry)
        {
            var database = _redis.GetDatabase();
            return await database.StringSetAsync(lockKey, lockValue, expiry, when: When.NotExists);
        }
        #endregion
        #region 释放Redis分布式锁ReleaseLockAsync
        public async Task<bool> ReleaseLockAsync(string lockKey, string lockValue)
        {
            var database = _redis.GetDatabase();
            var script = @" if redis.call('GET', KEYS[1]) == ARGV[1] then return redis.call('DEL', KEYS[1]) end return 0 ";
            var result = await database.ScriptEvaluateAsync(script, new RedisKey[] { lockKey }, new RedisValue[] { lockValue });
            if ((int)result == 1)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
