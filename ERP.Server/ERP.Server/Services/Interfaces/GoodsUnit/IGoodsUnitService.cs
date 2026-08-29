using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Request.GoodsUnit;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{
    public interface IGoodsUnitService
    {
        #region 查询商品单位列表GetGoodsUnitListAsync
        Task<ApiResponse<PageResponse<GoodsUnitListResponse>>> GetGoodsUnitListAsync(GetGoodsUnitListRequest request);
        #endregion
        #region 新增商品单位关系AddGoodsUnitAsync
        Task<ApiResponse<bool>> AddGoodsUnitAsync(AddGoodsUnitRequest request);
        #endregion
        #region 修改商品单位UpdateGoodsUnitAsync
        Task<ApiResponse<bool>> UpdateGoodsUnitAsync(UpdateGoodsUnitRequest request);
        #endregion
        #region 修改商品单位关系状态UpdateGoodsUnitStatusAsync
        Task<ApiResponse<bool>> UpdateGoodsUnitStatusAsync(UpdateGoodsUnitStatusRequest request);
        #endregion
    }
}
