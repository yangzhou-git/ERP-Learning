using ERP.Server.Common;
using ERP.Server.Models.Request.Inventory;
using ERP.Server.Models.Response.Inventory;

namespace ERP.Server.Services.Interfaces
{
    public interface IInventoryService
    {
        #region 查询库存汇总分页列表 GetInventorySummaryListAsync
        Task<ApiResponse<PageResponse<InventorySummaryResponse>>> GetInventorySummaryListAsync(GetInventorySummaryListRequest request);
        #endregion

        #region 查询库存供应商明细分页列表 GetInventoryDetailListAsync
        Task<ApiResponse<PageResponse<InventoryDetailResponse>>> GetInventoryDetailListAsync(GetInventoryDetailListRequest request);
        #endregion
    }
}
