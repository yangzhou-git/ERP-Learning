using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Repositories.Interfaces
{
    public interface IGoodsRepository
    {
        #region 新增商品资料AddGoodsAsync
        Task<bool> AddGoodsAsync(Goods Goods);
        #endregion
        #region 查询商品列表GetGoodsListAsync
        Task<List<GoodsListResponse>> GetGoodsListAsync(GetGoodsListRequest Request);
        #endregion
        #region 查询商品总数
        Task<int> GetGoodsCountAsync(GetGoodsListRequest Request);
        #endregion
        #region 修改商品状态UpdateGoodsStatusAsync
        Task<bool> UpdateGoodsStatusAsync(int GoodsID, bool IsActive);
        #endregion
        #region 修改商品资料UpdateGoodsAsync
        Task<bool> UpdateGoodsAsync(Goods goods);
        #endregion 
    }
}
