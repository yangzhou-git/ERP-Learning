using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Request.GoodsUnit;
using ERP.Server.Models.Response;
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

        #region 查询商品单位列表GetGoodsUnitListAsync
        public async Task<ApiResponse<PageResponse<GoodsUnitListResponse>>> GetGoodsUnitListAsync(GetGoodsUnitListRequest request)
        {
            var goodsUnitList = await _goodsUnitRepository.GetGoodsUnitListAsync(request);
            var count = await _goodsUnitRepository.GetGoodsUnitCountAsync(request);
            var goodsUnitResponseList = new List<GoodsUnitListResponse>();
            foreach (var goodsUnit in goodsUnitList)
            {
                var response = new GoodsUnitListResponse
                {
                    GoodsUnitID = goodsUnit.GoodsUnitID,
                    GoodsID = goodsUnit.GoodsID,
                    UnitID = goodsUnit.UnitID,
                    UnitName = goodsUnit.UnitName,
                    UnitCode = goodsUnit.UnitCode,
                    ConversionRate = goodsUnit.ConversionRate,
                    IsBaseUnit = goodsUnit.IsBaseUnit,
                    IsActive = goodsUnit.IsActive,
                    Sort = goodsUnit.Sort,
                    CreateUserID = goodsUnit.CreateUserID,
                    CreateUserName = goodsUnit.CreateUserName,
                    CreateTime = goodsUnit.CreateTime,
                    UpdateUserID = goodsUnit.UpdateUserID,
                    UpdateUserName = goodsUnit.UpdateUserName,
                    UpdateTime = goodsUnit.UpdateTime
                };
                goodsUnitResponseList.Add(response);
            }
            var pageResponse = new PageResponse<GoodsUnitListResponse>
            {
                TotalCount = count,
                List = goodsUnitResponseList
            };
            return ApiResponse<PageResponse<GoodsUnitListResponse>>.Success(pageResponse, "查询成功");
        }
        #endregion

        #region 修改商品单位UpdateGoodsUnitAsync
        public async Task<ApiResponse<bool>> UpdateGoodsUnitAsync(UpdateGoodsUnitRequest request)
        {
            var goodsUnit = await _goodsUnitRepository.GetGoodsUnitByIdAsync(request.GoodsUnitID);
            if (goodsUnit == null) 
            {
                return ApiResponse<bool>.Error("商品单位关系不存在");
            }
            var unit = await _unitRepository.GetUnitByIdAsync(request.UnitID);
            if (unit == null)
            {
                return ApiResponse<bool>.Error("所选单位不存在");
            }
            var existGoodsUnit = await _goodsUnitRepository.GetGoodsUnitByGoodsAndUnitAsync(goodsUnit.GoodsID, request.UnitID);
            if (existGoodsUnit != null && existGoodsUnit.GoodsUnitID != request.GoodsUnitID)
            {
                return ApiResponse<bool>.Error($"该商品已绑定单位【{unit.UnitName}】，请勿重复选择");
            }
            if (request.ConversionRate <= 0)
            {
                return ApiResponse<bool>.Error("单位换算率必须大于0");
            }
            if (goodsUnit.IsBaseUnit && request.ConversionRate != 1)
            {
                return ApiResponse<bool>.Error("基础单位换算率必须为1");
            }
            goodsUnit.UnitID = request.UnitID;
            goodsUnit.ConversionRate = request.ConversionRate;
            goodsUnit.Sort = request.Sort;
            goodsUnit.UpdateUserID = _currentUserService.UserID;
            var result = await _goodsUnitRepository.UpdateGoodsUnitAsync(goodsUnit);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改商品单位关系成功");
            }
            return ApiResponse<bool>.Error("修改商品单位关系失败");
        }
        #endregion

        #region 修改商品单位关系状态UpdateGoodsUnitStatusAsync
        public async Task<ApiResponse<bool>> UpdateGoodsUnitStatusAsync(UpdateGoodsUnitStatusRequest request)
        {
            var goodsUnit = await _goodsUnitRepository.GetGoodsUnitByIdAsync(request.GoodsUnitID);
            if (goodsUnit == null)
            {
                return ApiResponse<bool>.Error("商品单位关系不存在");
            }
            goodsUnit.IsActive = request.IsActive;
            goodsUnit.UpdateUserID = _currentUserService.UserID;
            var result = await _goodsUnitRepository.UpdateGoodsUnitStatusAsync(goodsUnit);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改商品单位状态成功");
            }
            return ApiResponse<bool>.Error("修改商品单位状态失败");
        }
        #endregion
    }
}
