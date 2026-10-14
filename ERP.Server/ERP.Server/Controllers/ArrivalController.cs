using ERP.Server.Common;
using ERP.Server.Models.Request.Arrival;
using ERP.Server.Models.Response.Arrival;
using ERP.Server.Services.Interfaces.Arrival;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class ArrivalController : ControllerBase
    {
        private readonly IArrivalService _arrivalService;

        public ArrivalController(IArrivalService arrivalService)
        {
            _arrivalService = arrivalService;
        }

        #region 新增到货单AddArrival
        [HttpPost]
        public async Task<ApiResponse<bool>> AddArrival([FromBody] AddArrivalRequest request)
        {
          return await _arrivalService.AddArrivalAsync(request);
        }
        #endregion

        #region 查询到货单列表GetArrivalList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<ArrivalListResponse>>> GetArrivalList([FromQuery] GetArrivalListRequest request)
        {
            return await _arrivalService.GetArrivalListAsync(request);
        }
        #endregion

        #region 查询到货单详情GetArrivalDetail
        [HttpGet]
        public async Task<ApiResponse<ArrivalDetailResponse>> GetArrivalDetail([FromQuery] GetArrivalDetailRequest request)
        {
            return await _arrivalService.GetArrivalDetailAsync(request);
        }
        #endregion

        #region 审核到货单UpdateArrivalAudit
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateArrivalAudit([FromBody] UpdateArrivalAuditRequest request)
        {
            return await _arrivalService.UpdateArrivalAuditAsync(request);
        }
        #endregion

        #region 到货单入库StockInArrival
        [HttpPost]
        public async Task<ApiResponse<bool>> StockInArrival([FromBody] StockInArrivalRequest request)
        {
            return await _arrivalService.StockInArrivalAsync(request);
        }
        #endregion

        #region 撤销到货入库CancelStockInArrival
        [HttpPost]
        public async Task<ApiResponse<bool>> CancelStockInArrival([FromBody] CancelStockInArrivalRequest request)
        {
            return await _arrivalService.CancelStockInArrivalAsync(request);
        }
        #endregion

        #region 反审核到货单CancelArrivalAudit
        [HttpPost]
        public async Task<ApiResponse<bool>> CancelArrivalAudit([FromBody] UpdateArrivalAuditRequest request)
        {
            return await _arrivalService.CancelArrivalAuditAsync(request);
        }
        #endregion

        #region 修改到货单UpdateArrival
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateArrival([FromBody] UpdateArrivalRequest request)
        {
            return await _arrivalService.UpdateArrivalAsync(request);
        }
        #endregion
    }
}
