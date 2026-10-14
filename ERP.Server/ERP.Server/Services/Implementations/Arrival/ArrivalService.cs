using System.Data;
using ERP.Server.Common;
using ERP.Server.Common.Helpers;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Entity.Inventory;
using ERP.Server.Models.Entity.InventoryTransaction;
using ERP.Server.Models.Enums;
using ERP.Server.Models.Request.Arrival;
using ERP.Server.Models.Response.Arrival;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;
using ERP.Server.Services.Interfaces.Arrival;
using ERP.Server.Services.Interfaces.BusinessCode;
using FluentValidation;

namespace ERP.Server.Services.Implementations
{
    public class ArrivalService : IArrivalService
    {
        private readonly IArrivalRepository _arrivalRepository;
        private readonly IDbConnection _dbConnection;
        private readonly ICurrentUserService _currentUserService;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IBusinessCodeService _businessCodeService;
        private readonly IValidator<AddArrivalRequest> _addArrivalValidator;
        private readonly IValidator<UpdateArrivalRequest> _updateArrivalValidator;

        public ArrivalService(IArrivalRepository arrivalRepository, IDbConnection dbConnection , ICurrentUserService currentUserService, IInventoryRepository inventoryRepository, IBusinessCodeService businessCodeService, IValidator<AddArrivalRequest> addArrivalValidator, IValidator<UpdateArrivalRequest> updateArrivalValidator)
        {
            _arrivalRepository = arrivalRepository;
            _dbConnection = dbConnection;
            _currentUserService = currentUserService;
            _inventoryRepository = inventoryRepository;
            _businessCodeService = businessCodeService;
            _addArrivalValidator = addArrivalValidator;
            _updateArrivalValidator = updateArrivalValidator;
        }

