using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{
    public interface IGoodsService
    {
        #region 新增商品AddGoodsAsync
        Task<ApiResponse<bool>> AddGoodsAsync(AddGoodsRequest request);
        #endregion
        #region 获取商品列表GetGoodsListAsync
        Task<ApiResponse<PageResponse<GoodsListResponse>>> GetGoodsListAsync(GetGoodsListRequest request);
        #endregion
        #region 修改商品状态UpdateGoodsStatusAsync
        Task<ApiResponse<bool>> UpdateGoodsStatusAsync(UpdateGoodsStatusRequest request);
        #endregion
        #region 修改商品UpdateGoodsAsync
        Task<ApiResponse<bool>> UpdateGoodsAsync(UpdateGoodsRequest request);
        #endregion
    }
}