using System.Data;
using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Enums;
using ERP.Server.Models.Request.Arrival;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;
using ERP.Server.Services.Interfaces.Arrival;

namespace ERP.Server.Services.Implementations
{
    public class ArrivalService : IArrivalService
    {
        private readonly IArrivalRepository _arrivalRepository;
        private readonly IDbConnection _dbConnection;
        private readonly ICurrentUserService _currentUserService;

        public ArrivalService(IArrivalRepository arrivalRepository, IDbConnection dbConnection , ICurrentUserService currentUserService)
        {
            _arrivalRepository = arrivalRepository;
            _dbConnection = dbConnection;
            _currentUserService = currentUserService;
        }

        #region 新增到货单AddArrivalAsync
        public async Task<ApiResponse<bool>> AddArrivalAsync(AddArrivalRequest request)
        {
            if (request.ArrivalDetails == null || request.ArrivalDetails.Count == 0)
            {
                return ApiResponse<bool>.Error("到货明细不能为空");
            }
            var arrivalDetails = new List<ArrivalDetail>();
            foreach (var detail in request.ArrivalDetails)
            {
                if (detail.Quantity <= 0)
                {
                    return ApiResponse<bool>.Error("到货数量必须大于0");
                }
                if (detail.Amount < 0)
                {
                    return ApiResponse<bool>.Error("到货金额不能小于0");
                }
                var goodsUnit = await _arrivalRepository.GetGoodsUnitByIDAsync(detail.GoodsUnitID);
                if (goodsUnit == null)
                {
                    return ApiResponse<bool>.Error("商品单位关系不存在");
                }
                if (!goodsUnit.IsActive)
                {
                    return ApiResponse<bool>.Error("商品单位关系已停用");
                }
                var arrivalDetail = new ArrivalDetail
                {
                    GoodsID = goodsUnit.GoodsID,
                    GoodsUnitID = detail.GoodsUnitID,
                    Quantity = detail.Quantity,
                    UnitCost = detail.Amount / detail.Quantity,
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
                    BaseQuantity = detail.Quantity * goodsUnit.ConversionRate
                };
                arrivalDetails.Add(arrivalDetail);
            }
            var arrival = new Arrival
            {
                SupplierID = request.SupplierID,
                CompanyID = _currentUserService.CompanyID,
                LocationID = request.LocationID,
                ArrivalStatus = ArrivalStatus.Pending,
                ArrivalTime = request.ArrivalTime,
                TotalAmount = arrivalDetails.Sum(x => x.Amount),
                Remark = request.Remark,
                CreateUserID = _currentUserService.UserID
            };
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using var transaction = _dbConnection.BeginTransaction();
            try {
                    var prefix = "DH" + DateTime.Now.ToString("yyyyMMdd");
                    var maxArrivalCode = await _arrivalRepository.GetMaxArrivalCodeAsync(prefix, transaction);
                    int sequence;
                    if (maxArrivalCode == null)
                    {
                        sequence = 1;
                    }
                    else
                    {
                        var sequenceText = maxArrivalCode.Substring(maxArrivalCode.Length - 4);
                        sequence = int.Parse(sequenceText) + 1;
                    }
                    var sequenceCode = sequence.ToString("D4");
                    var arrivalCode = prefix + sequenceCode;
                    arrival.ArrivalCode = arrivalCode;

                    var arrivalID = await _arrivalRepository.AddArrivalAsync(arrival, transaction);
                    foreach (var arrivalDetail in arrivalDetails)
                    {
                        arrivalDetail.ArrivalID = arrivalID;
                    }
                    var detailResult = await _arrivalRepository.AddArrivalDetailsAsync(arrivalDetails, transaction);
                    if (!detailResult)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error("新增到货明细失败");
                    }
                    transaction.Commit();
                    return ApiResponse<bool>.Success(true, "新增到货单成功");
                }catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        #endregion
    }
}