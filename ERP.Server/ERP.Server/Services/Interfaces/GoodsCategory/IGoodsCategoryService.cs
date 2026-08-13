using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{
    public interface IGoodsCategoryService
    {
        #region 获取商品分类列表GetGoodsCategoryListAsync
        Task<ApiResponse<PageResponse<GoodsCategoryResponse>>> GetGoodsCategoryListAsync(GetGoodsCategoryListRequest request);
        #endregion
        #region 查询商品分类树GetGoodsCategoryTreeAsync
        Task<ApiResponse<List<GoodsCategoryTreeResponse>>> GetGoodsCategoryTreeAsync();
        #endregion
        #region 新增商品分类AddGoodsCategoryAsync
        Task<ApiResponse<bool>> AddGoodsCategoryAsync(AddGoodsCategoryRequest request);
        #endregion
        #region 修改商品分类UpdateGoodsCategoryAsync
        Task<ApiResponse<bool>> UpdateGoodsCategoryAsync(UpdateGoodsCategoryRequest request);
        #endregion
        #region 修改商品分类状态UpdateGoodsCategoryStatusAsync
        Task<ApiResponse<bool>> UpdateGoodsCategoryStatusAsync(UpdateGoodsCategoryStatusRequest request);
        #endregion
    }
}