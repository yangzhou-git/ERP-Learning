using System.Data;
using Dapper;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Repositories.Interfaces;

namespace ERP.Server.Repositories.Implementations
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbConnection _dbConnection;
        public CompanyRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region 查询公司列表GetCompanyListAsync
        public async Task<List<Company>> GetCompanyListAsync(GetCompanyListRequest request)
        {
            var skip = (request.Page - 1) * request.PageSize;
            var sql = @"select CompanyID, CompanyCode, CompanyName, Contact, Phone, IsActive, CreateTime, UpdateTime from Company where
                                                    (@CompanyName is null or @CompanyName='' or CompanyName like '%'+@CompanyName+'%')
                                                    and
                                                    (@IsActive is null or IsActive=@IsActive)
                                                    order by CompanyID desc
                                                    offset @Skip rows
                                                    fetch next @PageSize rows only";
            var result = await _dbConnection.QueryAsync<Company>(sql,new
                {
                    CompanyName = request.CompanyName,
                    IsActive = request.IsActive,
                    Skip = skip,
                    PageSize = request.PageSize
                });
            return result.ToList();
        }
        #endregion
        #region 查询公司总数GetCompanyCountAsync
        public async Task<int> GetCompanyCountAsync(GetCompanyListRequest request)
        {
            var sql = @"select count(1) from Company where (@CompanyName is null or @CompanyName='' or CompanyName like '%'+@CompanyName+'%') and (@IsActive is null or IsActive=@IsActive)";
            var result = await _dbConnection.ExecuteScalarAsync<int>(sql,new
                {
                    CompanyName = request.CompanyName,
                    IsActive = request.IsActive
                });
            return result;
        }
        #endregion
        #region 根据公司编码查询公司GetCompanyByCodeAsync
        public async Task<Company?> GetCompanyByCodeAsync(string companyCode)
        {
            var sql = "select CompanyID, CompanyCode, CompanyName, Contact, Phone, IsActive, CreateTime, UpdateTime from Company where CompanyCode = @CompanyCode";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Company>(sql, new { CompanyCode = companyCode });
            return result;
        }
        #endregion
        #region 查询启用公司列表GetActiveCompanyListAsync
        public async Task<List<Company>> GetActiveCompanyListAsync()
        {
            var sql = @"select CompanyID, CompanyName from Company where IsActive = 1";
            var result = await _dbConnection.QueryAsync<Company>(sql);
            return result.ToList();
        }
        #endregion
        #region 新增公司AddCompanyAsync
        public async Task<bool> AddCompanyAsync(Company company)
        {
            var sql = "insert into Company (CompanyCode,CompanyName,Contact,Phone,IsActive,CreateTime) values (@CompanyCode,@CompanyName,@Contact,@Phone,1,getdate())";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                CompanyCode = company.CompanyCode,
                CompanyName = company.CompanyName,
                Contact = company.Contact,
                Phone = company.Phone
            });
            return result > 0;
        }
        #endregion
        #region 修改公司UpdateCompanyAsync
        public async Task<bool> UpdateCompanyAsync(Company company)
        {
            var sql = @"update Company set CompanyName = @CompanyName, CompanyCode = @CompanyCode, Contact = @Contact, Phone = @Phone, UpdateTime = getdate() where CompanyID = @CompanyID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                CompanyID = company.CompanyID,
                CompanyName = company.CompanyName,
                CompanyCode = company.CompanyCode,
                Contact = company.Contact,
                Phone = company.Phone
            });
            return result > 0;
        }
        #endregion
        #region 修改公司状态UpdateCompanyStatusAsync
        public async Task<bool> UpdateCompanyStatusAsync(Company company)
        {
            var sql = "update Company set IsActive = @IsActive, UpdateTime = getdate() where CompanyID = @CompanyID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                CompanyID = company.CompanyID,
                IsActive = company.IsActive
            });
            return result > 0;
        }
        #endregion
    }
}
