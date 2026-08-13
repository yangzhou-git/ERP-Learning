using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GoodsController : ControllerBase
    {
        private readonly IGoodsService _GoodsService;

        public GoodsController(IGoodsService GoodsService)
        {
            _GoodsService = GoodsService;
        }

        #region 新增商品AddGoods
        [HttpPost]
        public async Task<ApiResponse<bool>> AddGoods([FromBody]AddGoodsRequest Request)
        {
            var Result = await _GoodsService.AddGoodsAsync(Request);
            return Result;
        }
        #endregion
        #region 获取商品列表GetGoodsList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<GoodsListResponse>>> GetGoodsList([FromQuery] GetGoodsListRequest Request)
        {
            var Result = await _GoodsService.GetGoodsListAsync(Request);
            return Result;
        }
        #endregion
        #region 修改商品状态UpdateGoodsStatus
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateGoodsStatus(UpdateGoodsStatusRequest request)
        {
            var Result = await _GoodsService.UpdateGoodsStatusAsync(request);
            return Result;
        }
        #endregion
        #region 修改商品资料UpdateGoodsAsync
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateGoods(UpdateGoodsRequest request)
        {
            var result = await _GoodsService.UpdateGoodsAsync(request);
            return result;
        }
        #endregion
    }
}
