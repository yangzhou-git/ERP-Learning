using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
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
    }
}
