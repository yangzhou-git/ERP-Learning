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
        
        #region 登陆LoginAsync
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest Request)
        {
            var User = await _userRepository.GetUserByUserNameAsync(Request.UserName);
            if (User == null)
            {
                return ApiResponse<LoginResponse>.Error("用户名不存在");
            }
            if (User.Password != Request.Password)
            {
                return ApiResponse<LoginResponse>.Error("密码错误");
            }

            var response = new LoginResponse
            {
                UserID = User.UserID,
                UserName = User.UserName,
                CompanyID = User.CompanyID,
                Token = "test-token"
            };
            return ApiResponse<LoginResponse>.Success(response, "登录成功");
        }
        #endregion

        # region 获取用户列表GetUserListAsync
        public async Task<ApiResponse<PageResponse<UserResponse>>> GetUserListAsync(GetUserListRequest Request)
        {
            int TotalCount = await _userRepository.GetUserCountAsync(Request);
            List<User> UserList = await _userRepository.GetUserListAsync(Request);
            List<UserResponse> UserResponseList = new();
            foreach (User UserItem in UserList)
            {
                UserResponseList.Add(new UserResponse
                {
                    UserID = UserItem.UserID,
                    UserName = UserItem.UserName,
                    Email = UserItem.Email,
                    Phone = UserItem.Phone,
                    CompanyID = UserItem.CompanyID,
                    IsActive = UserItem.IsActive
                });
            }
            PageResponse<UserResponse> PageData = new()
            {
                TotalCount = TotalCount,
                List = UserResponseList
            };
            return ApiResponse<PageResponse<UserResponse>>.Success(PageData, "查询成功");
        }
        #endregion

        #region 添加用户AddUserAsync
        public async Task<ApiResponse<bool>> AddUserAsync(AddUserRequest Request)
        {
            var existingUser = await _userRepository.GetUserByUserNameAsync(Request.UserName);
            if (existingUser != null)
            {
                return ApiResponse<bool>.Error("用户名已经存在");
            }
            var user = new User
            {
                UserName = Request.UserName,
                Phone = Request.Phone,
                Password = Request.Password,
                CompanyID= Request.CompanyID,
                IsActive = true
            };
            var Result = await _userRepository.AddUserAsync(user);


            if (Result)
            {
                return ApiResponse<bool>.Success(true, "新增成功");
            }


            return ApiResponse<bool>.Error("新增失败");
        }
        #endregion

        #region 修改员工资料UpdateUserAsync
        public async Task<ApiResponse<bool>> UpdateUserAsync(UpdateUserRequest Request)
        {
            var User = new User
            {
                UserID = Request.UserID,
                UserName = Request.UserName,
                Email = Request.Email,
                Phone = Request.Phone,
                CompanyID = Request.CompanyID
            };
            var Result = await _userRepository.UpdateUserAsync(User);
            if (Result)
            {
                return ApiResponse<bool>.Success(true, "修改成功");
            }
            return ApiResponse<bool>.Error("修改失败");
        }
        #endregion

        #region 修改员工状态UpdateUserStatusAsync
        public async Task<ApiResponse<bool>> UpdateUserStatusAsync(UpdateUserStatusRequest Request)
        {
            var UserStatus = await _userRepository.UpdateUserStatusAsync(Request.UserID, Request.IsActive);
            if (UserStatus)
            {
                return ApiResponse<bool>.Success(true, "修改成功");
            }
            return ApiResponse<bool>.Error("修改失败");
        }
        #endregion
    }
}
