using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{
    public interface IUserService
    {
        #region 获取员工列表GetUserListAsync
        Task<ApiResponse<PageResponse<UserResponse>>> GetUserListAsync(GetUserListRequest request);
        #endregion

        #region 新增员工资料AddUserAsync
        Task<ApiResponse<bool>> AddUserAsync(AddUserRequest request);
        #endregion

        #region 修改员工资料UpdateUserAsync
        Task<ApiResponse<bool>> UpdateUserAsync(UpdateUserRequest request);
        #endregion

        #region 修改员工状态UpdateUserStatusAsync
        Task<ApiResponse<bool>> UpdateUserStatusAsync(UpdateUserStatusRequest request);
        #endregion
    }
}