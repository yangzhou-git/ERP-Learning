using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region 登陆接口Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResponse<LoginResponse>> Login(LoginRequest request)
        {
            return await _authService.LoginAsync(request);
        }
        #endregion
    }
}
