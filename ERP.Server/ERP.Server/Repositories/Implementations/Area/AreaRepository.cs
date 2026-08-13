using System.Data;
using Dapper;
using ERP.Server.Models.Entity;
using ERP.Server.Repositories.Interfaces;

namespace ERP.Server.Repositories.Implementations
{
    public class AreaRepository : IAreaRepository
    {
        private readonly IDbConnection _dbConnection;

        public AreaRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region 查询地区树（懒加载）GetAreaTreeAsync
        public async Task<List<Area>> GetAreaTreeAsync(int? ParentID)
        {
            var sql = @"select AreaID,AreaCode,AreaName,ParentID,AreaLevel,ZipCode,IsActive,CreateTime,UpdateTime,Sort
                from Area
                where IsActive = 1
                and ((@ParentID is null and ParentID is null)or(@ParentID is not null and ParentID = @ParentID)) 
                order by Sort";
            var result = await _dbConnection.QueryAsync<Area>(sql, new
            {
                ParentID = ParentID
            });
            return result.ToList();
        }
        #endregion
    }
}
