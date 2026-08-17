using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{
    public interface ILocationService
    {
        #region 查询站点列表GetLocationListAsync
        Task<ApiResponse<PageResponse<LocationResponse>>> GetLocationListAsync(GetLocationListRequest request);
        #endregion
        #region 查询站点树GetLocationTreeAsync
        Task<ApiResponse<List<LocationTreeResponse>>> GetLocationTreeAsync(int CompanyID);
        #endregion
        #region 新增站点AddLocationAsync
        Task<ApiResponse<bool>> AddLocationAsync(AddLocationRequest request);
        #endregion
        #region 修改站点UpdateLocationAsync
        Task<ApiResponse<bool>> UpdateLocationAsync(UpdateLocationRequest request);
        #endregion
        #region 修改站点状态UpdateLocationStatusAsync
        Task<ApiResponse<bool>> UpdateLocationStatusAsync(UpdateLocationStatusRequest request);
        #endregion
    }
}