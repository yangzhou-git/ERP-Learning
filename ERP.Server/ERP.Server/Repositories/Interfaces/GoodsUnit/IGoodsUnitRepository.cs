using ERP.Server.Models.DTO.GoodsUnit;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request.GoodsUnit;

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
        #region 根据商品单位关系ID查询GetGoodsUnitByIdAsync
        Task<GoodsUnit?> GetGoodsUnitByIdAsync(int goodsUnitID);
        #endregion
        #region 查询商品单位列表GetGoodsUnitListAsync
        Task<List<GoodsUnitListDto>> GetGoodsUnitListAsync(GetGoodsUnitListRequest request);
        #endregion
        #region 查询商品单位数量GetGoodsUnitCountAsync
        Task<int> GetGoodsUnitCountAsync(GetGoodsUnitListRequest request);
        #endregion
        #region 新增商品单位关系AddGoodsUnitAsync
        Task<bool> AddGoodsUnitAsync(GoodsUnit goodsUnit);
        #endregion
        #region 修改商品单位UpdateGoodsUnitAsync
        Task<bool> UpdateGoodsUnitAsync(GoodsUnit goodsUnit);
        #endregion
        #region 修改商品单位关系状态UpdateGoodsUnitStatusAsync
        Task<bool> UpdateGoodsUnitStatusAsync(GoodsUnit goodsUnit);
        #endregion
    }
}
