using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{
    public interface IGoodsService
    {
        #region 新增商品AddGoodsAsync
        Task<ApiResponse<bool>> AddGoodsAsync(AddGoodsRequest Request);
        #endregion
        #region 获取商品列表GetGoodsListAsync
        Task<ApiResponse<PageResponse<GoodsListResponse>>> GetGoodsListAsync(GetGoodsListRequest Request);
        #endregion
        #region 修改商品状态UpdateGoodsStatusAsync
        Task<ApiResponse<bool>> UpdateGoodsStatusAsync(UpdateGoodsStatusRequest Request);
        #endregion
        #region 修改商品UpdateGoodsAsync
        Task<ApiResponse<bool>> UpdateGoodsAsync(UpdateGoodsRequest request);
        #endregion
    }
}
