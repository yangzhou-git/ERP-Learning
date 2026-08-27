using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;
        private readonly ICurrentUserService _currentUserService;

        public UnitService(IUnitRepository unitRepository, ICurrentUserService currentUserService) 
        {
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
        }

        #region 新增单位AddUnitAsync
        public async Task<ApiResponse<bool>> AddUnitAsync(AddUnitRequest request)
        {
            var userID = _currentUserService.UserID;
            var existingUnitByName = await _unitRepository.GetUnitByNameAsync(request.UnitName);
            if (existingUnitByName != null)
            {
                return ApiResponse<bool>.Error($"单位名称【{request.UnitName}】已存在");
            }
            var existingUnitByCode = await _unitRepository.GetUnitByCodeAsync(request.UnitCode);
            if (existingUnitByCode != null) 
            {
                return ApiResponse<bool>.Error($"单位编码【{request.UnitCode}】已存在");
            }
            var unit = new Unit()
            {
                UnitName = request.UnitName,
                UnitCode = request.UnitCode,
                IsActive = true,
                Sort = 0,
                CreateUserID = userID
            };
            var result = await _unitRepository.AddUnitAsync(unit);
            if (result)
            {
                return ApiResponse<bool>.Success(true,"新增单位成功");
            }
            return ApiResponse<bool>.Error("新增单位失败");
        }
        #endregion

        #region 查询单位列表GetUnitListAsync
        public async Task<ApiResponse<PageResponse<UnitListResponse>>> GetUnitListAsync(GetUnitListRequest request)
        {
            var units = await _unitRepository.GetUnitListAsync(request);
            var totalCount = await _unitRepository.GetUnitCountAsync(request);
            var unitResponses = new List<UnitListResponse>();
            foreach (var unit in units)
            {
                var unitResponse = new UnitListResponse()
                {
                    UnitID = unit.UnitID,
                    UnitName = unit.UnitName,
                    UnitCode = unit.UnitCode,
                    IsActive = unit.IsActive,
                    Sort = unit.Sort,
                    CreateUserID = unit.CreateUserID,
                    CreateTime = unit.CreateTime,
                    UpdateTime = unit.UpdateTime,
                    UpdateUserID = unit.UpdateUserID
                };
                unitResponses.Add(unitResponse);
            }
            var pageResponse = new PageResponse<UnitListResponse>()
            {
                TotalCount = totalCount,
                List = unitResponses
            };
            return ApiResponse<PageResponse<UnitListResponse>>.Success(pageResponse, "查询单位列表成功");
        }
        #endregion

        #region 修改单位UpdateUnitAsync
        public async Task<ApiResponse<bool>> UpdateUnitAsync(UpdateUnitRequest request) 
        {
            var unit = await _unitRepository.GetUnitByIdAsync(request.UnitID);
            if(unit == null)
            {
                return ApiResponse<bool>.Error("单位不存在");
            }
            var unitByName = await _unitRepository.GetUnitByNameAsync(request.UnitName);
            if (unitByName != null && unitByName.UnitID != request.UnitID)
            {
                return ApiResponse<bool>.Error($"单位名称【{request.UnitName}】已存在,请使用其他单位名称");
            }
            var unitByCode = await _unitRepository.GetUnitByCodeAsync(request.UnitCode);
            if (unitByCode != null && unitByCode.UnitID != request.UnitID)
            {
                return ApiResponse<bool>.Error($"单位编码【{request.UnitCode}】已存在,请使用其他单位编码");
            }
            unit.UnitName = request.UnitName;
            unit.UnitCode = request.UnitCode;
            unit.UpdateUserID = _currentUserService.UserID;
            var result = await _unitRepository.UpdateUnitAsync(unit);
            if (result)
            {
                return ApiResponse<bool>.Success(true,"修改单位成功");
            }
            return ApiResponse<bool>.Error("修改单位失败");
        }
        #endregion

        #region 修改单位状态UpdateUnitStatusAsync
        public async Task<ApiResponse<bool>> UpdateUnitStatusAsync(UpdateUnitStatusRequest request)
        {
            var unit = await _unitRepository.GetUnitByIdAsync(request.UnitID);
            if (unit == null)
            {
                return ApiResponse<bool>.Error("单位不存在");
            }
            unit.IsActive = request.IsActive;
            unit.UpdateUserID = _currentUserService.UserID;
            var result = await _unitRepository.UpdateUnitStatusAsync(unit);
            if (result) 
            {
                return ApiResponse<bool>.Success(true, "修改单位成功");
            }
            return ApiResponse<bool>.Error("修改单位失败");
        }
        #endregion
    }
}
