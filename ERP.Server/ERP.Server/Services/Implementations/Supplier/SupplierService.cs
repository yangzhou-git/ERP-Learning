using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        #region 新增供应商AddSupplierAsync
        public async Task<ApiResponse<bool>> AddSupplierAsync(AddSupplierRequest request)
        {
            var supplier = new Supplier
            {
                SupplierName = request.SupplierName,
                Contact = request.Contact,
                Phone = request.Phone,
                Address = request.Address,
                Remark = request.Remark
            };
            var result = await _supplierRepository.AddSupplierAsync(supplier);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "新增供应商成功");
            }
            return ApiResponse<bool>.Error("新增供应商失败");
        }
        #endregion
        #region 查询供应商列表GetSupplierListAsync
        public async Task<ApiResponse<PageResponse<SupplierListResponse>>> GetSupplierListAsync(GetSupplierListRequest request)
        {
            var supplierList = await _supplierRepository.GetSupplierListAsync(request);
            var totalCount = await _supplierRepository.GetSupplierCountAsync(request);
            var supplierResponseList = new List<SupplierListResponse>();
            foreach (var supplierItem in supplierList)
            {
                var supplierResponse = new SupplierListResponse
                {
                    SupplierID = supplierItem.SupplierID,
                    SupplierName = supplierItem.SupplierName,
                    Contact = supplierItem.Contact,
                    Phone = supplierItem.Phone,
                    Address = supplierItem.Address,
                    Remark = supplierItem.Remark,
                    IsActive = supplierItem.IsActive,
                    CreateTime = supplierItem.CreateTime
                };
                supplierResponseList.Add(supplierResponse);
            }
            var pageData = new PageResponse<SupplierListResponse>
            {
                TotalCount = totalCount,
                List = supplierResponseList
            };
            return ApiResponse<PageResponse<SupplierListResponse>>.Success(pageData,"查询供应商列表成功");
        }
        #endregion
        #region 修改供应商状态UpdateSupplierStatusAsync
        public async Task<ApiResponse<bool>> UpdateSupplierStatusAsync(UpdateSupplierStatusRequest request)
        {
            var result = await _supplierRepository.UpdateSupplierStatusAsync(request);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改供应商状态成功");
            }
            return ApiResponse<bool>.Error("修改供应商状态失败");
        }
        #endregion
        #region 修改供应商UpdateSupplierAsync
        public async Task<ApiResponse<bool>> UpdateSupplierAsync(UpdateSupplierRequest request)
        {
            var result = await _supplierRepository.UpdateSupplierAsync(request);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改供应商成功");
            }
            return ApiResponse<bool>.Error("修改供应商失败");
        }
        #endregion
    }
}