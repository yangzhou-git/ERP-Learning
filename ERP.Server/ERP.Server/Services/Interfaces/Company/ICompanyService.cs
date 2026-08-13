using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{
    public interface ICompanyService
    {
        #region 获取公司列表GetCompanyListAsync
        Task<ApiResponse<PageResponse<CompanyResponse>>> GetCompanyListAsync(GetCompanyListRequest request);
        #endregion
        #region 查询启用公司列表GetActiveCompanyListAsync
        Task<ApiResponse<List<CompanySelectResponse>>> GetActiveCompanyListAsync();
        #endregion
        #region 新增公司AddCompanyAsync
        Task<ApiResponse<bool>> AddCompanyAsync(AddCompanyRequest request);
        #endregion
        #region 修改公司UpdateCompanyAsync
        Task<ApiResponse<bool>> UpdateCompanyAsync(UpdateCompanyRequest request);
        #endregion
        #region 修改公司状态UpdateCompanyStatusAsync
        Task<ApiResponse<bool>> UpdateCompanyStatusAsync(UpdateCompanyStatusRequest request);
        #endregion
    }
}
