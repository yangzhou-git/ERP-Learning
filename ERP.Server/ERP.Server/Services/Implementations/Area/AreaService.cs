using ERP.Server.Common;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly IRedisCacheService _redisCacheService;

        public AreaService(IAreaRepository areaRepository, IRedisCacheService redisCacheService)
        {
            _areaRepository = areaRepository;
            _redisCacheService = redisCacheService;
        }

        #region 查询地区树（懒加载）GetAreaTreeAsync
        public async Task<ApiResponse<List<AreaTreeResponse>>> GetAreaTreeAsync(int? ParentID)
        {
            var cacheKey = ParentID.HasValue ? $"area:children:{ParentID.Value}" : "area:children:root";
            var cachedAreas = await _redisCacheService.StringGetAsync<List<AreaTreeResponse>>(cacheKey);
            if (cachedAreas != null)
            {
                return ApiResponse<List<AreaTreeResponse>>.Success(cachedAreas,"查询地区成功");
            }
            var lockKey = $"lock:{cacheKey}";
            var lockValue = Guid.NewGuid().ToString();
            var lockAcquired = await _redisCacheService.TryAcquireLockAsync(lockKey, lockValue, TimeSpan.FromSeconds(10));
            if (lockAcquired)
            {
                try
                {
                    var secondCachedAreas = await _redisCacheService.StringGetAsync<List<AreaTreeResponse>>(cacheKey);
                    if (secondCachedAreas != null)
                    {
                        return ApiResponse<List<AreaTreeResponse>>.Success(secondCachedAreas, "查询地区成功");
                    }
                    var areas = await GetAreasFromDatabaseAndSetCacheAsync(ParentID,cacheKey);
                    return ApiResponse<List<AreaTreeResponse>>.Success(areas, "查询地区成功");
                }
                finally
                {
                    await _redisCacheService.ReleaseLockAsync(lockKey, lockValue);
                }
            }
            for (var i = 0; i< 20; i++)
            {
                await Task.Delay(50);
                var retryCachedAreas = await _redisCacheService.StringGetAsync<List<AreaTreeResponse>>(cacheKey);
                if(retryCachedAreas != null)
                {
                    return ApiResponse<List<AreaTreeResponse>>.Success(retryCachedAreas, "查询地区成功");
                }
                var retryLockAcquired = await _redisCacheService.TryAcquireLockAsync(lockKey, lockValue, TimeSpan.FromSeconds(10));
                if (retryLockAcquired)
                {
                    try
                    {
                        var retrySecondCachedAreas = await _redisCacheService.StringGetAsync<List<AreaTreeResponse>>(cacheKey);
                        if (retrySecondCachedAreas != null) 
                        {
                            return ApiResponse<List<AreaTreeResponse>>.Success(retrySecondCachedAreas, "查询地区成功");
                        }
                        var areas = await GetAreasFromDatabaseAndSetCacheAsync(ParentID, cacheKey);
                        return ApiResponse<List<AreaTreeResponse>>.Success(areas, "查询地区成功");
                    }
                    finally
                    {
                        await _redisCacheService.ReleaseLockAsync(lockKey, lockValue);
                    }
                }
            }
            return ApiResponse<List<AreaTreeResponse>>.Error("系统繁忙，请稍后重试");
        }
        #endregion

        #region 查询数据库并写入地区缓存GetAreasFromDatabaseAndSetCacheAsync
        private async Task<List<AreaTreeResponse>> GetAreasFromDatabaseAndSetCacheAsync(int? ParentID, string cacheKey)
        {
            var areaList = await _areaRepository.GetAreaTreeAsync(ParentID);
            var areas = new List<AreaTreeResponse>();
            foreach(var area in areaList)
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
            var expiry = areas.Count > 0 ? TimeSpan.FromDays(1) : TimeSpan.FromMinutes(5);
            await _redisCacheService.StringSetAsync<List<AreaTreeResponse>>(cacheKey, areas, expiry);
            return areas;
        }
        #endregion
    }
}