        #region 新增到货单AddArrivalAsync
        public async Task<ApiResponse<bool>> AddArrivalAsync(AddArrivalRequest request)
        {
            if(request == null)
            {
                return ApiResponse<bool>.Error("新增到货单请求不能为空");
            }
            var validationResult = await _addArrivalValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ApiResponse<bool>.Error(validationResult.Errors[0].ErrorMessage);
            }
            var companyID = _currentUserService.CompanyID;
            var createUserID = _currentUserService.UserID;
            if(_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                var supplier = await _arrivalRepository.GetSupplierByIDAsync(request.SupplierID, transaction);
                if(supplier == null)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("供应商不存在，无法新增到货单");
                }
                if (!supplier.IsActive)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error($"供应商【{supplier.SupplierName}】已停用，无法新增到货单");
                }
                var location = await _arrivalRepository.GetLocationByIDAsync(request.LocationID, companyID, transaction);
                if (location == null)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("站点不存在或不属于当前公司，无法新增到货单");
                }
                if (!location.IsActive)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error($"站点【{location.LocationName}】已停用，无法新增到货单");
                }
                var arrivalDetails = new List<ArrivalDetail>();
                List<int> goodsUnitIDs = request.ArrivalDetails.Select(x => x.GoodsUnitID).Distinct().ToList();
                var goodsUnits = await _arrivalRepository.GetGoodsUnitsByIDsAsync(goodsUnitIDs, transaction);
                foreach(var detail in request.ArrivalDetails)
                {
                    var goodsUnit = goodsUnits.FirstOrDefault(x => x.GoodsUnitID == detail.GoodsUnitID);
                    if (goodsUnit == null)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error($"商品单位关系【{detail.GoodsUnitID}】不存在，无法新增到货单");
                    }
                    if (!goodsUnit.GoodsIsActive)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error($"商品【{goodsUnit.GoodsTitle}】已停用，无法新增到货单");
                    }
                    if (!goodsUnit.GoodsUnitIsActive)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error($"商品单位关系【{goodsUnit.GoodsTitle} - {goodsUnit.UnitName}】已停用，无法新增到货单");
                    }
                    if (!goodsUnit.UnitIsActive)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error($"单位【{goodsUnit.UnitName}】已停用，无法新增到货单");
                    }
                    if(goodsUnit.ConversionRate <= 0)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error($"商品单位关系【{goodsUnit.GoodsTitle} - {goodsUnit.UnitName}】的转换率必须大于0，无法新增到货单");
                    }
                    decimal baseQuantity = detail.Quantity * goodsUnit.ConversionRate;
                    // ① 正向计算：数量 × 采购单位单价
                    decimal calculatedAmount = Math.Round(detail.Quantity * detail.PurchaseUnitCost, 2, MidpointRounding.AwayFromZero);
                    if(calculatedAmount != detail.Amount)
                    {
                        decimal calculatedPrice = Math.Round(detail.Amount / detail.Quantity, 6, MidpointRounding.AwayFromZero);
                        decimal amountDifference = Math.Abs(calculatedAmount - detail.Amount);
                        if (calculatedPrice != detail.PurchaseUnitCost || amountDifference > 0.01m)
                        {
                            transaction.Rollback();
                            return ApiResponse<bool>.Error($"商品【{goodsUnit.GoodsTitle}】的金额计算不一致，请检查数量、采购单位单价和金额的关系，无法新增到货单");
                        }
                    }
                    decimal unitCost = Math.Round(detail.Amount / baseQuantity, 6, MidpointRounding.AwayFromZero);
                    var arrivalDetail = new ArrivalDetail
                    {
                        GoodsID = goodsUnit.GoodsID,
                        GoodsUnitID = goodsUnit.GoodsUnitID,
                        Quantity = detail.Quantity,
                        PurchaseUnitCost = detail.PurchaseUnitCost,
                        Amount = detail.Amount,
                        BaseQuantity = baseQuantity,
                        UnitCost = unitCost,
                        BarCode = goodsUnit.BarCode,
                        GoodsCode = goodsUnit.GoodsCode,
                        GoodsTitle = goodsUnit.GoodsTitle,
                        GoodsSubTitle = goodsUnit.GoodsSubTitle,
                        UnitID = goodsUnit.UnitID,
                        UnitCode = goodsUnit.UnitCode,
                        UnitName = goodsUnit.UnitName,
                        ConversionRate = goodsUnit.ConversionRate,
                        Remark = detail.Remark,
                        CreateUserID = createUserID
                    };
                    arrivalDetails.Add(arrivalDetail);
                }
                decimal totalAmount = arrivalDetails.Sum(x => x.Amount);
                var codeResult = await _businessCodeService.GetNextCodeAsync(BusinessCodeType.Arrival, transaction);
                if(codeResult == null)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("到货单编号生成失败，无法新增到货单");
                }
                if(codeResult.Code != 200 || string.IsNullOrWhiteSpace(codeResult.Data))
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error(string.IsNullOrWhiteSpace(codeResult.Message) 
                                                    ? "到货单编号生成失败或编号为空，无法新增到货单" 
                                                    : $"到货单编号生成失败：{codeResult.Message}，无法新增到货单"
                                                   );
                }
                var arrivalcode = codeResult.Data;
                var arrival = new Arrival
                {
                    ArrivalCode = arrivalcode,
                    SupplierID = supplier.SupplierID,
                    CompanyID = companyID,
                    LocationID = location.LocationID,
                    ArrivalStatus = ArrivalStatus.Pending,
                    ArrivalTime = request.ArrivalTime,
                    TotalAmount = totalAmount,
                    Remark = request.Remark,
                    CreateUserID = createUserID
                };
                int arrivalID = await _arrivalRepository.AddArrivalAsync(arrival, transaction);
                if(arrivalID <= 0)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("到货单主表新增失败，无法新增到货单");
                }
                foreach(var detail in arrivalDetails)
                {
                    detail.ArrivalID = arrivalID;
                }
                var addDetailsResult = await _arrivalRepository.AddArrivalDetailsAsync(arrivalDetails, transaction);
                if (!addDetailsResult)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("到货明细表新增失败，无法新增到货单");
                }
                transaction.Commit();
                return ApiResponse<bool>.Success(true, "到货单新增成功");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        #endregion

        #region 查询到货单列表GetArrivalListAsync
        public async Task<ApiResponse<PageResponse<ArrivalListResponse>>> GetArrivalListAsync(GetArrivalListRequest request)
        {
            var companyID = _currentUserService.CompanyID;
            PaginationHelper.Normalize(request);
            if (request.StartDate.HasValue && request.EndDate.HasValue && request.EndDate.Value < request.StartDate.Value)
            {
                return ApiResponse<PageResponse<ArrivalListResponse>>.Error("结束日期不能早于开始日期");
            }
            request.StartDate = request.StartDate?.Date;
            request.EndDate = request.EndDate?.Date.AddDays(1);
            var totalCount = await _arrivalRepository.GetArrivalCountAsync(request, companyID);
            var arrivalList = await _arrivalRepository.GetArrivalListAsync(request, companyID);
            var response = new PageResponse<ArrivalListResponse>
            {
                TotalCount = totalCount,
                List = arrivalList
            };
            return ApiResponse<PageResponse<ArrivalListResponse>>.Success(response,"查询到货单列表成功");
        }
        #endregion

        #region 查询到货单详情GetArrivalDetailAsync
        public async Task<ApiResponse<ArrivalDetailResponse>> GetArrivalDetailAsync(GetArrivalDetailRequest request)
        {
            var companyID = _currentUserService.CompanyID;
            PaginationHelper.Normalize(request);
            var detail = await _arrivalRepository.GetArrivalDetailAsync(request.ArrivalID, companyID);
            if (detail == null)
            {
                return ApiResponse<ArrivalDetailResponse>.Error("到货单不存在");
            }
            var detailCount = await _arrivalRepository.GetArrivalDetailItemCountAsync(detail.ArrivalID, companyID);
            var detailItems = await _arrivalRepository.GetArrivalDetailItemsAsync(request, companyID, detail.LocationID);
            var detailPage = new PageResponse<ArrivalDetailItemResponse>
            {
                TotalCount = detailCount,
                List = detailItems
            };
            detail.ArrivalDetails = detailPage;
            return ApiResponse<ArrivalDetailResponse>.Success(detail, "查询到货单详情成功");
        }
        #endregion

        #region 审核到货单UpdateArrivalAuditAsync
        public async Task<ApiResponse<bool>> UpdateArrivalAuditAsync(UpdateArrivalAuditRequest request)
        {
            var companyID = _currentUserService.CompanyID;
            var arrival = await _arrivalRepository.GetArrivalByIDAsync(request.ArrivalID, companyID);
            if (arrival == null)
            {
                return ApiResponse<bool>.Error("到货单不存在");
            }
            if(arrival.ArrivalStatus != ArrivalStatus.Pending)
            {
                return ApiResponse<bool>.Error("只有待审核的到货单才能进行审核");
            }
            var auditUserID = _currentUserService.UserID; 
            var targetStatus = ArrivalStatus.Audited;
            var currentStatus = ArrivalStatus.Pending;
            return await _arrivalRepository.UpdateArrivalAuditAsync(request.ArrivalID, companyID, auditUserID, currentStatus, targetStatus)
                ? ApiResponse<bool>.Success(true, "审核到货单成功")
                : ApiResponse<bool>.Error("审核到货单失败");
        }
        #endregion

        #region 到货单入库StockInArrivalAsync
        public async Task<ApiResponse<bool>> StockInArrivalAsync(StockInArrivalRequest request)
        {
            var companyID = _currentUserService.CompanyID;
            var arrival = await _arrivalRepository.GetArrivalByIDAsync(request.ArrivalID, companyID);
            if (arrival == null)
            {
                return ApiResponse<bool>.Error("到货单不存在");
            }
            if(arrival.ArrivalStatus != ArrivalStatus.Audited)
            {
                return ApiResponse<bool>.Error("只有已审核的到货单才能入库");
            }
            var location = await _arrivalRepository.GetLocationByIDAsync(arrival.LocationID);
            if (location == null) 
            {
                return ApiResponse<bool>.Error("当前站点不存在");
            }
            if (location.CompanyID != companyID)
            {
                return ApiResponse<bool>.Error("不允许跨公司进行入库操作");
            }
            if (!location.IsActive)
            {
                return ApiResponse<bool>.Error("当前站点已经停用");
            }
            if (!location.CanStock)
            {
                return ApiResponse<bool>.Error("当前站点不允许库存");
            }
            if(_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                var arrivalDetails = await _arrivalRepository.GetArrivalDetailsByArrivalIDAsync(request.ArrivalID, transaction);
                if (arrivalDetails == null || arrivalDetails.Count == 0)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("到货单明细不存在");
                }
                foreach (var detail in arrivalDetails)
                {
                    var inventory = await _inventoryRepository.GetInventoryAsync(companyID, arrival.LocationID, detail.GoodsID, arrival.SupplierID, transaction);
                    int inventoryID;
                    if (inventory == null)
                    {
                        var newInventory = new Inventory
                        {
                            CompanyID = companyID,
                            LocationID = arrival.LocationID,
                            GoodsID = detail.GoodsID,
                            SupplierID = arrival.SupplierID,
                            Quantity = detail.BaseQuantity,
                            InventoryAmount = detail.Amount,
                            AverageCost = detail.Amount / detail.BaseQuantity,
                            CreateUserID = _currentUserService.UserID,
                        };
                        inventoryID = await _inventoryRepository.AddInventoryAsync(newInventory, transaction);
                    }
                    else
                    {
                        inventoryID = inventory.InventoryID;
                        var newQuantity = inventory.Quantity + detail.BaseQuantity;
                        var newInventoryAmount = inventory.InventoryAmount + detail.Amount;
                        var newAverageCost = newInventoryAmount / newQuantity;
                        var updateInventory = new Inventory
                        {
                            InventoryID = inventory.InventoryID,
                            Quantity = newQuantity,
                            InventoryAmount = newInventoryAmount,
                            AverageCost = newAverageCost,
                            UpdateUserID = _currentUserService.UserID
                        };
                        var updateInventoryResult = await _inventoryRepository.UpdateInventoryAsync(updateInventory, transaction);
                        if (!updateInventoryResult)
                        {
                            transaction.Rollback();
                            return ApiResponse<bool>.Error("库存更新失败");
                        }
                    }
                    var inventoryTransaction = new InventoryTransaction
                    {
                        InventoryID = inventoryID,
                        CompanyID = companyID,
                        LocationID = arrival.LocationID,
                        GoodsID = detail.GoodsID,
                        SupplierID = arrival.SupplierID,
                        TransactionType = InventoryTransactionType.ArrivalStockIn,
                        SourceID = arrival.ArrivalID,
                        SourceDetailID = detail.ArrivalDetailID,
                        SourceCode = arrival.ArrivalCode,
                        BeforeQuantity = inventory == null ? 0 : inventory.Quantity,
                        QuantityChange = detail.BaseQuantity,
                        AfterQuantity = inventory == null ? detail.BaseQuantity : inventory.Quantity + detail.BaseQuantity,
                        BeforeInventoryAmount = inventory == null ? 0 : inventory.InventoryAmount,
                        AmountChange = detail.Amount,
                        AfterInventoryAmount = inventory == null ? detail.Amount : inventory.InventoryAmount + detail.Amount,
                        UnitCost = detail.Amount / detail.BaseQuantity,
                        BeforeAverageCost = inventory == null ? 0 : inventory.AverageCost,
                        AfterAverageCost = inventory == null ? detail.Amount / detail.BaseQuantity : (inventory.InventoryAmount + detail.Amount) / (inventory.Quantity + detail.BaseQuantity),
                        Remark = string.IsNullOrWhiteSpace(detail.Remark) ? $"到货单【{arrival.ArrivalCode}】入库" : $"到货单【{arrival.ArrivalCode}】入库，{detail.Remark}",
                        CreateUserID = _currentUserService.UserID
                    };
                    var addTransactionResult = await _inventoryRepository.AddInventoryTransactionAsync(inventoryTransaction, transaction);
                    if (!addTransactionResult)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error("库存流水写入失败");
                    }
                }
                var updateArrivalResult = await _arrivalRepository.UpdateArrivalStockInAsync(arrival.ArrivalID, companyID, _currentUserService.UserID, ArrivalStatus.Audited, ArrivalStatus.Stocked, transaction);
                if (!updateArrivalResult)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("更新到货单状态失败");
                }
                transaction.Commit();
                return ApiResponse<bool>.Success(true, "到货单入库成功");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        #endregion

        #region 撤销到货入库CancelStockInArrivalAsync
        public async Task<ApiResponse<bool>> CancelStockInArrivalAsync(CancelStockInArrivalRequest request)
        {
            var companyID = _currentUserService.CompanyID;
            var arrival = await _arrivalRepository.GetArrivalByIDAsync(request.ArrivalID, companyID);
            if (arrival == null)
            {
                return ApiResponse<bool>.Error("到货单不存在,无法撤销入库");
            }
            if (arrival.ArrivalStatus != ArrivalStatus.Stocked)
            {
                return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】当前状态不是已入库,无法撤销入库");
            }
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                var arrivalDetails = await _arrivalRepository.GetArrivalDetailsByArrivalIDAsync(request.ArrivalID, transaction);
                if (arrivalDetails == null || arrivalDetails.Count == 0)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】明细不存在,无法撤销入库");
                }
                foreach (var detail in arrivalDetails)
                {
                    var inventory = await _inventoryRepository.GetInventoryAsync(companyID, arrival.LocationID, detail.GoodsID, arrival.SupplierID, transaction);
                    if (inventory == null)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】库存不存在,无法撤销入库");
                    }
                    var inventoryTransaction = await _inventoryRepository.GetArrivalStockInTransactionAsync(arrival.ArrivalID, detail.ArrivalDetailID, InventoryTransactionType.ArrivalStockIn, transaction);
                    if (inventoryTransaction == null)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】库存流水不存在,无法撤销入库");
                    }
                    if (inventory.Quantity < inventoryTransaction.QuantityChange)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】库存不足,无法撤销入库");
                    }
                    var afterQuantity = inventory.Quantity - inventoryTransaction.QuantityChange;
                    var afterInventoryAmount = inventory.InventoryAmount - inventoryTransaction.AmountChange;
                    if (afterInventoryAmount < 0)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】库存金额不足,无法撤销入库");
                    }
                    var afterAverageCost = afterQuantity == 0 ? 0 : afterInventoryAmount / afterQuantity;
                    var updateInventory = new Inventory
                    {
                        InventoryID = inventory.InventoryID,
                        Quantity = afterQuantity,
                        InventoryAmount = afterInventoryAmount,
                        AverageCost = afterAverageCost,
                        UpdateUserID = _currentUserService.UserID
                    };
                    var updateInventoryResult = await _inventoryRepository.UpdateInventoryAsync(updateInventory, transaction);
                    if (!updateInventoryResult)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】库存更新失败,无法撤销入库");
                    }
                    var cancelStockInTransaction = new InventoryTransaction
                    {
                        InventoryID = inventory.InventoryID,
                        CompanyID = _currentUserService.CompanyID,
                        LocationID = inventory.LocationID,
                        SupplierID = inventory.SupplierID,
                        GoodsID = inventory.GoodsID,
                        TransactionType = InventoryTransactionType.ArrivalStockInCancel,
                        SourceID = arrival.ArrivalID,
                        SourceDetailID = detail.ArrivalDetailID,
                        SourceCode = arrival.ArrivalCode,
                        BeforeQuantity = inventory.Quantity,
                        QuantityChange = -inventoryTransaction.QuantityChange,
                        AfterQuantity = afterQuantity,
                        BeforeInventoryAmount = inventory.InventoryAmount,
                        AmountChange = -inventoryTransaction.AmountChange,
                        AfterInventoryAmount = afterInventoryAmount,
                        UnitCost = inventoryTransaction.UnitCost,
                        BeforeAverageCost = inventory.AverageCost,
                        AfterAverageCost = afterAverageCost,
                        Remark = string.IsNullOrWhiteSpace(detail.Remark) ? $"撤销到货单【{arrival.ArrivalCode}】入库" : $"撤销到货单【{arrival.ArrivalCode}】入库，原备注：{detail.Remark}",
                        CreateUserID = _currentUserService.UserID,
                    };
                    var addTransactionResult = await _inventoryRepository.AddInventoryTransactionAsync(cancelStockInTransaction, transaction);
                    if (!addTransactionResult)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】撤销入库流水写入失败,无法撤销入库");
                    }
                }
                var updateArrivalResult = await _arrivalRepository.UpdateArrivalCancelStockInAsync(request.ArrivalID, companyID, _currentUserService.UserID , ArrivalStatus.Stocked, ArrivalStatus.Audited, transaction);
                if (!updateArrivalResult)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】状态修改失败,无法撤销入库");
                }
                transaction.Commit();
                return ApiResponse<bool>.Success(true, $"到货单【{arrival.ArrivalCode}】撤销入库成功");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        #endregion

        #region 反审核到货单CancelArrivalAuditAsync
        public async Task<ApiResponse<bool>> CancelArrivalAuditAsync(UpdateArrivalAuditRequest request)
        {
            var companyID = _currentUserService.CompanyID;
            var arrival = await _arrivalRepository.GetArrivalByIDAsync(request.ArrivalID, companyID);
            if (arrival == null)
            {
                return ApiResponse<bool>.Error("到货单不存在,无法反审核");
            }
            if (arrival.ArrivalStatus != ArrivalStatus.Audited)
            {
                return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】当前状态不是已审核,无法进行反审核");
            }
            var updateResult = await _arrivalRepository.UpdateArrivalCancelAuditAsync(request.ArrivalID, companyID, _currentUserService.UserID, ArrivalStatus.Audited, ArrivalStatus.Pending);
            if (!updateResult)
            {
                return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】反审核失败");
            }
            return ApiResponse<bool>.Success(true, $"到货单【{arrival.ArrivalCode}】反审核成功");
        }
        #endregion

        #region 修改到货单UpdateArrivalAsync
        public async Task<ApiResponse<bool>> UpdateArrivalAsync(UpdateArrivalRequest request)
        {
            if (request == null)
            {
                return ApiResponse<bool>.Error("修改到货单请求不能为空");
            }
            var validationResult = await _updateArrivalValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ApiResponse<bool>.Error(validationResult.Errors[0].ErrorMessage);
            }
            var companyID = _currentUserService.CompanyID;
            var arrival = await _arrivalRepository.GetArrivalByIDAsync(request.ArrivalID, companyID);
            if(arrival == null)
            {
                return ApiResponse<bool>.Error("此到货单不存在");
            }
            if(arrival.ArrivalStatus != ArrivalStatus.Pending)
            {
                return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】当前状态不是未审核,无法修改到货单");
            }
            var supplier = await _arrivalRepository.GetSupplierByIDAsync(request.SupplierID);
            if (supplier == null)
            {
                return ApiResponse<bool>.Error("此供应商不存在");
            }
            if (!supplier.IsActive)
            {
                return ApiResponse<bool>.Error($"供应商【{supplier.SupplierName}】已停用,无法修改到货单");
            }
            var location = await _arrivalRepository.GetLocationByIDAsync(request.LocationID);
            if(location == null)
            {
                return ApiResponse<bool>.Error("此站点不存在");
            }
            if (!location.IsActive)
            {
                return ApiResponse<bool>.Error($"站点【{location.LocationName}】已停用,无法修改到货单");
            }
            if (location.CompanyID != _currentUserService.CompanyID)
            {
                return ApiResponse<bool>.Error($"站点【{location.LocationName}】不属于当前公司,无法修改到货单");
            }
            var arrivalDetails = new List<ArrivalDetail>();
            foreach (var detail in request.ArrivalDetails) 
            {
                var goodsUnit = await _arrivalRepository.GetGoodsUnitByIDAsync(detail.GoodsUnitID);
                if (goodsUnit == null)
                {
                    return ApiResponse<bool>.Error("商品单位关系不存在");
                }
                if (!goodsUnit.GoodsIsActive)
                {
                    return ApiResponse<bool>.Error("商品已停用");
                }
                if (!goodsUnit.UnitIsActive)
                {
                    return ApiResponse<bool>.Error("单位已停用");
                }
                if (!goodsUnit.GoodsUnitIsActive)
                {
                    return ApiResponse<bool>.Error("商品单位关系已停用");
                }
                if (goodsUnit.ConversionRate <= 0)
                {
                    return ApiResponse<bool>.Error("商品单位换算率必须大于0");
                }
                var baseQuantity = detail.Quantity * goodsUnit.ConversionRate;
                var unitCost = detail.Amount / baseQuantity;
                var arrivalDetail = new ArrivalDetail
                {
                    ArrivalID = request.ArrivalID,
                    GoodsID = goodsUnit.GoodsID,
                    GoodsUnitID = detail.GoodsUnitID,
                    Quantity = detail.Quantity,
                    UnitCost = unitCost,
                    Amount = detail.Amount,
                    Remark = detail.Remark,
                    CreateUserID = _currentUserService.UserID,
                    BarCode = goodsUnit.BarCode,
                    GoodsCode = goodsUnit.GoodsCode,
                    GoodsTitle = goodsUnit.GoodsTitle,
                    GoodsSubTitle = goodsUnit.GoodsSubTitle,
                    UnitID = goodsUnit.UnitID,
                    UnitCode = goodsUnit.UnitCode,
                    UnitName = goodsUnit.UnitName,
                    ConversionRate = goodsUnit.ConversionRate,
                    BaseQuantity = baseQuantity
                };
                arrivalDetails.Add(arrivalDetail);
            }
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                var currentArrival = await _arrivalRepository.GetArrivalForUpdateAsync(request.ArrivalID, companyID, transaction);
                if (currentArrival == null)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("到货单数据已发生变化,请刷新后重试");
                }
                if(currentArrival.ArrivalStatus != ArrivalStatus.Pending)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error($"到货单【{currentArrival.ArrivalCode}】数据已发生变化,请刷新后重试");
                }
                var updateArrival = new Arrival
                {
                    ArrivalID = request.ArrivalID,
                    SupplierID = request.SupplierID,
                    LocationID = request.LocationID,
                    ArrivalTime = request.ArrivalTime,
                    TotalAmount = arrivalDetails.Sum(x => x.Amount),
                    Remark = request.Remark,
                    UpdateUserID = _currentUserService.UserID
                };
                var updateResult = await _arrivalRepository.UpdateArrivalAsync(updateArrival, companyID, ArrivalStatus.Pending, transaction);
                if (!updateResult)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error($"到货单【{currentArrival.ArrivalCode}】主表修改失败,无法修改到货单");
                }
                var deleteResult = await _arrivalRepository.DeleteArrivalDetailsAsync(request.ArrivalID, companyID, transaction);
                if (!deleteResult) 
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error($"到货单【{currentArrival.ArrivalCode}】旧明细删除失败,无法修改到货单");
                }
                var addResult = await _arrivalRepository.AddArrivalDetailsAsync(arrivalDetails, transaction);
                if (!addResult)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error($"到货单【{currentArrival.ArrivalCode}】新明细写入失败,无法修改到货单");
                }
                transaction.Commit();
                return ApiResponse<bool>.Success(true, "到货单修改成功");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        #endregion
    }
}