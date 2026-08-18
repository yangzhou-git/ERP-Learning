using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region 获取员工列表GetUserList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<UserResponse>>> GetUserList([FromQuery] GetUserListRequest request)
        {
            return await _userService.GetUserListAsync(request);
        }
        #endregion
        #region 新增员工AddUser
        [HttpPost]
        public async Task<ApiResponse<bool>> AddUser(AddUserRequest request)
        {
            return await _userService.AddUserAsync(request);
        }
        #endregion
        #region 修改员工资料UpdateUser
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateUser(UpdateUserRequest request)
        {
            return await _userService.UpdateUserAsync(request);
        }
        #endregion
        #region 修改员工状态UpdateUserStatus
        [HttpPost]
        public async Task<ApiResponse<bool>> UpdateUserStatus(UpdateUserStatusRequest request)
        {
            return await _userService.UpdateUserStatusAsync(request);
        }
        #endregion
    }
}