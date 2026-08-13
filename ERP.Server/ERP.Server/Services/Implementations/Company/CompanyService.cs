using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        #region 查询公司列表GetCompanyListAsync
        public async Task<ApiResponse<PageResponse<CompanyResponse>>> GetCompanyListAsync(GetCompanyListRequest request)
        {
            var totalCount = await _companyRepository.GetCompanyCountAsync(request);
            var companyList = await _companyRepository.GetCompanyListAsync(request);
            var responseList = new List<CompanyResponse>();
            foreach (var company in companyList)
            {
                var companyResponse = new CompanyResponse
                {
                    CompanyID = company.CompanyID,
                    CompanyCode = company.CompanyCode,
                    CompanyName = company.CompanyName,
                    Contact = company.Contact,
                    Phone = company.Phone,
                    IsActive = company.IsActive,
                    CreateTime = company.CreateTime,
                    UpdateTime = company.UpdateTime
                };
                responseList.Add(companyResponse);
            }
            var pageResponse = new PageResponse<CompanyResponse>
            {
                TotalCount = totalCount,
                List = responseList
            };
            return ApiResponse<PageResponse<CompanyResponse>>.Success(pageResponse, "查询成功");
        }
        #endregion
        #region 查询启用公司列表GetActiveCompanyListAsync
        public async Task<ApiResponse<List<CompanySelectResponse>>> GetActiveCompanyListAsync()
        {
            var companylist = await _companyRepository.GetActiveCompanyListAsync();
            var company = new List<CompanySelectResponse>();
            foreach (var item in companylist)
            {
                var res = new CompanySelectResponse()
                {
                    CompanyID = item.CompanyID,
                    CompanyName = item.CompanyName
                };
                company.Add(res);
            }
            return ApiResponse<List<CompanySelectResponse>>.Success(company, "查询公司成功");
        }
        #endregion
        #region 新增公司AddCompanyAsync
        public async Task<ApiResponse<bool>> AddCompanyAsync(AddCompanyRequest request)
        {
            var existCompany = await _companyRepository.GetCompanyByCodeAsync(request.CompanyCode);
            if (existCompany != null)
            {
                return ApiResponse<bool>.Error($"公司编码已存在：{existCompany.CompanyName}");
            }
            var company = new Company()
            {
                CompanyCode = request.CompanyCode,
                CompanyName = request.CompanyName,
                Contact = request.Contact,
                Phone = request.Phone
            };
            var result = await _companyRepository.AddCompanyAsync(company);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "新增公司成功");
            }
            return ApiResponse<bool>.Error("新增公司失败");
        }
        #endregion
        #region 修改公司UpdateCompanyAsync
        public async Task<ApiResponse<bool>> UpdateCompanyAsync(UpdateCompanyRequest request)
        {
            var existCompany = await _companyRepository.GetCompanyByCodeAsync(request.CompanyCode);
            if (existCompany != null && existCompany.CompanyID != request.CompanyID)
            {
                return ApiResponse<bool>.Error($"公司编码已存在：{existCompany.CompanyName}");
            }
            var company = new Company()
            {
                CompanyID = request.CompanyID,
                CompanyCode = request.CompanyCode,
                CompanyName = request.CompanyName,
                Contact = request.Contact,
                Phone = request.Phone
            };
            var result = await _companyRepository.UpdateCompanyAsync(company);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改公司成功");
            }
            return ApiResponse<bool>.Error("修改公司失败");
        }
        #endregion
        #region 修改公司状态UpdateCompanyStatusAsync
        public async Task<ApiResponse<bool>> UpdateCompanyStatusAsync(UpdateCompanyStatusRequest request)
        {
            var company = new Company()
            {
                CompanyID = request.CompanyID,
                IsActive = request.IsActive
            };
            var result = await _companyRepository.UpdateCompanyStatusAsync(company);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改公司状态成功");
            }
            return ApiResponse<bool>.Error("修改公司状态失败");
        }
        #endregion
    }
}