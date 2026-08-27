using ERP.Server.Models.Entity;

namespace ERP.Server.Repositories.Interfaces
{
    public interface IGoodsUnitRepository
    {
        #region 根据商品ID查询基础单位GetBaseUnitByGoodsIDAsync
        Task<GoodsUnit?> GetBaseUnitByGoodsIDAsync(int goodsID);
        #endregion
        #region 根据商品ID和单位ID查询商品单位关系GetGoodsUnitByGoodsAndUnitAsync
        Task<GoodsUnit?> GetGoodsUnitByGoodsAndUnitAsync(int goodsID, int unitID);
        #endregion
        #region 新增商品单位关系AddGoodsUnitAsync
        Task<bool> AddGoodsUnitAsync(GoodsUnit goodsUnit);
        #endregion
    }
}
