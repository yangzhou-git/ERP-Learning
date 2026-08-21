namespace ERP.Server.Services.Interfaces
{
    public interface IRedisCacheService
    {
        #region 读取Redis缓存StringGetAsync
        Task<T?> StringGetAsync<T>(string key);
        #endregion
        #region 写入Redis缓存StringSetAsync
        Task<bool> StringSetAsync<T>(string key, T value, TimeSpan expiry);
        #endregion
        #region 删除Redis缓存KeyDeleteAsync
        Task<bool> KeyDeleteAsync(string key);
        #endregion
        #region 尝试获取Redis分布式锁TryAcquireLockAsync
        Task<bool> TryAcquireLockAsync(string lockKey, string lockValue, TimeSpan expiry);
        #endregion
        #region 释放Redis分布式锁ReleaseLockAsync
        Task<bool> ReleaseLockAsync(string lockKey, string lockValue);
        #endregion
    }
}
