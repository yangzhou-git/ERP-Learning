using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class GoodsService : IGoodsService
    {
        private readonly IGoodsRepository _goodsRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IGoodsCategoryRepository _goodsCategoryRepository;

        public GoodsService(IGoodsRepository goodsRepository, ICurrentUserService currentUserService, IGoodsCategoryRepository goodsCategoryRepository)
        {
            _goodsRepository = goodsRepository;
            _currentUserService = currentUserService;
            _goodsCategoryRepository = goodsCategoryRepository;
        }

        #region 新增商品AddGoodsAsync
        public async Task<ApiResponse<bool>> AddGoodsAsync(AddGoodsRequest request)
        {
            var existingGoodsCode = await _goodsRepository.GetGoodsByCodeAsync(request.GoodsCode);
            if (existingGoodsCode != null)
            {
                return ApiResponse<bool>.Error($"商品编码【{request.GoodsCode}】已存在");
            }
            var existingBarCode = await _goodsRepository.GetGoodsByBarCodeAsync(request.BarCode);
            if (existingBarCode != null)
            {
                return ApiResponse<bool>.Error($"商品条码【{request.BarCode}】已存在");
            }
            var category = await _goodsCategoryRepository.GetGoodsCategoryByIdAsync(request.CategoryID);
            if (category == null)
            {
                return ApiResponse<bool>.Error($"商品分类ID【{request.CategoryID}】不存在，请重新选择商品分类");
            }
            if (!category.IsActive)
            {
                return ApiResponse<bool>.Error($"商品分类【{category.CategoryName}】已停用，请选择其他商品分类");
            }
            var goods = new Goods
            {
                BarCode = request.BarCode,
                GoodsCode = request.GoodsCode,
                CategoryID = request.CategoryID,
                GoodsTitle = request.GoodsTitle,
                GoodsSubTitle = request.GoodsSubTitle,
                GoodsPrice = request.GoodsPrice,
                CreateUserID = _currentUserService.UserID,
                IsActive = true
            };
            var result = await _goodsRepository.AddGoodsAsync(goods);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "新增商品成功");
            }
            return ApiResponse<bool>.Error("新增商品失败");
        }
        #endregion
        #region 获取商品列表GetGoodsListAsync
        public async Task<ApiResponse<PageResponse<GoodsListResponse>>> GetGoodsListAsync(GetGoodsListRequest request)
        {
            var goodsList = await _goodsRepository.GetGoodsListAsync(request);
            var goodsCount = await _goodsRepository.GetGoodsCountAsync(request);
            var pageData = new PageResponse<GoodsListResponse>
            {
                TotalCount = goodsCount,
                List = goodsList
            };
            return ApiResponse<PageResponse<GoodsListResponse>>.Success(pageData,"查询商品列表成功");
        }
        #endregion
        #region 修改商品状态UpdateGoodsStatusAsync
        public async Task<ApiResponse<bool>> UpdateGoodsStatusAsync(UpdateGoodsStatusRequest request)
        {
            var goods = await _goodsRepository.GetGoodsByIdAsync(request.GoodsID);
            if (goods == null)
            {
                return ApiResponse<bool>.Error($"商品ID【{request.GoodsID}】不存在");
            }
            goods.IsActive = request.IsActive;
            goods.UpdateUserID = _currentUserService.UserID;
            var result = await _goodsRepository.UpdateGoodsStatusAsync(goods);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改商品状态成功");
            }
            return ApiResponse<bool>.Error("修改商品状态失败");
        }
        #endregion
        #region 修改商品UpdateGoodsAsync
        public async Task<ApiResponse<bool>> UpdateGoodsAsync(UpdateGoodsRequest request)
        {
            var currentGoods = await _goodsRepository.GetGoodsByIdAsync(request.GoodsID);
            if (currentGoods == null)
            {
                return ApiResponse<bool>.Error($"商品ID【{request.GoodsID}】不存在");
            }
            var existingGoodsCode = await _goodsRepository.GetGoodsByCodeAsync(request.GoodsCode);
            if (existingGoodsCode != null && existingGoodsCode.GoodsID != request.GoodsID)
            {
                return ApiResponse<bool>.Error($"商品编码【{request.GoodsCode}】已存在");
            }
            var existingBarCode = await _goodsRepository.GetGoodsByBarCodeAsync(request.BarCode);
            if (existingBarCode != null && existingBarCode.GoodsID != request.GoodsID)
            {
                return ApiResponse<bool>.Error($"商品条码【{request.BarCode}】已存在");
            }
            var category = await _goodsCategoryRepository.GetGoodsCategoryByIdAsync(request.CategoryID);
            if (category == null)
            {
                return ApiResponse<bool>.Error($"商品分类ID【{request.CategoryID}】不存在，请重新选择商品分类");
            }
            if (!category.IsActive)
            {
                return ApiResponse<bool>.Error($"商品分类【{category.CategoryName}】已停用，请选择其他商品分类");
            }
            var goods = new Goods
            {
                GoodsID = request.GoodsID,
                BarCode = request.BarCode,
                GoodsCode = request.GoodsCode,
                CategoryID = request.CategoryID,
                GoodsTitle = request.GoodsTitle,
                GoodsSubTitle = request.GoodsSubTitle,
                GoodsPrice = request.GoodsPrice,
                UpdateUserID = _currentUserService.UserID
            };
            var result = await _goodsRepository.UpdateGoodsAsync(goods);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改商品成功");
            }
            return ApiResponse<bool>.Error("修改商品失败");
        }
        #endregion
    }
}