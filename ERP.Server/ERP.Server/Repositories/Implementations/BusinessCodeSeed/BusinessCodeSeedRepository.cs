using System.Data;
using Dapper;
using ERP.Server.Models.Entity.BusinessCodeSeed;
using ERP.Server.Models.Enums;
using ERP.Server.Repositories.Interfaces;

namespace ERP.Server.Repositories.Implementations
{
    public class BusinessCodeSeedRepository : IBusinessCodeSeedRepository
    {
        private readonly IDbConnection _dbconnection;

        public BusinessCodeSeedRepository(IDbConnection dbconnection)
        {
            _dbconnection = dbconnection;
        }

        #region 新增业务编号规则AddBusinessCodeSeedAsync
        public async Task<bool> AddBusinessCodeSeedAsync(BusinessCodeSeed businessCodeSeed, IDbTransaction transaction)
        {
            var sql = @"insert into BusinessCodeSeed 
                        (CompanyID, BusinessType, Prefix, NumberLength, ResetType, CurrentNumber, NumberDate, CreateUserID, CreateTime)
                        values
                        (@CompanyID, @BusinessType, @Prefix, @NumberLength, @ResetType, @CurrentNumber, @NumberDate, @CreateUserID, getdate())";
            return await _dbconnection.ExecuteAsync(sql, new
            {
                CompanyID = businessCodeSeed.CompanyID,
                BusinessType = businessCodeSeed.BusinessType,
                Prefix = businessCodeSeed.Prefix,
                NumberLength = businessCodeSeed.NumberLength,
                ResetType = businessCodeSeed.ResetType,
                CurrentNumber = businessCodeSeed.CurrentNumber,
                NumberDate = businessCodeSeed.NumberDate,
                CreateUserID = businessCodeSeed.CreateUserID
            }, transaction) > 0;
        }
        #endregion

        #region 根据ID加锁查询业务编号规则GetBusinessCodeSeedByIDForUpdateAsync
        public async Task<BusinessCodeSeed?> GetBusinessCodeSeedByIDForUpdateAsync(int businessCodeSeedID, int companyID, IDbTransaction transaction)
        {
            var sql = @"select BusinessCodeSeedID,
                      CompanyID,
                      BusinessType,
                      Prefix,
                      NumberLength,
                      ResetType,
                      CurrentNumber,
                      NumberDate,
                      HasBeenUsed
                from BusinessCodeSeed with (updlock, holdlock, rowlock)
                where BusinessCodeSeedID = @businessCodeSeedID and CompanyID = @companyID";

            return await _dbconnection.QueryFirstOrDefaultAsync<BusinessCodeSeed>(sql, new { businessCodeSeedID, companyID }, transaction);
        }
        #endregion

        #region 根据公司和业务类型查询业务编号种子并加锁GetBusinessCodeSeedForUpdateAsync
        public async Task<BusinessCodeSeed?> GetBusinessCodeSeedForUpdateAsync(int companyID, BusinessCodeType businessType, IDbTransaction transaction)
        {
            var sql = @"select BusinessCodeSeedID,
                      CompanyID,
                      BusinessType,
                      Prefix,
                      NumberLength,
                      ResetType,
                      CurrentNumber,
                      NumberDate,
                      HasBeenUsed
                from BusinessCodeSeed with (updlock, holdlock, rowlock)
                where CompanyID = @companyID and BusinessType = @businessType";

            return await _dbconnection.QueryFirstOrDefaultAsync<BusinessCodeSeed>(sql, new { companyID, businessType }, transaction);
        }
        #endregion

        #region 更新业务编号种子UpdateBusinessCodeSeedAsync
        public async Task<bool> UpdateBusinessCodeSeedAsync(int companyID, BusinessCodeSeed businessCodeSeed, IDbTransaction transaction)
        {
            var sql = @"update BusinessCodeSeed
                        set CurrentNumber = @CurrentNumber,
                            NumberDate = @NumberDate,
                            HasBeenUsed = @HasBeenUsed,
                            UpdateUserID = @UpdateUserID,
                            UpdateTime = getdate()
                        where BusinessCodeSeedID = @BusinessCodeSeedID
                          and CompanyID = @companyID";
            return await _dbconnection.ExecuteAsync(sql, new
            {
                companyID,
                BusinessCodeSeedID = businessCodeSeed.BusinessCodeSeedID,
                CurrentNumber = businessCodeSeed.CurrentNumber,
                NumberDate = businessCodeSeed.NumberDate,
                HasBeenUsed = businessCodeSeed.HasBeenUsed,
                UpdateUserID = businessCodeSeed.UpdateUserID
            }, transaction) > 0;
        }
        #endregion

        #region 修改业务编号配置UpdateBusinessCodeSeedConfigAsync
        public async Task<bool> UpdateBusinessCodeSeedConfigAsync(int companyID, BusinessCodeSeed businessCodeSeed, IDbTransaction transaction)
        {
            var sql = @"update BusinessCodeSeed
                        set BusinessType = @BusinessType,
                            Prefix = @Prefix,
                            NumberLength = @NumberLength,
                            ResetType = @ResetType,
                            UpdateUserID = @UpdateUserID,
                            UpdateTime = getdate()
                        where BusinessCodeSeedID = @BusinessCodeSeedID
                          and CompanyID = @companyID";
            return await _dbconnection.ExecuteAsync(sql, new
            {
                companyID,
                BusinessCodeSeedID = businessCodeSeed.BusinessCodeSeedID,
                BusinessType = businessCodeSeed.BusinessType,
                Prefix = businessCodeSeed.Prefix,
                NumberLength = businessCodeSeed.NumberLength,
                ResetType = businessCodeSeed.ResetType,
                UpdateUserID = businessCodeSeed.UpdateUserID
            }, transaction) > 0;
        }
        #endregion
    }
}
