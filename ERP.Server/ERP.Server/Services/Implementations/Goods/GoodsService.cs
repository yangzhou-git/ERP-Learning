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

        public GoodsService(IGoodsRepository goodsRepository)
        {
            _goodsRepository = goodsRepository;
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
            var goods = new Goods
            {
                BarCode = request.BarCode,
                GoodsCode = request.GoodsCode,
                CategoryID = request.CategoryID,
                GoodsTitle = request.GoodsTitle,
                GoodsSubTitle = request.GoodsSubTitle,
                GoodsPrice = request.GoodsPrice,
                Unit = request.Unit,
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
            var result = await _goodsRepository.UpdateGoodsStatusAsync(request.GoodsID,request.IsActive);
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
            var goods = new Goods
            {
                GoodsID = request.GoodsID,
                BarCode = request.BarCode,
                GoodsCode = request.GoodsCode,
                CategoryID = request.CategoryID,
                GoodsTitle = request.GoodsTitle,
                GoodsSubTitle = request.GoodsSubTitle,
                GoodsPrice = request.GoodsPrice,
                Unit = request.Unit
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