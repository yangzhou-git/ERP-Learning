using System.Security.Claims;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserID
        {
            get
            {
                var userIDClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;
                if (int.TryParse(userIDClaim, out var userID))
                {
                    return userID;
                }
                throw new UnauthorizedAccessException("无法获取当前登录用户ID");
            }
        }

        public string UserName
        {
            get
            {
                var userName = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
                if (userName != null)
                {
                    return userName;
                }
                throw new UnauthorizedAccessException("无法获取当前登录用户姓名");
            }
        }
        public int CompanyID
        {
            get
            {
                var companyIDClaim = _httpContextAccessor.HttpContext?.User.FindFirst("CompanyID")?.Value;
                if (int.TryParse(companyIDClaim, out var companyID))
                {
                    return companyID;
                }
                throw new UnauthorizedAccessException("无法获取当前登陆用户公司ID");
            }
        }
    }
}
