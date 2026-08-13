using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        #region 查询公司列表GetCompanyList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<CompanyResponse>>> GetCompanyList([FromQuery] GetCompanyListRequest request)
        {
            return await _companyService.GetCompanyListAsync(request);
        }
        #endregion
        #region 查询启用公司列表GetActiveCompanyList
        [HttpGet]
        public async Task<ApiResponse<List<CompanySelectResponse>>> GetActiveCompanyList()
        {
            return await _companyService.GetActiveCompanyListAsync();
        }
        #endregion
        #region 新增公司AddCompany
        [HttpPost]
        public async Task<ApiResponse<bool>> AddCompany(AddCompanyRequest request)
        {
            return await _companyService.AddCompanyAsync(request);
        }
        #endregion
        #region 修改公司UpdateCompany
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateCompany(UpdateCompanyRequest request)
        {
            return await _companyService.UpdateCompanyAsync(request);
        }
        #endregion
        #region 修改公司状态UpdateCompanyStatus
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateCompanyStatus(UpdateCompanyStatusRequest request)
        {
            return await _companyService.UpdateCompanyStatusAsync(request);
        }
        #endregion
    }
}
