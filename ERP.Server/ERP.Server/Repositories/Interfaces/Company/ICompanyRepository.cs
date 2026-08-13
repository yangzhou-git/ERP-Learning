using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;

namespace ERP.Server.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        #region 查询公司列表GetCompanyListAsync
        Task<List<Company>> GetCompanyListAsync(GetCompanyListRequest request);
        #endregion
        #region 查询公司总数GetCompanyCountAsync
        Task<int> GetCompanyCountAsync(GetCompanyListRequest request);
        #endregion
        #region 根据公司编码查询公司GetCompanyByCodeAsync
        Task<Company?> GetCompanyByCodeAsync(string companyCode);
        #endregion
        #region 查询启用公司列表GetActiveCompanyListAsync
        Task<List<Company>> GetActiveCompanyListAsync();
        #endregion
        #region 新增公司AddCompanyAsync
        Task<bool> AddCompanyAsync(Company company);
        #endregion
        #region 修改公司UpdateCompanyAsync
        Task<bool> UpdateCompanyAsync(Company company);
        #endregion
        #region 修改公司状态UpdateCompanyStatusAsync
        Task<bool> UpdateCompanyStatusAsync(Company company);
        #endregion
    }
}
