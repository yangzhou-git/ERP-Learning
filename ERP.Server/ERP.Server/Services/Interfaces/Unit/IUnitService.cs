using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{
    public interface IUnitService
    {
        #region 新增单位AddUnitAsync
        Task<ApiResponse<bool>> AddUnitAsync(AddUnitRequest request);
        #endregion
        #region 查询单位列表GetUnitListAsync
        Task<ApiResponse<PageResponse<UnitListResponse>>> GetUnitListAsync(GetUnitListRequest request);
        #endregion
        #region 修改单位UpdateUnitAsync
        Task<ApiResponse<bool>> UpdateUnitAsync(UpdateUnitRequest request);
        #endregion
        #region 修改单位状态UpdateUnitStatusAsync
        Task<ApiResponse<bool>> UpdateUnitStatusAsync(UpdateUnitStatusRequest request);
        #endregion
    }
}
