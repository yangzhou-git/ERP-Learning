using System.Data;
using ERP.Server.Common;
using ERP.Server.Models.Enums;
using ERP.Server.Models.Request.BusinessCodeSeed;

namespace ERP.Server.Services.Interfaces.BusinessCode
{
    public interface IBusinessCodeService
    {
        #region 新增业务编号规则AddBusinessCodeSeedAsync
        Task <ApiResponse<bool>> AddBusinessCodeSeedAsync(AddBusinessCodeSeedRequest request);
        #endregion
        #region 获取下一个业务编号GetNextCodeAsync
        Task<ApiResponse<string?>> GetNextCodeAsync(BusinessCodeType businessCodeType, IDbTransaction transaction);
        #endregion
        #region 修改业务编号配置UpdateBusinessCodeSeedAsync
        Task<ApiResponse<bool>> UpdateBusinessCodeSeedAsync(UpdateBusinessCodeSeedRequest request);
        #endregion
    }
}
