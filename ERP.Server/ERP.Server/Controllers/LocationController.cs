using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        #region 查询站点列表GetLocationList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<LocationResponse>>> GetLocationList([FromQuery] GetLocationListRequest request)
        {
            return await _locationService.GetLocationListAsync(request);
        }
        #endregion
        #region 查询站点树GetLocationTree
        [HttpGet]
        public async Task<ApiResponse<List<LocationTreeResponse>>> GetLocationTree(int CompanyID)
        {
            return await _locationService.GetLocationTreeAsync(CompanyID);
        }
        #endregion
        #region 新增站点AddLocation
        [HttpPost]
        public async Task<ApiResponse<bool>> AddLocation(AddLocationRequest request)
        {
            return await _locationService.AddLocationAsync(request);
        }
        #endregion
        #region 修改站点UpdateLocation
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateLocation(UpdateLocationRequest request)
        {
            return await _locationService.UpdateLocationAsync(request);
        }
        #endregion
        #region 修改站点状态UpdateLocationStatus
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateLocationStatus(UpdateLocationStatusRequest request)
        {
            return await _locationService.UpdateLocationStatusAsync(request);
        }
        #endregion
    }
}
