using System.Data;
using ERP.Server.Models.Entity.BusinessCodeSeed;
using ERP.Server.Models.Enums;

namespace ERP.Server.Repositories.Interfaces
{
    public interface IBusinessCodeSeedRepository
    {
        #region 新增业务编号规则AddBusinessCodeSeedAsync
        Task<bool> AddBusinessCodeSeedAsync(BusinessCodeSeed businessCodeSeed, IDbTransaction transaction);
        #endregion
        #region 根据公司和业务类型查询业务编号种子并加锁GetBusinessCodeSeedForUpdateAsync
        Task<BusinessCodeSeed?> GetBusinessCodeSeedForUpdateAsync(int companyID, BusinessCodeType businessType, IDbTransaction transaction);
        #endregion
        #region 根据ID加锁查询业务编号规则GetBusinessCodeSeedByIDForUpdateAsync
        Task<BusinessCodeSeed?> GetBusinessCodeSeedByIDForUpdateAsync(int businessCodeSeedID, int companyID, IDbTransaction transaction);
        #endregion
        #region 更新业务编号种子UpdateBusinessCodeSeedAsync
        Task<bool> UpdateBusinessCodeSeedAsync(int companyID, BusinessCodeSeed businessCodeSeed, IDbTransaction transaction);
        #endregion
        #region 修改业务编号配置UpdateBusinessCodeSeedConfigAsync
        Task<bool> UpdateBusinessCodeSeedConfigAsync(int companyID, BusinessCodeSeed businessCodeSeed, IDbTransaction transaction);
        #endregion
    }
}
