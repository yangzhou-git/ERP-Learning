using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #region 登录LoginAsync
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByUserNameAsync(request.UserName);

            if (user == null)
            {
                return ApiResponse<LoginResponse>.Error("用户名不存在");
            }

            if (!user.IsActive)
            {
                return ApiResponse<LoginResponse>.Error("当前账号已被禁用，请联系管理员");
            }

            if (user.Password != request.Password)
            {
                return ApiResponse<LoginResponse>.Error("密码错误");
            }

            var response = new LoginResponse
            {
                UserID = user.UserID,
                UserName = user.UserName,
                CompanyID = user.CompanyID,
                Token = "test-token"
            };

            return ApiResponse<LoginResponse>.Success(response, "登录成功");
        }
        #endregion

        #region 获取员工列表GetUserListAsync
        public async Task<ApiResponse<PageResponse<UserResponse>>> GetUserListAsync(GetUserListRequest request)
        {
            var totalCount = await _userRepository.GetUserCountAsync(request);
            var userList = await _userRepository.GetUserListAsync(request);

            var userResponseList = new List<UserResponse>();

            foreach (var userItem in userList)
            {
                userResponseList.Add(new UserResponse
                {
                    UserID = userItem.UserID,
                    UserName = userItem.UserName,
                    Email = userItem.Email,
                    Phone = userItem.Phone,
                    CompanyID = userItem.CompanyID,
                    IsActive = userItem.IsActive
                });
            }

            var pageData = new PageResponse<UserResponse>
            {
                TotalCount = totalCount,
                List = userResponseList
            };

            return ApiResponse<PageResponse<UserResponse>>.Success(
                pageData,
                "查询员工列表成功"
            );
        }
        #endregion

        #region 新增员工AddUserAsync
        public async Task<ApiResponse<bool>> AddUserAsync(AddUserRequest request)
        {
            var existingUser = await _userRepository.GetUserByUserNameAsync(request.UserName);

            if (existingUser != null)
            {
                return ApiResponse<bool>.Error($"用户名【{request.UserName}】已存在");
            }

            var user = new User
            {
                UserName = request.UserName,
                Password = request.Password,
                Email = request.Email,
                Phone = request.Phone,
                CompanyID = request.CompanyID,
                IsActive = true
            };

            var result = await _userRepository.AddUserAsync(user);

            if (result)
            {
                return ApiResponse<bool>.Success(true, "新增员工成功");
            }

            return ApiResponse<bool>.Error("新增员工失败");
        }
        #endregion

        #region 修改员工资料UpdateUserAsync
        public async Task<ApiResponse<bool>> UpdateUserAsync(UpdateUserRequest request)
        {
            var existingUser = await _userRepository.GetUserByUserNameAsync(request.UserName);

            if (existingUser != null && existingUser.UserID != request.UserID)
            {
                return ApiResponse<bool>.Error($"用户名【{request.UserName}】已存在");
            }

            var user = new User
            {
                UserID = request.UserID,
                UserName = request.UserName,
                Email = request.Email,
                Phone = request.Phone,
                CompanyID = request.CompanyID
            };

            var result = await _userRepository.UpdateUserAsync(user);

            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改员工成功");
            }

            return ApiResponse<bool>.Error("修改员工失败");
        }
        #endregion

        #region 修改员工状态UpdateUserStatusAsync
        public async Task<ApiResponse<bool>> UpdateUserStatusAsync(UpdateUserStatusRequest request)
        {
            var result = await _userRepository.UpdateUserStatusAsync(
                request.UserID,
                request.IsActive
            );

            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改员工状态成功");
            }

            return ApiResponse<bool>.Error("修改员工状态失败");
        }
        #endregion
    }
}