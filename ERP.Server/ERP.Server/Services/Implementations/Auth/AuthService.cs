using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace ERP.Server.Services.Implementations.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher , IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        #region 登录LoginAsync
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByUserNameAsync (request.UserName);
            if(user == null)
            {
                return ApiResponse<LoginResponse>.Error("用户名或密码错误");
            }
            if (!user.IsActive)
            {
                return ApiResponse<LoginResponse>.Error("当前账号已被禁用");
            }
            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return ApiResponse<LoginResponse>.Error("用户名或密码错误");
            }
            var claims = new List<Claim>
            {
                new Claim("UserID", user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("CompanyID", user.CompanyID.ToString())
            };
            // 先读取JWT配置
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var expireMinutes = _configuration.GetValue<int>("Jwt:ExpireMinutes");
            // 再使用配置创建签名密钥
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            );
            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );
            var jwtToken = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var response = new LoginResponse
            {
                UserID = user.UserID,
                UserName = user.UserName,
                CompanyID = user.CompanyID,
                Token = token
            };
            return ApiResponse<LoginResponse>.Success(response, "登录成功");
        }
        #endregion
    }
}
