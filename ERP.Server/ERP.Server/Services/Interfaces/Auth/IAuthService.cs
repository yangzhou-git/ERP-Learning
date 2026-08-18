using ERP.Server.Common;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{
    public interface IAuthService
    {
        #region 登录LoginAsync
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        #endregion
    }
}
