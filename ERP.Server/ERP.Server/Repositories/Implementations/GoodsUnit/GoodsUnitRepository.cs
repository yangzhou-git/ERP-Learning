using System.Data;
using Dapper;
using ERP.Server.Models.DTO.GoodsUnit;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request.GoodsUnit;
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

        #region 根据商品单位关系ID查询GetGoodsUnitByIdAsync
        public async Task<GoodsUnit?> GetGoodsUnitByIdAsync(int goodsUnitID)
        {
            var sql = @"select GoodsUnitID, GoodsID, UnitID, ConversionRate, IsBaseUnit, IsActive, Sort, CreateUserID, CreateTime, UpdateUserID, UpdateTime from GoodsUnit
                        where GoodsUnitID = @GoodsUnitID";
            return await _dbConnection.QueryFirstOrDefaultAsync<GoodsUnit>(sql, new
            {
                GoodsUnitID = goodsUnitID
            });
        }
        #endregion

        #region 查询商品单位列表GetGoodsUnitListAsync
        public async Task<List<GoodsUnitListDto>> GetGoodsUnitListAsync(GetGoodsUnitListRequest request)
        {
            var skip = (request.Page - 1) * request.PageSize;

            var sql = @"select gu.GoodsUnitID, gu.GoodsID, gu.UnitID, u.UnitName, u.UnitCode,
                        gu.ConversionRate, gu.IsBaseUnit, gu.IsActive, gu.Sort,
                        gu.CreateUserID,
                        cu.UserName as CreateUserName,
                        gu.CreateTime,
                        gu.UpdateUserID,
                        uu.UserName as UpdateUserName,
                        gu.UpdateTime
                        from GoodsUnit gu
                        join Unit u on gu.UnitID = u.UnitID
                        left join Users cu on gu.CreateUserID = cu.UserID
                        left join Users uu on gu.UpdateUserID = uu.UserID
                        where gu.GoodsID = @GoodsID
                        and (@UnitName is null or u.UnitName like '%' + @UnitName + '%')
                        and (@IsBaseUnit is null or gu.IsBaseUnit = @IsBaseUnit)
                        and (@IsActive is null or gu.IsActive = @IsActive)
                        order by gu.IsBaseUnit desc, gu.Sort asc, gu.GoodsUnitID asc
                        offset @Skip rows
                        fetch next @PageSize rows only";

            var result = await _dbConnection.QueryAsync<GoodsUnitListDto>(sql, new
            {
                GoodsID = request.GoodsID,
                UnitName = request.UnitName,
                IsBaseUnit = request.IsBaseUnit,
                IsActive = request.IsActive,
                Skip = skip,
                PageSize = request.PageSize
            });

            return result.ToList();
        }
        #endregion

        #region 查询商品单位数量GetGoodsUnitCountAsync
        public async Task<int> GetGoodsUnitCountAsync(GetGoodsUnitListRequest request)
        {
            var sql = @"select count(1) from GoodsUnit gu
                        join Unit u on gu.UnitID = u.UnitID
                        where gu.GoodsID = @GoodsID
                        and (@UnitName is null or u.UnitName like '%' + @UnitName + '%')
                        and (@IsBaseUnit is null or gu.IsBaseUnit = @IsBaseUnit)
                        and (@IsActive is null or gu.IsActive = @IsActive)";
            var result = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                GoodsID = request.GoodsID,
                UnitName = request.UnitName,
                IsBaseUnit = request.IsBaseUnit,
                IsActive= request.IsActive
            });
            return result;
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

        #region 修改商品单位UpdateGoodsUnitAsync
        public async Task<bool> UpdateGoodsUnitAsync(GoodsUnit goodsUnit)
        {
            var sql = @"update GoodsUnit
                        set UnitID = @UnitID,
                        ConversionRate = @ConversionRate,
                        Sort = @Sort,
                        UpdateUserID = @UpdateUserID,
                        UpdateTime = getdate()
                        where GoodsUnitID = @GoodsUnitID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                GoodsUnitID = goodsUnit.GoodsUnitID,
                UnitID = goodsUnit.UnitID,
                ConversionRate = goodsUnit.ConversionRate,
                Sort = goodsUnit.Sort,
                UpdateUserID = goodsUnit.UpdateUserID
            });
            return result > 0;
        }
        #endregion

        #region 修改商品单位关系状态UpdateGoodsUnitStatusAsync
        public async Task<bool> UpdateGoodsUnitStatusAsync(GoodsUnit goodsUnit)
        {
            var sql = @"update GoodsUnit set IsActive = @IsActive, UpdateUserID = @UpdateUserID, UpdateTime = getdate() where GoodsUnitID = @GoodsUnitID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                IsActive = goodsUnit.IsActive,
                UpdateUserID = goodsUnit.UpdateUserID,
                GoodsUnitID = goodsUnit.GoodsUnitID
            });
            return result > 0;
        }
        #endregion
    }
}
