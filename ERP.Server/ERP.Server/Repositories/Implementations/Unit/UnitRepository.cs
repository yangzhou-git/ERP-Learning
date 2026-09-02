using System.Data;
using System.Reflection.Metadata.Ecma335;
using Dapper;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response.Unit;
using ERP.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP.Server.Repositories.Implementations
{
    public class UnitRepository : IUnitRepository
    {
        private readonly IDbConnection _dbConnection;

        public UnitRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region 根据单位名称查询GetUnitByNameAsync
        public async Task<Unit?> GetUnitByNameAsync(string unitName)
        {
            var sql = @"select UnitID, UnitName, UnitCode, IsActive, Sort, CreateUserID, CreateTime, UpdateUserID, UpdateTime from Unit where UnitName = @UnitName";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Unit>(sql, new
            {
                UnitName = unitName
            });
            return result;
        }
        #endregion

        #region 根据单位编码查询GetUnitByCodeAsync
        public async Task<Unit?> GetUnitByCodeAsync(string unitCode)
        {
            var sql = @"select UnitID, UnitName, UnitCode, IsActive, Sort, CreateUserID, CreateTime, UpdateUserID, UpdateTime from Unit where UnitCode = @UnitCode";
            var result = await  _dbConnection.QueryFirstOrDefaultAsync<Unit>(sql, new
            {
                UnitCode = unitCode
            });
            return result;
        }
        #endregion

        #region 新增单位AddUnitAsync
        public async Task<bool> AddUnitAsync(Unit unit) 
        {
            var sql = @"insert into Unit (UnitName, UnitCode, IsActive, Sort, CreateUserID, CreateTime) values (@UnitName, @UnitCode, @IsActive, @Sort, @CreateUserID, getdate())";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                UnitName = unit.UnitName,
                UnitCode = unit.UnitCode,
                IsActive = unit.IsActive,
                Sort = unit.Sort,
                CreateUserID = unit.CreateUserID
            });
            return result > 0;
        }
        #endregion

        #region 查询单位下拉选项GetUnitOptionsAsync
        public async Task<List<UnitOptionResponse>> GetUnitOptionsAsync()
        {
            var sql = @"select UnitID, UnitName, UnitCode from Unit where IsActive = 1 order by Sort asc, UnitID asc";
            var result = await _dbConnection.QueryAsync<UnitOptionResponse>(sql);
            return result.ToList();
        }
        #endregion

        #region 查询单位列表GetUnitListAsync
        public async Task<List<Unit>> GetUnitListAsync(GetUnitListRequest request)
        {
            var skip = (request.Page - 1) * request.PageSize;
            var sql = @"select UnitID, UnitName, UnitCode, IsActive, Sort, CreateUserID, CreateTime, UpdateUserID, UpdateTime from Unit 
                        where (@UnitName is null or @UnitName = '' or UnitName like '%' + @UnitName + '%')
                          and (@UnitCode is null or @UnitCode = '' or UnitCode like '%' + @UnitCode + '%')
                          and (@IsActive is null or IsActive = @IsActive)
                        order by Sort asc, UnitID asc
                        offset @Skip rows
                        fetch next @PageSize rows only";
            var result = await _dbConnection.QueryAsync<Unit>(sql, new
            {
                UnitName = request.UnitName,
                UnitCode = request.UnitCode,
                IsActive = request.IsActive,
                Skip = skip,
                PageSize = request.PageSize
            });
            return result.ToList();
        }
        #endregion

        #region 查询单位数量GetUnitCountAsync
        public async Task<int> GetUnitCountAsync(GetUnitListRequest request)
        {
            var sql = @"select count(1) from Unit 
                        where (@UnitName is null or @UnitName = '' or UnitName like '%' + @UnitName + '%')
                          and (@UnitCode is null or @UnitCode = '' or UnitCode like '%' + @UnitCode + '%')
                          and (@IsActive is null or IsActive = @IsActive)";
            return await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                UnitName = request.UnitName,
                UnitCode = request.UnitCode,
                IsActive = request.IsActive
            });
        }
        #endregion

        #region 根据单位ID查询GetUnitByIdAsync
        public async Task<Unit?> GetUnitByIdAsync(int unitID)
        {
            var sql = @"select UnitID, UnitName, UnitCode, IsActive, Sort, CreateUserID, CreateTime, UpdateUserID, UpdateTime from Unit where UnitID = @UnitID";
            return await _dbConnection.QueryFirstOrDefaultAsync<Unit>(sql, new
            {
                UnitID = unitID
            });
        }
        #endregion

        #region 修改单位UpdateUnitAsync
        public async Task<bool> UpdateUnitAsync(Unit unit)
        {
            var sql = @"update Unit set UnitName = @UnitName, UnitCode = @UnitCode, Sort = @Sort, UpdateUserID = @UpdateUserID, UpdateTime = getdate() where UnitID = @UnitID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                UnitID = unit.UnitID,
                UnitName = unit.UnitName,
                UnitCode = unit.UnitCode,
                Sort = unit.Sort,
                UpdateUserID = unit.UpdateUserID
            });
            return result > 0;
        }
        #endregion

        #region 修改单位状态UpdateUnitStatusAsync
        public async Task<bool> UpdateUnitStatusAsync(Unit unit)
        {
            var sql = @"update Unit set IsActive = @IsActive, UpdateUserID = @UpdateUserID, UpdateTime = getdate() where UnitID = @UnitID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                UnitID = unit.UnitID,
                IsActive = unit.IsActive,
                UpdateUserID = unit.UpdateUserID
            });
            return result > 0;
        }
        #endregion
    }
}
