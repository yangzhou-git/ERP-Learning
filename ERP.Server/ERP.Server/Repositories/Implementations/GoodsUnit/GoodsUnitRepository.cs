using System.Data;
using Dapper;
using ERP.Server.Models.Entity;
using ERP.Server.Repositories.Interfaces;

namespace ERP.Server.Repositories.Implementations
{
    public class GoodsUnitRepository : IGoodsUnitRepository
    {
        private readonly IDbConnection _dbConnection;

        public GoodsUnitRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region 根据商品ID查询基础单位GetBaseUnitByGoodsIDAsync
        public async Task<GoodsUnit?> GetBaseUnitByGoodsIDAsync(int goodsID)
        {
            var sql = @"select GoodsUnitID, GoodsID, UnitID, ConversionRate, IsBaseUnit, IsActive, Sort, CreateUserID, CreateTime, UpdateUserID, UpdateTime from GoodsUnit
                        where GoodsID = @GoodsID and IsBaseUnit = 1";
            return await _dbConnection.QueryFirstOrDefaultAsync<GoodsUnit>(sql, new
            {
                GoodsID = goodsID
            });
        }
        #endregion

        #region 根据商品ID和单位ID查询商品单位关系GetGoodsUnitByGoodsAndUnitAsync
        public async Task<GoodsUnit?> GetGoodsUnitByGoodsAndUnitAsync(int goodsID, int unitID)
        {
            var sql = @"select GoodsUnitID, GoodsID, UnitID, ConversionRate, IsBaseUnit, IsActive, Sort, CreateUserID, CreateTime, UpdateUserID, UpdateTime from GoodsUnit
                        where GoodsID = @GoodsID and UnitID = @UnitID";
            return await _dbConnection.QueryFirstOrDefaultAsync<GoodsUnit>(sql, new
            {
                GoodsID = goodsID,
                UnitID = unitID
            });
        }
        #endregion

        #region 新增商品单位关系AddGoodsUnitAsync
        public async Task<bool> AddGoodsUnitAsync(GoodsUnit goodsUnit)
        {
            var sql = @"insert into GoodsUnit (GoodsID, UnitID, ConversionRate, IsBaseUnit, IsActive, Sort, CreateUserID, CreateTime) values (@GoodsID, @UnitID, @ConversionRate, @IsBaseUnit, @IsActive, @Sort, @CreateUserID, getdate())";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                GoodsID = goodsUnit.GoodsID,
                UnitID = goodsUnit.UnitID,
                ConversionRate = goodsUnit.ConversionRate,
                IsBaseUnit = goodsUnit.IsBaseUnit,
                IsActive = goodsUnit.IsActive,
                Sort = goodsUnit.Sort,
                CreateUserID = goodsUnit.CreateUserID
            });
            return result > 0;
        }
        #endregion
    }
}
