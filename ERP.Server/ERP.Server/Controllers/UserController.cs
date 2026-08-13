using ERP.Server.Common;
using Microsoft.AspNetCore.Mvc;
using ERP.Server.Services.Interfaces;
using ERP.Server.Models.Response;
using ERP.Server.Models.Request;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region 获取员工列表GetUserList
        [HttpGet]
        public async Task<ApiResponse<PageResponse<UserResponse>>> GetUserList([FromQuery] GetUserListRequest Request)
        {
            var Result = await _userService.GetUserListAsync(Request);
            return Result;
        }
        #endregion
        #region 新增员工 AddUser
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
        public async Task<ApiResponse<bool>> UpdateUserStatus(UpdateUserStatusRequest Request)
        {
            return await _userService.UpdateUserStatusAsync(Request);
        }
        #endregion
    }
}
