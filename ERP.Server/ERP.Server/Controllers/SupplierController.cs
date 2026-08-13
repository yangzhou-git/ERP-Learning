using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        #region 新增供应商AddSupplier
        [HttpPost]
        public async Task<ApiResponse<bool>> AddSupplier(AddSupplierRequest request)
        {
            return await _supplierService.AddSupplierAsync(request);
        }
        #endregion
        #region 查询供应商列表GetSupplierList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<SupplierListResponse>>> GetSupplierList([FromQuery] GetSupplierListRequest request)
        {
            return await _supplierService.GetSupplierListAsync(request);
        }
        #endregion
        #region 修改供应商状态UpdateSupplierStatus
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateSupplierStatus(UpdateSupplierStatusRequest request)
        {
            return await _supplierService.UpdateSupplierStatusAsync(request);
        }
        #endregion
        #region 修改供应商UpdateSupplier
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateSupplier(UpdateSupplierRequest request)
        {
            return await _supplierService.UpdateSupplierAsync(request);
        }
        #endregion
    }
}
