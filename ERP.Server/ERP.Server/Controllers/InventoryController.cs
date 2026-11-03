using ERP.Server.Common;
using ERP.Server.Models.Request.Inventory;
using ERP.Server.Models.Response.Inventory;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        #region 查询库存汇总分页列表 GetInventorySummaryList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<InventorySummaryResponse>>> GetInventorySummaryList([FromQuery] GetInventorySummaryListRequest request)
        {
            return await _inventoryService.GetInventorySummaryListAsync(request);
        }
        #endregion

        #region 查询库存明细分页列表 GetInventoryDetailList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<InventoryDetailResponse>>> GetInventoryDetailList([FromQuery] GetInventoryDetailListRequest request)
        {
            return await _inventoryService.GetInventoryDetailListAsync(request);
        }
        #endregion
    }
}
