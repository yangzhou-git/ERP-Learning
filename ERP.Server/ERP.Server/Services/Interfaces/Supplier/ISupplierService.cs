using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{
    public interface ISupplierService
    {
        #region 新增供应商AddSupplierAsync
        Task<ApiResponse<bool>> AddSupplierAsync(AddSupplierRequest request);
        #endregion
        #region 查询供应商GetSupplierListAsync
        Task<ApiResponse<PageResponse<SupplierListResponse>>> GetSupplierListAsync(GetSupplierListRequest request);
        #endregion
        #region 修改供应商状态UpdateSupplierStatusAsync
        Task<ApiResponse<bool>> UpdateSupplierStatusAsync(UpdateSupplierStatusRequest request);
        #endregion
        #region 修改供应商UpdateSupplierAsync
        Task<ApiResponse<bool>> UpdateSupplierAsync(UpdateSupplierRequest request);
        #endregion
    }
}
