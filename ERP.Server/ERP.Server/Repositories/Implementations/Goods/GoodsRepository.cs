using System.Data;
using Dapper;
using ERP.Server.Common;
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
        public async Task<bool> AddGoodsAsync(Goods Goods)
        {
            var Sql = @"insert into Goods (BarCode,GoodsCode,CategoryID,GoodsTitle,GoodsSubTitle,GoodsPrice,Unit,IsActive,CreateTime) values (@BarCode,@GoodsCode,@CategoryID,@GoodsTitle,@GoodsSubTitle,@GoodsPrice,@Unit,@IsActive,getdate())";
            var Rusult = await _dbConnection.ExecuteAsync(Sql, new
            {
                BarCode = Goods.BarCode,
                GoodsCode = Goods.GoodsCode,
                CategoryID = Goods.CategoryID,
                GoodsTitle = Goods.GoodsTitle,
                GoodsSubTitle = Goods.GoodsSubTitle,
                GoodsPrice = Goods.GoodsPrice,
                Unit = Goods.Unit,
                IsActive = Goods.IsActive
            });
            return Rusult > 0;
        }
        #endregion
        #region 查询商品列表GetGoodsListAsync
        public async Task<List<GoodsListResponse>> GetGoodsListAsync(GetGoodsListRequest Request)
        {
            int Skip = (Request.Page - 1) * Request.PageSize;
            var Sql = @"select g.GoodsID, g.BarCode, g.GoodsTitle, c.CategoryName, g.GoodsPrice, g.Unit, g.IsActive
                from Goods g
                join GoodsCategory c on g.CategoryID = c.CategoryID
                where (
                    @GoodsTitle is null
                    or @GoodsTitle = ''
                    or g.GoodsTitle like '%' + @GoodsTitle + '%'
                )
                order by g.GoodsID
                offset @Skip rows
                fetch next @PageSize rows only";

            var Result = await _dbConnection.QueryAsync<GoodsListResponse>(Sql, new
            {
                GoodsTitle = Request.GoodsTitle,
                Skip = Skip,
                PageSize = Request.PageSize
            });
            return Result.ToList();
        }
        #endregion
        #region 查询商品总数GetGoodsCountAsync
        public async Task<int> GetGoodsCountAsync(GetGoodsListRequest Request)
        {
            var Sql = @"select count(1) from Goods where(@GoodsTitle IS NULL 
                                                        OR @GoodsTitle = ''
                                                        OR GoodsTitle LIKE '%' + @GoodsTitle + '%'
                                                        )";
            var GoodsCount = await _dbConnection.ExecuteScalarAsync<int>(Sql, new
            {
                GoodsTitle = Request.GoodsTitle,
            });
            return GoodsCount;
        }
        #endregion
        #region 修改商品状态UpdateGoodsStatusAsync
        public async Task<bool> UpdateGoodsStatusAsync(int GoodsID, bool IsActive)
        {
            var Sql = @"update Goods set IsActive=@IsActive where GoodsID=@GoodsID";
            var Result = await _dbConnection.ExecuteAsync(Sql, new
            {
                GoodsID = GoodsID,
                IsActive = IsActive
            });
            return Result > 0;
        }
        #endregion
        #region 修改商品资料UpdateGoodsAsync
        public async Task<bool> UpdateGoodsAsync(Goods goods)
        {
            var Sql = @"update Goods set BarCode=@BarCode,GoodsCode=@GoodsCode,CategoryID=@CategoryID,GoodsTitle=@GoodsTitle,GoodsSubTitle=@GoodsSubTitle,GoodsPrice=@GoodsPrice,Unit=@Unit,UpdateTime=getdate() where GoodsID=@GoodsID";
            var Result = await _dbConnection.ExecuteAsync(Sql, new
            {
                GoodsID = goods.GoodsID,
                BarCode = goods.BarCode,
                GoodsCode = goods.GoodsCode,
                CategoryID = goods.CategoryID,
                GoodsTitle = goods.GoodsTitle,
                GoodsSubTitle = goods.GoodsSubTitle,
                GoodsPrice = goods.GoodsPrice,
                Unit = goods.Unit
            });
            return Result > 0;
        }
        #endregion
    }
}
