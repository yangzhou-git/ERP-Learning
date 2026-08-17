using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        #region 查询站点列表GetLocationListAsync
        public async Task<ApiResponse<PageResponse<LocationResponse>>> GetLocationListAsync(GetLocationListRequest request)
        {
            var locationList = await _locationRepository.GetLocationListAsync(request);
            var count = await _locationRepository.GetLocationCountAsync(request);
            var list = new List<LocationResponse>();
            foreach (var location in locationList)
            {
                var response = new LocationResponse
                {
                    LocationID = location.LocationID,
                    CompanyID = location.CompanyID,
                    CompanyName = location.CompanyName,
                    LocationCode = location.LocationCode,
                    LocationName = location.LocationName,
                    ParentID = location.ParentID,
                    CanSale = location.CanSale,
                    CanStock = location.CanStock,
                    CanReceive = location.CanReceive,
                    ProvinceID = location.ProvinceID,
                    CityID = location.CityID,
                    DistrictID = location.DistrictID,
                    Address = location.Address,
                    Contact = location.Contact,
                    Phone = location.Phone,
                    IsActive = location.IsActive,
                    CreateTime = location.CreateTime,
                    UpdateTime = location.UpdateTime
                };
                list.Add(response);
            }
            var pageResponse = new PageResponse<LocationResponse>
            {
                TotalCount = count,
                List = list
            };
            return ApiResponse<PageResponse<LocationResponse>>.Success(pageResponse, "查询站点成功");
        }
        #endregion
        #region 查询站点树GetLocationTreeAsync
        public async Task<ApiResponse<List<LocationTreeResponse>>> GetLocationTreeAsync(int CompanyID)
        {
            var locationList = await _locationRepository.GetLocationTreeAsync(CompanyID);
            if (locationList.Count == 0)
            {
                return ApiResponse<List<LocationTreeResponse>>.Success(new List<LocationTreeResponse>(),"暂无站点数据");
            }
            var locationTreeList = BuildLocationTree(locationList, null);
            return ApiResponse<List<LocationTreeResponse>>.Success(locationTreeList,"查询站点树成功");
        }
        #endregion
        #region 构建站点树BuildLocationTree
        private List<LocationTreeResponse> BuildLocationTree(List<Location> locationList, int? parentID)
        {
            var children = locationList.Where(x => x.ParentID == parentID).ToList();
            var treeList = new List<LocationTreeResponse>();
            foreach (var child in children)
            {
                var node = new LocationTreeResponse()
                {
                    LocationID = child.LocationID,
                    LocationName = child.LocationName,
                    ParentID = child.ParentID
                };
                node.Children = BuildLocationTree(locationList, child.LocationID);
                treeList.Add(node);
            }
            return treeList;
        }
        #endregion
        #region 新增站点AddLocationAsync
        public async Task<ApiResponse<bool>> AddLocationAsync(AddLocationRequest request)
        {
            var locationExist = await _locationRepository.GetLocationByCodeAsync(request.CompanyID,request.LocationCode);
            if (locationExist != null)
            {
                return ApiResponse<bool>.Error($"站点编码【{request.LocationCode}】已存在,站点名称:{locationExist.LocationName}");
            }
            if (request.ParentID.HasValue)
            {
                var parentLocation = await _locationRepository.GetLocationByIdAsync(request.ParentID.Value);
                if (parentLocation == null)
                {
                    return ApiResponse<bool>.Error($"父级站点ID【{request.ParentID.Value}】不存在");
                }
                if (parentLocation.CompanyID != request.CompanyID)
                {
                    return ApiResponse<bool>.Error($"新增站点【{request.LocationName}】不能选择其他公司的站点【{parentLocation.LocationName}】作为父级站点");
                }
            }
            var location = new Location()
            {
                CompanyID = request.CompanyID,
                LocationName = request.LocationName,
                LocationCode = request.LocationCode,
                ParentID = request.ParentID,
                CanSale = request.CanSale,
                CanStock = request.CanStock,
                CanReceive = request.CanReceive,
                ProvinceID = request.ProvinceID,
                CityID = request.CityID,
                DistrictID = request.DistrictID,
                Address = request.Address,
                Contact = request.Contact,
                Phone = request.Phone,
                IsActive = true
            };
            var result = await _locationRepository.AddLocationAsync(location);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "新增站点成功");
            }
            return ApiResponse<bool>.Error("新增站点失败");
        }
        #endregion
        #region 修改站点UpdateLocationAsync
        public async Task<ApiResponse<bool>> UpdateLocationAsync(UpdateLocationRequest request)
        {
            var currentLocation = await _locationRepository.GetLocationByIdAsync(request.LocationID);
            if (currentLocation == null)
            {
                return ApiResponse<bool>.Error($"站点ID【{request.LocationID}】不存在");
            }
            // 不能选择自己作为父级站点
            if (request.ParentID == request.LocationID)
            {
                return ApiResponse<bool>.Error($"站点【{currentLocation.LocationName}】不能选择自己作为父级站点");
            }
            // 选择了父级站点时进行校验
            if (request.ParentID.HasValue)
            {
                var parentTree = await _locationRepository.GetLocationParentTreeAsync(request.ParentID.Value);
                var parentLocation = parentTree.FirstOrDefault(x => x.LocationID == request.ParentID.Value);
                // 父级站点不存在
                if (parentLocation == null)
                {
                    return ApiResponse<bool>.Error($"父级站点ID【{request.ParentID.Value}】不存在");
                }
                // 不能跨公司选择父级站点
                if (parentLocation.CompanyID != currentLocation.CompanyID)
                {
                    return ApiResponse<bool>.Error($"站点【{currentLocation.LocationName}】不能选择其他公司的站点【{parentLocation.LocationName}】作为父级站点");
                }
                // 不能选择自己的子孙站点作为父级
                if (parentTree.Any(x => x.LocationID == request.LocationID))
                {
                    return ApiResponse<bool>.Error($"站点【{currentLocation.LocationName}】不能选择子孙站点【{parentLocation.LocationName}】作为父级站点");
                }
            }
            // 检查站点编码是否重复
            var existingLocation = await _locationRepository.GetLocationByCodeAsync(currentLocation.CompanyID,request.LocationCode);
            if (existingLocation != null && existingLocation.LocationID != request.LocationID)
            {
                return ApiResponse<bool>.Error($"站点编码【{request.LocationCode}】已存在,站点名称:{existingLocation.LocationName}");}

            var location = new Location()
            {
                LocationID = request.LocationID,
                LocationName = request.LocationName,
                LocationCode = request.LocationCode,
                ParentID = request.ParentID,
                CanSale = request.CanSale,
                CanReceive = request.CanReceive,
                CanStock = request.CanStock,
                ProvinceID = request.ProvinceID,
                CityID = request.CityID,
                DistrictID = request.DistrictID,
                Address = request.Address,
                Contact = request.Contact,
                Phone = request.Phone
            };
            var result = await _locationRepository.UpdateLocationAsync(location);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改站点成功");
            }
            return ApiResponse<bool>.Error("修改站点失败");
        }
        #endregion
        #region 修改站点状态UpdateLocationStatusAsync
        public async Task<ApiResponse<bool>> UpdateLocationStatusAsync(UpdateLocationStatusRequest request)
        {
            var newLocation = new Location()
            {
                LocationID = request.LocationID,
                IsActive = request.IsActive
            };
            var result = await _locationRepository.UpdateLocationStatusAsync(newLocation);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改站点状态成功");
            }
            return ApiResponse<bool>.Error("修改站点状态失败");
        }
        #endregion
    }
}
