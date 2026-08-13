using System.Data;
using Dapper;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Repositories.Interfaces;

namespace ERP.Server.Repositories.Implementations
{
    public class GoodsCategoryRepository : IGoodsCategoryRepository
    {
        private readonly IDbConnection _dbConnection;

        public GoodsCategoryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region 获取商品分类列表GetGoodsCategoryListAsync
        public async Task<List<GoodsCategory>> GetGoodsCategoryListAsync(GetGoodsCategoryListRequest request)
        {
            var skip = (request.Page - 1) * request.PageSize;
            var sql = @"select CategoryID, ParentID, CategoryName, CategoryCode, Sort, IsActive, CreateTime, UpdateTime from GoodsCategory where 
                                                                    (@CategoryName is null or @CategoryName = ''or CategoryName like '%' + @CategoryName + '%')
                                                                    and
                                                                    (@IsActive is null or IsActive = @IsActive)
                                                                    order by Sort
                                                                    offset @skip rows
                                                                    fetch next @PageSize rows only";
            var result = await _dbConnection.QueryAsync<GoodsCategory>(sql, new 
            {
                CategoryName = request.CategoryName, 
                IsActive = request.IsActive, 
                skip = skip, 
                PageSize = request.PageSize 
            });
            return result.ToList();
        }
        #endregion
        #region 获取商品分类数量GetGoodsCategoryCountAsync
        public async Task<int> GetGoodsCategoryCountAsync(GetGoodsCategoryListRequest request)
        {
            var sql = @"select count(1) from GoodsCategory where (@CategoryName is null or @CategoryName = ''or CategoryName like '%' + @CategoryName + '%')
                                                                  and
                                                                 (@IsActive is null or IsActive = @IsActive)";
            var result = await _dbConnection.ExecuteScalarAsync<int>(sql, new 
            { 
                CategoryName = request.CategoryName, 
                IsActive = request.IsActive
            });
            return result;
        }
        #endregion
        #region 查询商品分类树GetGoodsCategoryTreeAsync
        public async Task<List<GoodsCategory>> GetGoodsCategoryTreeAsync()
        {
            var sql = @"select CategoryID, ParentID, CategoryName, CategoryCode, Sort, IsActive, CreateTime, UpdateTime from GoodsCategory where IsActive = 1 order by Sort, CategoryID";
            var result = await _dbConnection.QueryAsync<GoodsCategory>(sql);
            return result.ToList();
        }
        #endregion
        #region 新增商品分类AddGoodsCategoryAsync
        public async Task<bool> AddGoodsCategoryAsync(GoodsCategory category)
        {
            var sql = @"insert into GoodsCategory (ParentID, CategoryName, CategoryCode, Sort, IsActive, CreateTime) values (@ParentID, @CategoryName, @CategoryCode, @Sort, @IsActive, getdate())";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                ParentID = category.ParentID,
                CategoryName = category.CategoryName,
                CategoryCode = category.CategoryCode,
                Sort = category.Sort,
                IsActive = category.IsActive
            });
            return result > 0;
        }
        #endregion
        #region 根据父级ID和分类名称查询商品分类GetGoodsCategoryByParentAndNameAsync
        public async Task<GoodsCategory?> GetGoodsCategoryByParentAndNameAsync(int? parentID, string categoryName)
        {
            var sql = @"select CategoryID, ParentID, CategoryName, CategoryCode, Sort, IsActive, CreateTime, UpdateTime 
                from GoodsCategory 
                where (ParentID = @ParentID or (ParentID IS NULL AND @ParentID IS NULL)) 
                and CategoryName = @CategoryName";

            var goodsCategory = await _dbConnection.QueryFirstOrDefaultAsync<GoodsCategory>(sql, new
            {
                ParentID = parentID,
                CategoryName = categoryName
            });

            return goodsCategory;
        }
        #endregion
        #region 根据分类编码查询商品分类GetGoodsCategoryByCodeAsync
        public async Task<GoodsCategory?> GetGoodsCategoryByCodeAsync(string categoryCode)
        {
            var sql = @"select top 1 * from GoodsCategory where CategoryCode = @CategoryCode";

            return await _dbConnection.QueryFirstOrDefaultAsync<GoodsCategory>(sql,new
                {
                    CategoryCode = categoryCode
                }
            );
        }
        #endregion
        #region 修改商品分类UpdateGoodsCategoryAsync
        public async Task<bool> UpdateGoodsCategoryAsync(GoodsCategory category)
        {
            var sql = @"update GoodsCategory set ParentID = @ParentID, CategoryName = @CategoryName, CategoryCode = @CategoryCode, Sort = @Sort, UpdateTime = getdate() where CategoryID = @CategoryID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                CategoryID = category.CategoryID,
                ParentID = category.ParentID,
                CategoryName = category.CategoryName,
                CategoryCode = category.CategoryCode,
                Sort = category.Sort
            }); 
            return result > 0;
        }
        #endregion
        #region 修改商品分类状态UpdateGoodsCategoryStatusAsync
        public async Task<bool> UpdateGoodsCategoryStatusAsync(GoodsCategory category)
        {
            var sql = @"update GoodsCategory set IsActive = @IsActive, UpdateTime = getdate() where CategoryID = @CategoryID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                CategoryID = category.CategoryID,
                IsActive = category.IsActive
            });
            return result > 0;
        }
        #endregion
    }
}