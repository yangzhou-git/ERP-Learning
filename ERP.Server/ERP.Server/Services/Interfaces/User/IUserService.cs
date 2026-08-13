using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{
    public interface IUserService
    {
        #region 员工登陆LoginAsync
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest Request);
        #endregion
        #region 获取员工列表GetUserListAsync
        Task<ApiResponse<PageResponse<UserResponse>>> GetUserListAsync(GetUserListRequest Request);
        #endregion
        #region 新增员工资料AddUserAsync
        Task<ApiResponse<bool>> AddUserAsync(AddUserRequest Request);
        #endregion
        #region 修改员工资料UpdateUserAsync
        Task<ApiResponse<bool>> UpdateUserAsync(UpdateUserRequest Request);
        #endregion
        #region 修改员工状态UpdateUserStatusAsync
        Task<ApiResponse<bool>> UpdateUserStatusAsync(UpdateUserStatusRequest Request);
        #endregion
    }
}
