using System.Data;
using Dapper;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;

namespace ERP.Server.Repositories.Implementations
{
    public class GoodsRepository : IGoodsRepository
    {
        private readonly IDbConnection _dbConnection;

        public GoodsRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region 新增商品资料AddGoodsAsync
        public async Task<bool> AddGoodsAsync(Goods goods)
        {
            var sql = @"insert into Goods
                        (BarCode,GoodsCode,CategoryID,GoodsTitle,GoodsSubTitle,GoodsPrice,IsActive,CreateUserID,CreateTime)
                        values
                        (@BarCode,@GoodsCode,@CategoryID,@GoodsTitle,@GoodsSubTitle,@GoodsPrice,@IsActive,@CreateUserID,getdate())";

            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                BarCode = goods.BarCode,
                GoodsCode = goods.GoodsCode,
                CategoryID = goods.CategoryID,
                GoodsTitle = goods.GoodsTitle,
                GoodsSubTitle = goods.GoodsSubTitle,
                GoodsPrice = goods.GoodsPrice,
                CreateUserID = goods.CreateUserID,
                IsActive = goods.IsActive
            });

            return result > 0;
        }
        #endregion
        #region 查询商品列表GetGoodsListAsync
        public async Task<List<GoodsListResponse>> GetGoodsListAsync(GetGoodsListRequest request)
        {
            var skip = (request.Page - 1) * request.PageSize;
            var sql = @"select g.GoodsID,
                       g.BarCode,
                       g.GoodsCode,
                       g.GoodsTitle,
                       g.GoodsSubTitle,
                       g.CategoryID,
                       c.CategoryName,
                       g.GoodsPrice,
                       g.IsActive,
                       g.CreateUserID,
                       cu.UserName as CreateUserName,
                       g.CreateTime,
                       g.UpdateUserID,
                       uu.UserName as UpdateUserName,
                       g.UpdateTime
                from Goods g
                join GoodsCategory c on g.CategoryID = c.CategoryID
                left join Users cu on g.CreateUserID = cu.UserID
                left join Users uu on g.UpdateUserID = uu.UserID
                where (@GoodsTitle is null
                       or @GoodsTitle = ''
                       or g.GoodsTitle like '%' + @GoodsTitle + '%')
                order by g.GoodsID
                offset @Skip rows
                fetch next @PageSize rows only";
            var result = await _dbConnection.QueryAsync<GoodsListResponse>(sql, new
            {
                GoodsTitle = request.GoodsTitle,
                Skip = skip,
                PageSize = request.PageSize
            });
            return result.ToList();
        }
        #endregion
        #region 查询商品总数GetGoodsCountAsync
        public async Task<int> GetGoodsCountAsync(GetGoodsListRequest request)
        {
            var sql = @"select count(1)
                        from Goods
                        where (@GoodsTitle is null
                               or @GoodsTitle = ''
                               or GoodsTitle like '%' + @GoodsTitle + '%')";

            var goodsCount = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                GoodsTitle = request.GoodsTitle
            });

            return goodsCount;
        }
        #endregion
        #region 根据商品编码查询商品GetGoodsByCodeAsync
        public async Task<Goods?> GetGoodsByCodeAsync(string goodsCode)
        {
            var sql = @"select GoodsID, GoodsCode from Goods where GoodsCode = @GoodsCode";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Goods>(sql, new
            {
                GoodsCode = goodsCode
            });
            return result;
        }
        #endregion
        #region 根据商品条码查询商品GetGoodsByBarCodeAsync
        public async Task<Goods?> GetGoodsByBarCodeAsync(string barCode)
        {
            var sql = @"select GoodsID, BarCode from Goods where BarCode = @BarCode";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Goods>(sql, new
            {
                BarCode = barCode
            });
            return result;
        }
        #endregion

        #region 根据商品ID查询商品GetGoodsByIdAsync
        public async Task<Goods?> GetGoodsByIdAsync(int goodsID)
        {
            var sql = @"select GoodsID, BarCode, GoodsCode, CategoryID, GoodsTitle, GoodsSubTitle, GoodsPrice, IsActive, CreateUserID, CreateTime, UpdateUserID, UpdateTime from Goods
                        where GoodsID = @GoodsID";
            return await _dbConnection.QueryFirstOrDefaultAsync<Goods>(sql, new
            {
                GoodsID = goodsID
            });
        }
        #endregion

        #region 修改商品状态UpdateGoodsStatusAsync
        public async Task<bool> UpdateGoodsStatusAsync(Goods goods)
        {
            var sql = @"update Goods
                set IsActive = @IsActive,
                    UpdateUserID = @UpdateUserID,
                    UpdateTime = getdate()
                where GoodsID = @GoodsID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                GoodsID = goods.GoodsID,
                IsActive = goods.IsActive,
                UpdateUserID = goods.UpdateUserID
            });
            return result > 0;
        }
        #endregion
        #region 修改商品资料UpdateGoodsAsync
        public async Task<bool> UpdateGoodsAsync(Goods goods)
        {
            var sql = @"update Goods
                        set BarCode = @BarCode,
                            GoodsCode = @GoodsCode,
                            CategoryID = @CategoryID,
                            GoodsTitle = @GoodsTitle,
                            GoodsSubTitle = @GoodsSubTitle,
                            GoodsPrice = @GoodsPrice,
                            UpdateUserID = @UpdateUserID,
                            UpdateTime = getdate()
                        where GoodsID = @GoodsID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                GoodsID = goods.GoodsID,
                BarCode = goods.BarCode,
                GoodsCode = goods.GoodsCode,
                CategoryID = goods.CategoryID,
                GoodsTitle = goods.GoodsTitle,
                GoodsSubTitle = goods.GoodsSubTitle,
                GoodsPrice = goods.GoodsPrice,
                UpdateUserID = goods.UpdateUserID
            });
            return result > 0;
        }
        #endregion
    }
}