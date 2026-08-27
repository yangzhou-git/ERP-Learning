using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class GoodsUnitService : IGoodsUnitService
    {
        private readonly IGoodsUnitRepository _goodsUnitRepository;
        private readonly IGoodsRepository _goodsRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ICurrentUserService _currentUserService;

        public GoodsUnitService(IGoodsUnitRepository goodsUnitRepository, IGoodsRepository goodsRepository, IUnitRepository unitRepository, ICurrentUserService currentUserService)
        {
            _goodsUnitRepository = goodsUnitRepository;
            _goodsRepository = goodsRepository;
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
        }

        #region 新增商品单位关系AddGoodsUnitAsync
        public async Task<ApiResponse<bool>> AddGoodsUnitAsync(AddGoodsUnitRequest request)
        {
            var goods = await _goodsRepository.GetGoodsByIdAsync(request.GoodsID);
            if (goods == null)
            {
                return ApiResponse<bool>.Error("商品不存在");
            }
            var unit = await _unitRepository.GetUnitByIdAsync(request.UnitID);
            if (unit == null)
            {
                return ApiResponse<bool>.Error("单位不存在");
            }
            var existingGoodsUnit = await _goodsUnitRepository.GetGoodsUnitByGoodsAndUnitAsync(request.GoodsID, request.UnitID);
            if (existingGoodsUnit != null)
            {
                return ApiResponse<bool>.Error($"商品【{goods.GoodsTitle}】已经配置过单位【{unit.UnitName}】");
            }
            if (request.ConversionRate <= 0)
            {
                return ApiResponse<bool>.Error("换算率必须大于0");
            }
            if(request.IsBaseUnit)
            {
                if (request.ConversionRate != 1)
                {
                    return ApiResponse<bool>.Error("基础单位换算率必须为1");
                }
                var baseUnit = await _goodsUnitRepository.GetBaseUnitByGoodsIDAsync(goods.GoodsID);
                if (baseUnit != null)
                {
                    return ApiResponse<bool>.Error($"商品【{goods.GoodsTitle}】已经存在基础单位");
                }
            }
            var goodsUnit = new GoodsUnit()
            {
                GoodsID = request.GoodsID,
                UnitID = request.UnitID,
                ConversionRate = request.ConversionRate,
                IsBaseUnit = request.IsBaseUnit,
                IsActive = true,
                Sort = 0,
                CreateUserID = _currentUserService.UserID
            };
           var result = await _goodsUnitRepository.AddGoodsUnitAsync(goodsUnit);
            if (result)
            {
                return ApiResponse<bool>.Success(true,"新增商品单位关系成功");
            }
            return ApiResponse<bool>.Error("新增商品单位关系失败");
        }
        #endregion
    }
}
