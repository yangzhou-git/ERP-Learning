using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ERP.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]/[action]")]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        #region 新增单位AddUnit
        [HttpPost]
        public async Task<ApiResponse<bool>> AddUnit(AddUnitRequest request)
        {
            return await _unitService.AddUnitAsync(request);
        }
        #endregion
    }
}
