using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;

        public AreaService(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }

        #region 查询地区树（懒加载）GetAreaTreeAsync
        public async Task<ApiResponse<List<AreaTreeResponse>>> GetAreaTreeAsync(int? ParentID)
        {
            var AreaList = await _areaRepository.GetAreaTreeAsync(ParentID);
            var areas = new List<AreaTreeResponse>();
            foreach (var area in AreaList)
            {
                var response = new AreaTreeResponse()
                {
                    AreaID = area.AreaID,
                    AreaCode = area.AreaCode,
                    AreaName = area.AreaName,
                    ParentID = area.ParentID,
                    AreaLevel = area.AreaLevel
                };
                areas.Add(response);
            };
            return ApiResponse<List<AreaTreeResponse>>.Success(areas, "查询地区成功");
        }
        #endregion
    }
}