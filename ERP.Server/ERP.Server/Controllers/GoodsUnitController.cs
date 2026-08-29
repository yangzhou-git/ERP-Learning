using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Request.GoodsUnit;
using ERP.Server.Models.Response;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("Api/[controller]/[action]")]
    [Authorize]
    public class GoodsUnitController : ControllerBase
    {
        private readonly IGoodsUnitService _goodsUnitService;

        public GoodsUnitController(IGoodsUnitService goodsUnitService) 
        {
            _goodsUnitService = goodsUnitService;
        }

        #region 新增商品单位关系AddGoodsUnit
        [HttpPost]
        public async Task<ApiResponse<bool>> AddGoodsUnit(AddGoodsUnitRequest request)
        {
            return await _goodsUnitService.AddGoodsUnitAsync(request);
        }
        #endregion
        #region 查询商品单位列表GetGoodsUnitList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<GoodsUnitListResponse>>> GetGoodsUnitList([FromQuery] GetGoodsUnitListRequest request)
        {
            return await _goodsUnitService.GetGoodsUnitListAsync(request);
        }
        #endregion
        #region 修改商品单位 UpdateGoodsUnit
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateGoodsUnit(UpdateGoodsUnitRequest request)
        {
            return await _goodsUnitService.UpdateGoodsUnitAsync(request);
        }
        #endregion
        #region 修改商品单位关系状态UpdateGoodsUnitStatus
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateGoodsUnitStatus(UpdateGoodsUnitStatusRequest request)
        {
            return await _goodsUnitService.UpdateGoodsUnitStatusAsync(request);
        }
        #endregion
    }
}
