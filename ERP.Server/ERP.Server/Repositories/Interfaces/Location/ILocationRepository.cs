using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;

namespace ERP.Server.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        #region 查询站点列表GetLocationListAsync
        Task<List<Location>> GetLocationListAsync(GetLocationListRequest request);
        #endregion
        #region 查询站点数量GetLocationCountAsync
        Task<int> GetLocationCountAsync(GetLocationListRequest request);
        #endregion
        #region 根据站点编码查询GetLocationByCodeAsync
        Task<Location?> GetLocationByCodeAsync(int CompanyID, string LocationCode);
        #endregion
        #region 根据站点ID查询GetLocationByIdAsync
        Task<Location?> GetLocationByIdAsync(int LocationID);
        #endregion
        #region 查询站点父级链GetLocationParentTreeAsync
        Task<List<Location>> GetLocationParentTreeAsync(int LocationID);
        #endregion
        #region 查询站点树GetLocationTreeAsync
        Task<List<Location>> GetLocationTreeAsync(int CompanyID);
        #endregion
        #region 新增站点AddLocationAsync
        Task<bool> AddLocationAsync(Location location);
        #endregion
        #region 修改站点UpdateLocationAsync
        Task<bool> UpdateLocationAsync(Location location);
        #endregion
        #region 修改站点状态UpdateLocationStatusAsync
        Task<bool> UpdateLocationStatusAsync(Location location);
        #endregion
    }
}
