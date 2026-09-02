using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Models.Response.Unit;

namespace ERP.Server.Services.Interfaces
{
    public interface IUnitService
    {
        #region 新增单位AddUnitAsync
        Task<ApiResponse<bool>> AddUnitAsync(AddUnitRequest request);
        #endregion
        #region 查询单位下拉选项GetUnitOptionsAsync
        Task<ApiResponse<List<UnitOptionResponse>>> GetUnitOptionsAsync();
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
