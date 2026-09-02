using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Repositories.Interfaces
{
    public interface IGoodsRepository
    {
        #region 新增商品资料AddGoodsAsync
        Task<bool> AddGoodsAsync(Goods goods);
        #endregion
        #region 查询商品列表GetGoodsListAsync
        Task<List<GoodsListResponse>> GetGoodsListAsync(GetGoodsListRequest request);
        #endregion
        #region 查询商品总数GetGoodsCountAsync
        Task<int> GetGoodsCountAsync(GetGoodsListRequest request);
        #endregion
        #region 根据商品编码查询商品GetGoodsByCodeAsync
        Task<Goods?> GetGoodsByCodeAsync(string goodsCode);
        #endregion
        #region 根据商品条码查询商品GetGoodsByBarCodeAsync
        Task<Goods?> GetGoodsByBarCodeAsync(string barCode);
        #endregion
        #region 根据商品ID查询商品GetGoodsByIdAsync
        Task<Goods?> GetGoodsByIdAsync(int goodsID);
        #endregion
        #region 修改商品状态UpdateGoodsStatusAsync
        Task<bool> UpdateGoodsStatusAsync(Goods goods);
        #endregion
        #region 修改商品资料UpdateGoodsAsync
        Task<bool> UpdateGoodsAsync(Goods goods);
        #endregion
    }
}