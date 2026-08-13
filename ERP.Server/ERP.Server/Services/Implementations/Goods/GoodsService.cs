using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Models.Entity;
using ERP.Server.Services.Interfaces;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Implementations
{
    public class GoodsService : IGoodsService
    {
        private readonly IGoodsRepository _GoodsRepository;

        public GoodsService(IGoodsRepository GoodsRepository)
        {
            _GoodsRepository = GoodsRepository;
        }
        #region 新增商品AddGoodsAsync
        public async Task<ApiResponse<bool>> AddGoodsAsync(AddGoodsRequest Request)
        {
            var Goods = new Goods
            {
                BarCode = Request.BarCode,
                GoodsCode = Request.GoodsCode,
                CategoryID = Request.CategoryID,
                GoodsTitle = Request.GoodsTitle,
                GoodsSubTitle = Request.GoodsSubTitle,
                GoodsPrice = Request.GoodsPrice,
                IsActive = true,
                Unit = Request.Unit,
            };
            var Result = await _GoodsRepository.AddGoodsAsync(Goods);
            if (Result)
            {
                return ApiResponse<bool>.Success(true, "新增成功");
            }
            return ApiResponse<bool>.Error("新增失败");
        }
        #endregion
        #region 获取商品列表GetGoodsListAsync
        public async Task<ApiResponse<PageResponse<GoodsListResponse>>> GetGoodsListAsync(GetGoodsListRequest Request)
        {
            var GoodsList = await _GoodsRepository.GetGoodsListAsync(Request);
            var GoodsCount = await _GoodsRepository.GetGoodsCountAsync(Request);
            var Result = new PageResponse<GoodsListResponse>()
            {
                TotalCount = GoodsCount,
                List = GoodsList
            };
            return ApiResponse<PageResponse<GoodsListResponse>>.Success(Result, "查询商品列表成功");
        }
        #endregion
        #region 修改商品状态UpdateGoodsAsync
        public async Task<ApiResponse<bool>> UpdateGoodsStatusAsync(UpdateGoodsStatusRequest Request)
        {
            var GoodsStatus = await _GoodsRepository.UpdateGoodsStatusAsync(Request.GoodsID,Request.IsActive);
            if(GoodsStatus)
            {
                return ApiResponse<bool>.Success(true, "修改成功");
            }
            return ApiResponse<bool>.Error("修改失败");
        }
        #endregion
        #region 修改商品UpdateGoodsAsync
        public async Task<ApiResponse<bool>> UpdateGoodsAsync(UpdateGoodsRequest request)
        {
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
            var Result = await _GoodsRepository.UpdateGoodsAsync(goods);
            if (Result)
            {
                return ApiResponse<bool>.Success(true, "修改成功");
            }
            return ApiResponse<bool>.Error("修改失败");
        }
        #endregion
    }
}
