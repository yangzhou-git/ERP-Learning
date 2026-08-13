using ERP.Server.Common;
using ERP.Server.Models.Response;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        #region 查询地区树（懒加载）GetAreaTree
        [HttpGet]
        public async Task<ApiResponse<List<AreaTreeResponse>>> GetAreaTree(int? parentID)
        {
            return await _areaService.GetAreaTreeAsync(parentID);
        }
        #endregion
    }
}
