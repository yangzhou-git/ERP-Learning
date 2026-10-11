using ERP.Server.Common;
using ERP.Server.Models.Request.BusinessCodeSeed;
using ERP.Server.Services.Interfaces.BusinessCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class BusinessCodeController : ControllerBase
    {
        private readonly IBusinessCodeService _businessCodeService;

        public BusinessCodeController(IBusinessCodeService businessCodeService)
        {
            _businessCodeService = businessCodeService;
        }

        #region 新增业务编号规则AddBusinessCodeSeed
        [HttpPost]
        public async Task<ApiResponse<bool>> AddBusinessCodeSeed([FromBody] AddBusinessCodeSeedRequest request)
        {
            return await _businessCodeService.AddBusinessCodeSeedAsync(request);
        }
        #endregion

        #region 修改业务编号规则UpdateBusinessCodeSeed
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateBusinessCodeSeed([FromBody] UpdateBusinessCodeSeedRequest request)
        {
            return await _businessCodeService.UpdateBusinessCodeSeedAsync(request);
        }
        #endregion
    }
}
