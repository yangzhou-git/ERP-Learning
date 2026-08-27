using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERP.Server.Models.Response;

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

        #region 查询单位列表GetUnitList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<UnitListResponse>>> GetUnitList([FromQuery] GetUnitListRequest request)
        {
            return await _unitService.GetUnitListAsync(request);
        }
        #endregion

        #region 修改单位 UpdateUnit
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateUnit(UpdateUnitRequest request)
        {
            return await _unitService.UpdateUnitAsync(request);
        }
        #endregion

        #region 修改单位状态UpdateUnitStatus
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateUnitStatus(UpdateUnitStatusRequest request)
        {
            return await _unitService.UpdateUnitStatusAsync(request);
        }
        #endregion
    }
}
