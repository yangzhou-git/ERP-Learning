using ERP.Server.Common;
using ERP.Server.Common.Helpers;
using ERP.Server.Models.Request.Inventory;
using ERP.Server.Models.Response.Inventory;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;
using FluentValidation;

namespace ERP.Server.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<GetInventorySummaryListRequest> _summaryValidator;
        private readonly IValidator<GetInventoryDetailListRequest> _detailValidator;

        public InventoryService(IInventoryRepository inventoryRepository, ICurrentUserService currentUserService, IValidator<GetInventorySummaryListRequest> summaryValidator, IValidator<GetInventoryDetailListRequest> detailValidator)
        {
            _inventoryRepository = inventoryRepository;
            _currentUserService = currentUserService;
            _summaryValidator = summaryValidator;
            _detailValidator = detailValidator;
        }

        #region 查询库存汇总分页列表 GetInventorySummaryListAsync
        public async Task<ApiResponse<PageResponse<InventorySummaryResponse>>> GetInventorySummaryListAsync(GetInventorySummaryListRequest request)
        {
            if (request == null)
            {
                return ApiResponse<PageResponse<InventorySummaryResponse>>.Error("库存汇总查询请求不能为空", 400);
            }
            var validationResult = await _summaryValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ApiResponse<PageResponse<InventorySummaryResponse>>.Error(validationResult.Errors[0].ErrorMessage, 400);
            }
            var companyID = _currentUserService.CompanyID;
            PaginationHelper.Normalize(request);
            var result = await _inventoryRepository.GetInventorySummaryListAsync(request, companyID);
            return ApiResponse<PageResponse<InventorySummaryResponse>>.Success(result, "查询库存汇总成功");
        }
        #endregion

        #region 查询库存明细分页列表 GetInventoryDetailListAsync
        public async Task<ApiResponse<PageResponse<InventoryDetailResponse>>> GetInventoryDetailListAsync(GetInventoryDetailListRequest request)
        {
            if (request == null)
            {
                return ApiResponse<PageResponse<InventoryDetailResponse>>.Error("库存明细查询请求不能为空", 400);
            }
            var validationResult = await _detailValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ApiResponse<PageResponse<InventoryDetailResponse>>.Error(validationResult.Errors[0].ErrorMessage, 400);
            }
            var companyID = _currentUserService.CompanyID;
            PaginationHelper.Normalize(request);
            var result = await _inventoryRepository.GetInventoryDetailListAsync(request, companyID);
            return ApiResponse<PageResponse<InventoryDetailResponse>>.Success(result, "查询库存明细成功");
        }
        #endregion
    }
}
