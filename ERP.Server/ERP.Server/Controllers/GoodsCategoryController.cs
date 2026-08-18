using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class GoodsCategoryController : ControllerBase
    {
        private readonly IGoodsCategoryService _goodsCategoryService;

        public GoodsCategoryController(IGoodsCategoryService goodsCategoryService)
        {
            _goodsCategoryService = goodsCategoryService;
        }

        #region 获取商品分类列表GetGoodsCategoryList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<GoodsCategoryResponse>>> GetGoodsCategoryList([FromQuery] GetGoodsCategoryListRequest request)
        {
            return await _goodsCategoryService.GetGoodsCategoryListAsync(request);
        }
        #endregion
        #region 查询商品分类树GetGoodsCategoryTree
        [HttpGet]
        public async Task<ApiResponse<List<GoodsCategoryTreeResponse>>> GetGoodsCategoryTree()
        {
            return await _goodsCategoryService.GetGoodsCategoryTreeAsync();
        }
        #endregion
        #region 新增商品分类AddGoodsCategory
        [HttpPost]
        public async Task<ApiResponse<bool>> AddGoodsCategory(AddGoodsCategoryRequest request)
        {
            return await _goodsCategoryService.AddGoodsCategoryAsync(request);
        }
        #endregion
        #region 修改商品分类UpdateGoodsCategory
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateGoodsCategory(UpdateGoodsCategoryRequest request)
        {
            return await _goodsCategoryService.UpdateGoodsCategoryAsync(request);
        }
        #endregion
        #region 修改商品分类状态UpdateGoodsCategoryStatus
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateGoodsCategoryStatus(UpdateGoodsCategoryStatusRequest request)
        {
            return await _goodsCategoryService.UpdateGoodsCategoryStatusAsync(request);
        }
        #endregion
    }
}
