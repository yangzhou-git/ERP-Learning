using ERP.Server.Common;
using ERP.Server.Models.Request.Arrival;
using ERP.Server.Models.Response.Arrival;

namespace ERP.Server.Services.Interfaces.Arrival
{
    public interface IArrivalService
    {
        #region 新增到货单AddArrivalAsync
        Task<ApiResponse<bool>> AddArrivalAsync(AddArrivalRequest request);
        #endregion
        #region 查询到货单列表GetArrivalListAsync
        Task<ApiResponse<PageResponse<ArrivalListResponse>>> GetArrivalListAsync(GetArrivalListRequest request);
        #endregion
        #region 查询到货单详情GetArrivalDetailAsync
        Task<ApiResponse<ArrivalDetailResponse>> GetArrivalDetailAsync(GetArrivalDetailRequest request);
        #endregion
        #region 审核到货单UpdateArrivalAuditAsync
        Task<ApiResponse<bool>> UpdateArrivalAuditAsync(UpdateArrivalAuditRequest request);
        #endregion
        #region 到货单入库StockInArrivalAsync
        Task<ApiResponse<bool>> StockInArrivalAsync(StockInArrivalRequest request);
        #endregion
        #region 撤销到货入库CancelStockInArrivalAsync
        Task<ApiResponse<bool>> CancelStockInArrivalAsync(CancelStockInArrivalRequest request);
        #endregion
        #region 反审核到货单CancelArrivalAuditAsync
        Task<ApiResponse<bool>> CancelArrivalAuditAsync(UpdateArrivalAuditRequest request);
        #endregion
        #region 修改到货单UpdateArrivalAsync
        Task<ApiResponse<bool>> UpdateArrivalAsync(UpdateArrivalRequest request);
        #endregion
    }
}
