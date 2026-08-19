using System.Text.Json;
using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;
using StackExchange.Redis;

namespace ERP.Server.Services.Implementations
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly IConnectionMultiplexer _redis;

        public AreaService(IAreaRepository areaRepository, IConnectionMultiplexer redis)
        {
            _areaRepository = areaRepository;
            _redis = redis;
        }

        #region 查询地区树（懒加载）GetAreaTreeAsync
        public async Task<ApiResponse<List<AreaTreeResponse>>> GetAreaTreeAsync(int? ParentID)
        {
            var database = _redis.GetDatabase();
            var cacheKey = ParentID.HasValue ? $"area:children:{ParentID.Value}" : "area:children:root";
            var cachedValue = await database.StringGetAsync(cacheKey);
            if (cachedValue.HasValue)
            {
                var cachedAreas = JsonSerializer.Deserialize<List<AreaTreeResponse>>(cachedValue.ToString());
                if (cachedAreas != null)
                {
                    return ApiResponse<List<AreaTreeResponse>>.Success(cachedAreas, "查询地区成功");
                }
            }
            var lockKey = $"lock:{cacheKey}";
            var lockValue = Guid.NewGuid().ToString();
            var lockAcquired = await database.StringSetAsync(lockKey, lockValue, TimeSpan.FromSeconds(10), When.NotExists);
            if (lockAcquired)
            {
                try
                {
                    var secondCachedValue = await database.StringGetAsync(cacheKey);
                    if (secondCachedValue.HasValue)
                    {
                        var secondCachedAreas =JsonSerializer.Deserialize<List<AreaTreeResponse>>(secondCachedValue.ToString());
                        if (secondCachedAreas != null)
                        {
                            return ApiResponse<List<AreaTreeResponse>>.Success(secondCachedAreas, "查询地区成功");
                        }
                    }
                    var AreaList =await _areaRepository.GetAreaTreeAsync(ParentID);
                    var areas = new List<AreaTreeResponse>();
                    foreach (var area in AreaList)
                    {
                        var response = new AreaTreeResponse()
                        {
                            AreaID = area.AreaID,
                            AreaCode = area.AreaCode,
                            AreaName = area.AreaName,
                            ParentID = area.ParentID,
                            AreaLevel = area.AreaLevel
                        };
                        areas.Add(response);
                    }
                    var json = JsonSerializer.Serialize(areas);
                    var expiration = areas.Count > 0 ? TimeSpan.FromDays(1) : TimeSpan.FromMinutes(5);
                    await database.StringSetAsync(cacheKey,json,expiration);
                    return ApiResponse<List<AreaTreeResponse>>.Success(areas, "查询地区成功");
                }
                finally
                {
                    var releaseLockScript = @"if redis.call('GET', KEYS[1]) == ARGV[1] then return redis.call('DEL', KEYS[1]) else return 0 end";
                    await database.ScriptEvaluateAsync(releaseLockScript, new RedisKey[] { lockKey }, new RedisValue[] { lockValue });
                }
            }
            for (var i = 0; i < 20; i++)
            {
                await Task.Delay(50);
                var retryCachedValue = await database.StringGetAsync(cacheKey);
                if (retryCachedValue.HasValue)
                {
                    var retryCachedAreas =JsonSerializer.Deserialize<List<AreaTreeResponse>>(retryCachedValue.ToString());
                    if (retryCachedAreas != null)
                    {
                        return ApiResponse<List<AreaTreeResponse>>.Success(retryCachedAreas, "查询地区成功");
                    }
                }
            }
            return ApiResponse<List<AreaTreeResponse>>.Error("系统繁忙，请稍后重试");
        }
        #endregion
    }
}