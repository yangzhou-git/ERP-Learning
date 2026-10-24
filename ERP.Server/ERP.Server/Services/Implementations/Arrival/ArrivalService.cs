using System.Data;
using System.Data.Common;
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
using Microsoft.VisualBasic;

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
            var DbConnection = (DbConnection)_dbConnection;
            if (DbConnection.State != ConnectionState.Open)
            {
                await DbConnection.OpenAsync();
            }
            await using var transaction = await DbConnection.BeginTransactionAsync();
            try
            {
                var supplier = await _arrivalRepository.GetSupplierByIDAsync(request.SupplierID, transaction);
                if(supplier == null)
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error("供应商不存在，无法新增到货单");
                }
                if (!supplier.IsActive)
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error($"供应商【{supplier.SupplierName}】已停用，无法新增到货单");
                }
                var location = await _arrivalRepository.GetLocationByIDAsync(request.LocationID, companyID, transaction);
                if (location == null)
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error("站点不存在或不属于当前公司，无法新增到货单");
                }
                if (!location.IsActive)
                {
                    await transaction.RollbackAsync();
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
                        await transaction.RollbackAsync();
                        return ApiResponse<bool>.Error($"商品单位关系【{detail.GoodsUnitID}】不存在，无法新增到货单");
                    }
                    if (!goodsUnit.GoodsIsActive)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<bool>.Error($"商品【{goodsUnit.GoodsTitle}】已停用，无法新增到货单");
                    }
                    if (!goodsUnit.GoodsUnitIsActive)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<bool>.Error($"商品单位关系【{goodsUnit.GoodsTitle} - {goodsUnit.UnitName}】已停用，无法新增到货单");
                    }
                    if (!goodsUnit.UnitIsActive)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<bool>.Error($"单位【{goodsUnit.UnitName}】已停用，无法新增到货单");
                    }
                    if(goodsUnit.ConversionRate <= 0)
                    {
                        await transaction.RollbackAsync();
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
                            await transaction.RollbackAsync();
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
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error("到货单编号生成失败，无法新增到货单");
                }
                if(codeResult.Code != 200 || string.IsNullOrWhiteSpace(codeResult.Data))
                {
                    await transaction.RollbackAsync();
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
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error("到货单主表新增失败，无法新增到货单");
                }
                foreach(var detail in arrivalDetails)
                {
                    detail.ArrivalID = arrivalID;
                }
                var addDetailsResult = await _arrivalRepository.AddArrivalDetailsAsync(arrivalDetails, transaction);
                if (!addDetailsResult)
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error("到货明细表新增失败，无法新增到货单");
                }
                await transaction.CommitAsync();
                return ApiResponse<bool>.Success(true, "到货单新增成功");
            }
            catch
            {
                await transaction.RollbackAsync();
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
                return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】当前不是待审核状态,无法审核");
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
            var DbConnection = (DbConnection)_dbConnection;
            if (DbConnection.State != ConnectionState.Open)
            {
                await DbConnection.OpenAsync();
            }
            await using var transaction = await DbConnection.BeginTransactionAsync();
            try
            {
                var arrival = await _arrivalRepository.GetArrivalForUpdateAsync(request.ArrivalID, companyID, transaction);
                if (arrival == null)
                {
                    return ApiResponse<bool>.Error("到货单不存在,无法入库");
                }
                if (arrival.ArrivalStatus != ArrivalStatus.Audited)
                {
                    return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】当前状态不是已审核,无法入库");
                }
                var location = await _arrivalRepository.GetLocationByIDAsync(arrival.LocationID, companyID, transaction);
                if(location == null)
                {
                    return ApiResponse<bool>.Error($"站点【{arrival.LocationID}】不存在,无法入库");
                }
                if(!location.IsActive)
                {
                    return ApiResponse<bool>.Error($"站点【{location.LocationName}】已停用,无法入库");
                }
                if(!location.CanStock)
                {
                    return ApiResponse<bool>.Error($"站点【{location.LocationName}】不允许入库,无法入库");
                }
                var arrivalDetails = await _arrivalRepository.GetArrivalDetailsByArrivalIDAsync(arrival.ArrivalID, transaction);
                if (arrivalDetails == null || arrivalDetails.Count == 0)
                {
                    return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】明细不存在,无法入库");
                }
                foreach (var detail in arrivalDetails)
                {
                    if (detail.BaseQuantity <= 0 || detail.Amount < 0)
                    {
                        return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】明细商品【{detail.GoodsTitle}】数量或金额不合法,无法入库");
                    }
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
                            AverageCost = detail.UnitCost,
                            CreateUserID = _currentUserService.UserID
                        };
                        inventoryID = await _inventoryRepository.AddInventoryAsync(newInventory, transaction);
                        if (inventoryID <= 0)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】新增库存失败,无法入库");
                        }
                    }
                    else
                    {
                        inventoryID = inventory.InventoryID;
                        var afterQuantity = inventory.Quantity + detail.BaseQuantity;
                        var afterInventoryAmount = inventory.InventoryAmount + detail.Amount;
                        if (afterQuantity <= 0)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】入库后库存数量异常,无法入库");
                        }
                        var afterAverageCost = afterInventoryAmount / afterQuantity;
                        var updateInventory = new Inventory
                        {
                            InventoryID = inventory.InventoryID,
                            Quantity = afterQuantity,
                            InventoryAmount = afterInventoryAmount,
                            AverageCost = afterAverageCost,
                            UpdateUserID = _currentUserService.UserID
                        };
                        var updateInventoryResult = await _inventoryRepository.UpdateInventoryAsync(updateInventory, companyID, transaction);
                        if (!updateInventoryResult)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】库存更新失败,无法入库");
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
                        BeforeQuantity = inventory?.Quantity ?? 0,
                        QuantityChange = detail.BaseQuantity,
                        AfterQuantity = (inventory?.Quantity ?? 0) + detail.BaseQuantity,
                        BeforeInventoryAmount = inventory?.InventoryAmount ?? 0,
                        AmountChange = detail.Amount,
                        AfterInventoryAmount = (inventory?.InventoryAmount ?? 0) + detail.Amount,
                        UnitCost = detail.UnitCost,
                        BeforeAverageCost = (inventory?.AverageCost ?? 0),
                        AfterAverageCost = inventory == null ? detail.UnitCost : (inventory.InventoryAmount + detail.Amount) / (inventory.Quantity + detail.BaseQuantity),
                        Remark = string.IsNullOrWhiteSpace(detail.Remark) ? $"到货单【{arrival.ArrivalCode}】入库" : $"到货单【{arrival.ArrivalCode}】入库，原备注：{detail.Remark}",
                        CreateUserID = _currentUserService.UserID
                    };
                    var addTransactionResult = await _inventoryRepository.AddInventoryTransactionAsync(inventoryTransaction, transaction);
                    if (!addTransactionResult)
                    {
                        return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】商品【{detail.GoodsTitle}】库存流水写入失败,无法入库");
                    }
                }
                var updateArrivalResult = await _arrivalRepository.UpdateArrivalStockInAsync(arrival.ArrivalID, companyID, _currentUserService.UserID, ArrivalStatus.Audited, ArrivalStatus.Stocked, transaction);
                if (!updateArrivalResult)
                {
                    return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】状态修改失败,无法入库");
                }
                await transaction.CommitAsync();
                return ApiResponse<bool>.Success(true, $"到货单【{arrival.ArrivalCode}】入库成功");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion

        #region 撤销到货入库CancelStockInArrivalAsync
        public async Task<ApiResponse<bool>> CancelStockInArrivalAsync(CancelStockInArrivalRequest request)
        {
            var companyID = _currentUserService.CompanyID;
            var DbConnection = (DbConnection)_dbConnection;
            if (DbConnection.State != ConnectionState.Open)
            {
                await DbConnection.OpenAsync();
            }
            await using var transaction = await DbConnection.BeginTransactionAsync();
            try
            {
                var arrival = await _arrivalRepository.GetArrivalForUpdateAsync(request.ArrivalID, companyID, transaction);
                if (arrival == null)
                {
                    return ApiResponse<bool>.Error("到货单不存在,无法撤销入库");
                }
                if (arrival.ArrivalStatus != ArrivalStatus.Stocked)
                {
                    return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】当前状态不是已入库,无法撤销入库");
                }
                var arrivalDetails = await _arrivalRepository.GetArrivalDetailsByArrivalIDAsync(request.ArrivalID, transaction);
                if (arrivalDetails == null || arrivalDetails.Count == 0)
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error($"到货单【{arrival.ArrivalCode}】明细不存在,无法撤销入库");
                }
                foreach (var detail in arrivalDetails)
                {
                    var inventory = await _inventoryRepository.GetInventoryAsync(companyID, arrival.LocationID, detail.GoodsID, arrival.SupplierID, transaction);
                    if (inventory == null)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】库存不存在,无法撤销入库");
                    }
                    var inventoryTransaction = await _inventoryRepository.GetArrivalStockInTransactionAsync(arrival.ArrivalID, detail.ArrivalDetailID, companyID, InventoryTransactionType.ArrivalStockIn, transaction);
                    if (inventoryTransaction == null)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】库存流水不存在,无法撤销入库");
                    }
                    var subsequentTransactions = await _inventoryRepository.GetSubsequentInventoryTransactionsAsync(inventory.InventoryID, companyID, inventoryTransaction.InventoryTransactionID, transaction);
                    // 判断本次入库后，同一库存是否发生过其他库存流水
                    // 无后续流水：校验一致性后，直接恢复本次入库前的库存快照
                    // 有后续流水：需要重新核算后续业务对库存数量、金额和成本的影响
                    if (subsequentTransactions.Count == 0)
                    {
                        if (inventory.InventoryID != inventoryTransaction.InventoryID
                            || inventory.Quantity != inventoryTransaction.AfterQuantity
                            || inventory.InventoryAmount != inventoryTransaction.AfterInventoryAmount
                            || inventory.AverageCost != inventoryTransaction.AfterAverageCost)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】当前库存数量或库存金额与原始入库流水不一致,无法撤销入库");
                        }
                        var updateInventory = new Inventory
                        {
                            InventoryID = inventory.InventoryID,
                            Quantity = inventoryTransaction.BeforeQuantity,
                            InventoryAmount = inventoryTransaction.BeforeInventoryAmount,
                            AverageCost = inventoryTransaction.BeforeAverageCost,
                            UpdateUserID = _currentUserService.UserID
                        };
                        var updateInventoryResult = await _inventoryRepository.UpdateInventoryAsync(updateInventory, companyID, transaction);
                        if (!updateInventoryResult)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】库存更新失败,无法撤销入库");
                        }
                        var cancelStockInTransaction = new InventoryTransaction
                        {
                            InventoryID = inventory.InventoryID,
                            CompanyID = companyID,
                            LocationID = arrival.LocationID,
                            GoodsID = detail.GoodsID,
                            TransactionType = InventoryTransactionType.ArrivalStockInCancel,
                            SourceID = arrival.ArrivalID,
                            SourceDetailID = detail.ArrivalDetailID,
                            SourceCode = arrival.ArrivalCode,
                            BeforeQuantity = inventory.Quantity,
                            QuantityChange = -inventoryTransaction.QuantityChange,
                            AfterQuantity = inventoryTransaction.BeforeQuantity,
                            BeforeInventoryAmount = inventory.InventoryAmount,
                            AmountChange = -inventoryTransaction.AmountChange,
                            AfterInventoryAmount = inventoryTransaction.BeforeInventoryAmount,
                            BeforeAverageCost = inventory.AverageCost,
                            AfterAverageCost = inventoryTransaction.BeforeAverageCost,
                            UnitCost = inventoryTransaction.UnitCost,
                            Remark = string.IsNullOrWhiteSpace(detail.Remark) ? $"撤销到货单【{arrival.ArrivalCode}】入库" : $"撤销到货单【{arrival.ArrivalCode}】入库，原备注：{detail.Remark}",
                            ReversalOfTransactionID = inventoryTransaction.InventoryTransactionID,
                            SupplierID = inventory.SupplierID,
                            CreateUserID = _currentUserService.UserID
                        };
                        var addTransactionResult = await _inventoryRepository.AddInventoryTransactionAsync(cancelStockInTransaction, transaction);
                        if (!addTransactionResult)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】库存流水写入失败,无法撤销入库");
                        }
                    }
                    else
                    {
                        // 初始起点：本次准备撤销的原始入库流水
                        var startTransaction = inventoryTransaction;
                        bool startChanged;
                        if (inventoryTransaction.InventoryID != inventory.InventoryID || inventoryTransaction.CompanyID != companyID)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】原始入库流水库存归属不一致，无法撤销入库");
                        }
                        do
                        {
                            // 每轮开始，默认起点没有发生变化
                            startChanged = false;
                            // 查询当前重算起点之后的库存流水
                            subsequentTransactions = await _inventoryRepository.GetSubsequentInventoryTransactionsAsync(inventory.InventoryID, companyID, startTransaction.InventoryTransactionID, transaction);
                            foreach (var item in subsequentTransactions)
                            {
                                // 不是撤销入库流水，跳过
                                if (item.TransactionType != InventoryTransactionType.ArrivalStockInCancel)
                                {
                                    continue;
                                }
                                // 撤销流水必须关联原始入库流水
                                if (!item.ReversalOfTransactionID.HasValue)
                                {
                                    return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】撤销流水缺少原始入库关联ID，无法撤销入库");
                                }
                                // 校验撤销流水的库存和公司归属
                                if (item.InventoryID != inventory.InventoryID || item.CompanyID != companyID)
                                {
                                    return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】撤销流水库存归属不一致，无法撤销入库");
                                }
                                // 根据撤销流水的关联ID，查询被撤销的原始入库流水
                                var originalTransaction = await _inventoryRepository.GetInventoryTransactionByIDAsync(item.ReversalOfTransactionID.Value, companyID, transaction);
                                // 原始流水不存在
                                if (originalTransaction == null)
                                {
                                    return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】撤销流水关联的原始入库流水不存在，无法撤销入库");
                                }
                                // 校验原始流水的业务类型
                                if (originalTransaction.TransactionType != InventoryTransactionType.ArrivalStockIn)
                                {
                                    return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】撤销流水关联的原始入库流水类型不正确，无法撤销入库");
                                }
                                // 校验原始流水的库存和公司归属
                                if (originalTransaction.InventoryID != inventory.InventoryID || originalTransaction.CompanyID != companyID)
                                {
                                    return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】原始入库流水库存归属不一致，无法撤销入库");
                                }
                                // 原始入库流水必须早于对应的撤销流水
                                if (originalTransaction.InventoryTransactionID >= item.InventoryTransactionID)
                                {
                                    return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】原始入库流水与撤销流水顺序不正确，无法撤销入库");
                                }
                                // 找到更早的原始入库，将重算起点向前移动
                                if (originalTransaction.InventoryTransactionID < startTransaction.InventoryTransactionID)
                                {
                                    startTransaction = originalTransaction;
                                    startChanged = true;
                                    break;
                                }
                            }
                        } while (startChanged);
                        // 重算库存数量（基础单位）
                        var recalculatedQuantity = startTransaction.BeforeQuantity;
                        // 重算库存金额
                        var recalculatedAmount = startTransaction.BeforeInventoryAmount;
                        // 重算移动平均成本（每基础单位成本）
                        var recalculatedAverageCost = startTransaction.BeforeAverageCost;
                        var skippedTransactionIDs = new HashSet<int>();
                        // 排除本次准备撤销的原始入库流水
                        skippedTransactionIDs.Add(inventoryTransaction.InventoryTransactionID);
                        foreach (var item in subsequentTransactions)
                        {
                            // 不是撤销入库流水，直接跳过
                            if (item.TransactionType != InventoryTransactionType.ArrivalStockInCancel)
                            {
                                continue;
                            }
                            if (!item.ReversalOfTransactionID.HasValue)
                            {
                                return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】撤销流水缺少原始入库关联ID，无法撤销入库");
                            }
                            // 历史被撤销的原始入库
                            skippedTransactionIDs.Add(item.ReversalOfTransactionID.Value);
                            // 历史撤销流水本身
                            skippedTransactionIDs.Add(item.InventoryTransactionID);
                        }
                        foreach(var item in subsequentTransactions)
                        {
                            if (skippedTransactionIDs.Contains(item.InventoryTransactionID))
                            {
                                continue;
                            }
                            switch (item.TransactionType)
                            {
                                // 普通入库
                                case InventoryTransactionType.ArrivalStockIn:
                                case InventoryTransactionType.OtherStockIn:
                                    recalculatedQuantity += item.QuantityChange;
                                    recalculatedAmount += item.AmountChange;
                                    recalculatedAverageCost = recalculatedQuantity > 0 ? recalculatedAmount / recalculatedQuantity : 0;
                                    break;
                                // 普通出库
                                case InventoryTransactionType.SaleStockOut:
                                case InventoryTransactionType.OtherStockOut:
                                    var nextQuantity = recalculatedQuantity + item.QuantityChange;
                                    if (nextQuantity < 0)
                                    {
                                        return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】重算后库存不足，无法撤销入库");
                                    }
                                    recalculatedAmount += item.QuantityChange * recalculatedAverageCost;
                                    recalculatedQuantity = nextQuantity;
                                    if (recalculatedQuantity == 0)
                                    {
                                        recalculatedAmount = 0;
                                        recalculatedAverageCost = 0;
                                    }
                                    break;
                                case InventoryTransactionType.ArrivalStockInCancel:
                                    return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】存在未正确排除的撤销入库流水，无法撤销入库");

                                default:
                                    return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】存在不支持的库存流水类型，无法撤销入库");
                            }
                        }
                        // 原始入库流水必须增加库存数量
                        if (inventoryTransaction.QuantityChange <= 0)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】原始入库流水数量变化不合法，无法撤销入库");
                        }
                        // 理论撤销后的库存数量
                        var expectedQuantity = inventory.Quantity - inventoryTransaction.QuantityChange;
                        if (expectedQuantity < 0)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】撤销后理论库存数量不足，无法撤销入库");
                        }
                        if (recalculatedQuantity != expectedQuantity)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】重算后库存数量不一致，无法撤销入库");
                        }
                        if (recalculatedAmount < 0)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】重算后库存金额小于0，无法撤销入库");
                        }
                        if (recalculatedQuantity == 0 &&
                            (recalculatedAmount != 0 || recalculatedAverageCost != 0))
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】重算后零库存的金额或平均成本不为0，无法撤销入库");
                        }
                        // 保存本次撤销前的库存状态
                        var beforeQuantity = inventory.Quantity;
                        var beforeAmount = inventory.InventoryAmount;
                        var beforeAverageCost = inventory.AverageCost;
                        // 组装重算后的库存数据
                        var updateInventory = new Inventory
                        {
                            InventoryID = inventory.InventoryID,
                            Quantity = recalculatedQuantity,
                            InventoryAmount = recalculatedAmount,
                            AverageCost = recalculatedAverageCost,
                            UpdateUserID = _currentUserService.UserID
                        };
                        var updateInventoryResult = await _inventoryRepository.UpdateInventoryAsync(updateInventory, companyID, transaction);
                        if (!updateInventoryResult)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】重算库存更新失败，无法撤销入库");
                        }
                        // 组装本次撤销入库的库存流水
                        var cancelStockInTransaction = new InventoryTransaction
                        {
                            InventoryID = inventory.InventoryID,
                            CompanyID = companyID,
                            LocationID = arrival.LocationID,
                            GoodsID = detail.GoodsID,
                            SupplierID = inventory.SupplierID,
                            TransactionType = InventoryTransactionType.ArrivalStockInCancel,
                            SourceID = arrival.ArrivalID,
                            SourceDetailID = detail.ArrivalDetailID,
                            SourceCode = arrival.ArrivalCode,
                            // 撤销前后库存数量（基础单位）
                            BeforeQuantity = beforeQuantity,
                            QuantityChange = recalculatedQuantity - beforeQuantity,
                            AfterQuantity = recalculatedQuantity,
                            // 撤销前后库存金额
                            BeforeInventoryAmount = beforeAmount,
                            AmountChange = recalculatedAmount - beforeAmount,
                            AfterInventoryAmount = recalculatedAmount,
                            // 撤销前后移动平均成本
                            BeforeAverageCost = beforeAverageCost,
                            AfterAverageCost = recalculatedAverageCost,
                            // 原始入库流水的基础单位成本
                            UnitCost = inventoryTransaction.UnitCost,
                            // 指向本次被撤销的原始入库流水
                            ReversalOfTransactionID = inventoryTransaction.InventoryTransactionID,
                            Remark = $"撤销到货单【{arrival.ArrivalCode}】入库，重新计算库存成本",
                            CreateUserID = _currentUserService.UserID
                        };
                        var addTransactionResult = await _inventoryRepository.AddInventoryTransactionAsync(cancelStockInTransaction, transaction);
                        if (!addTransactionResult)
                        {
                            return ApiResponse<bool>.Error($"商品【{detail.GoodsTitle}】撤销入库流水写入失败，无法撤销入库");
                        }
                    }
                }
                await transaction.CommitAsync();
                return ApiResponse<bool>.Success(true, $"到货单【{arrival.ArrivalCode}】撤销入库成功");
            }
            catch
            {
                await transaction.RollbackAsync();
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
            var DbConnection = (DbConnection)_dbConnection;
            if (DbConnection.State != ConnectionState.Open)
            {
                await DbConnection.OpenAsync();
            }
            await using var transaction = await DbConnection.BeginTransactionAsync();
            try
            {
                var currentArrival = await _arrivalRepository.GetArrivalForUpdateAsync(request.ArrivalID, companyID, transaction);
                if (currentArrival == null)
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error("到货单数据已发生变化,请刷新后重试");
                }
                if (currentArrival.ArrivalStatus != ArrivalStatus.Pending)
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error($"到货单【{currentArrival.ArrivalCode}】当前状态不是未审核，无法修改到货单");
                }
                var supplier = await _arrivalRepository.GetSupplierByIDAsync(request.SupplierID, transaction);
                if (supplier == null)
                {
                    return ApiResponse<bool>.Error($"供应商【{request.SupplierID}】不存在，无法修改到货单");
                }
                if (!supplier.IsActive)
                {
                    return ApiResponse<bool>.Error($"供应商【{supplier.SupplierName}】已停用,无法修改到货单");
                }
                var location = await _arrivalRepository.GetLocationByIDAsync(request.LocationID, companyID, transaction);
                if (location == null)
                {
                    return ApiResponse<bool>.Error("站点不存在或不属于当前公司，无法修改到货单");
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
                List<int> goodsUnitIDs = request.ArrivalDetails.Select(x => x.GoodsUnitID).Distinct().ToList();
                var goodsUnits = await _arrivalRepository.GetGoodsUnitsByIDsAsync(goodsUnitIDs, transaction);
                foreach (var detail in request.ArrivalDetails)
                {
                    var goodsUnit = goodsUnits.FirstOrDefault(x => x.GoodsUnitID == detail.GoodsUnitID);
                    if (goodsUnit == null)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<bool>.Error($"商品单位关系【{detail.GoodsUnitID}】不存在，无法修改到货单");
                    }
                    if (!goodsUnit.GoodsIsActive)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<bool>.Error($"商品【{goodsUnit.GoodsCode}】已停用，无法修改到货单");
                    }
                    if (!goodsUnit.UnitIsActive)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<bool>.Error($"单位【{goodsUnit.UnitCode}】已停用，无法修改到货单");
                    }
                    if (!goodsUnit.GoodsUnitIsActive)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<bool>.Error($"商品单位关系【{goodsUnit.GoodsUnitID}】已停用，无法修改到货单");
                    }
                    if (goodsUnit.ConversionRate <= 0)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<bool>.Error($"商品单位关系【{goodsUnit.GoodsUnitID}】换算率必须大于0，无法修改到货单");
                    }
                    decimal calculatedAmount = Math.Round(detail.Quantity * detail.PurchaseUnitCost, 2, MidpointRounding.AwayFromZero);
                    if(calculatedAmount != detail.Amount)
                    {
                        decimal calculatedPrice = Math.Round(detail.Amount / detail.Quantity, 6, MidpointRounding.AwayFromZero);
                        if(calculatedPrice != detail.PurchaseUnitCost || Math.Abs(calculatedAmount - detail.Amount) > 0.01m)
                        {
                            await transaction.RollbackAsync();
                            return ApiResponse<bool>.Error($"商品【{goodsUnit.GoodsCode}】的金额计算不一致，请检查数量、采购单位单价和金额的关系，无法修改到货单");
                        }
                    }
                    var baseQuantity = detail.Quantity * goodsUnit.ConversionRate;
                    decimal unitCost = Math.Round(detail.Amount / baseQuantity, 6, MidpointRounding.AwayFromZero);
                    var arrivalDetail = new ArrivalDetail
                    {
                        ArrivalID = request.ArrivalID,
                        GoodsID = goodsUnit.GoodsID,
                        GoodsUnitID = detail.GoodsUnitID,
                        Quantity = detail.Quantity,
                        PurchaseUnitCost = detail.PurchaseUnitCost,
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
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error($"到货单【{currentArrival.ArrivalCode}】主表修改失败,无法修改到货单");
                }
                var deleteResult = await _arrivalRepository.DeleteArrivalDetailsAsync(request.ArrivalID, companyID, transaction);
                if (!deleteResult) 
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error($"到货单【{currentArrival.ArrivalCode}】旧明细删除失败,无法修改到货单");
                }
                var addResult = await _arrivalRepository.AddArrivalDetailsAsync(arrivalDetails, transaction);
                if (!addResult)
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.Error($"到货单【{currentArrival.ArrivalCode}】新明细写入失败,无法修改到货单");
                }
                await transaction.CommitAsync();
                return ApiResponse<bool>.Success(true, "到货单修改成功");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion
    }
}