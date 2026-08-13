using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        #region 登陆接口Login
        [HttpPost]
        public async Task<ApiResponse<LoginResponse>> Login(LoginRequest request)
        {
            return await _userService.LoginAsync(request);
        }
        #endregion
    }
}