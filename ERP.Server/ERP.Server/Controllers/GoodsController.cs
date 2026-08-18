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
    public class GoodsController : ControllerBase
    {
        private readonly IGoodsService _goodsService;

        public GoodsController(IGoodsService goodsService)
        {
            _goodsService = goodsService;
        }

        #region 新增商品AddGoods
        [HttpPost]
        public async Task<ApiResponse<bool>> AddGoods(AddGoodsRequest request)
        {
            return await _goodsService.AddGoodsAsync(request);
        }
        #endregion
        #region 获取商品列表GetGoodsList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<GoodsListResponse>>> GetGoodsList([FromQuery] GetGoodsListRequest request)
        {
            return await _goodsService.GetGoodsListAsync(request);
        }
        #endregion
        #region 修改商品状态UpdateGoodsStatus
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateGoodsStatus(UpdateGoodsStatusRequest request)
        {
            return await _goodsService.UpdateGoodsStatusAsync(request);
        }
        #endregion
        #region 修改商品资料UpdateGoods
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateGoods(UpdateGoodsRequest request)
        {
            return await _goodsService.UpdateGoodsAsync(request);
        }
        #endregion
    }
}
