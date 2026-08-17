using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;

namespace ERP.Server.Repositories.Interfaces
{
    public interface IGoodsCategoryRepository
    {
        #region 获取商品分类列表GetGoodsCategoryListAsync
        Task<List<GoodsCategory>> GetGoodsCategoryListAsync(GetGoodsCategoryListRequest request);
        #endregion
        #region 获取商品分类数量GetGoodsCategoryCountAsync
        Task<int> GetGoodsCategoryCountAsync(GetGoodsCategoryListRequest request);
        #endregion
        #region 查询商品分类树GetGoodsCategoryTreeAsync
        Task<List<GoodsCategory>> GetGoodsCategoryTreeAsync();
        #endregion
        #region 查询全部商品分类关系GetAllGoodsCategoryAsync
        Task<List<GoodsCategory>> GetAllGoodsCategoryAsync();
        #endregion
        #region 查询启用子分类GetActiveChildCategoryListAsync
        Task<List<GoodsCategory>> GetActiveChildCategoryListAsync(int parentID);
        #endregion
        #region 新增商品分类AddGoodsCategoryAsync
        Task<bool> AddGoodsCategoryAsync(GoodsCategory category);
        #endregion
        #region 根据父级ID和分类名称查询商品分类GetGoodsCategoryByParentAndNameAsync
        Task<GoodsCategory?> GetGoodsCategoryByParentAndNameAsync(int? parentID, string categoryName);
        #endregion
        #region 根据分类编码查询商品分类GetGoodsCategoryByCodeAsync
        Task<GoodsCategory?> GetGoodsCategoryByCodeAsync(string categoryCode);
        #endregion
        #region 根据分类ID查询商品分类GetGoodsCategoryByIdAsync
        Task<GoodsCategory?> GetGoodsCategoryByIdAsync(int categoryID);
        #endregion
        #region 修改商品分类UpdateGoodsCategoryAsync
        Task<bool> UpdateGoodsCategoryAsync(GoodsCategory category);
        #endregion
        #region 修改商品分类状态UpdateGoodsCategoryStatusAsync
        Task<bool> UpdateGoodsCategoryStatusAsync(GoodsCategory category);
        #endregion
    }
}