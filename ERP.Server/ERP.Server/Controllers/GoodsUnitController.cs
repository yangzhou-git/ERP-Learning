using ERP.Server.Common;
using ERP.Server.Models.Request;
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
    }
}
