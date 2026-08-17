using System.Data;
using Dapper;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Repositories.Interfaces;

namespace ERP.Server.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        #region 登录根据用户名获取用户信息GetUserByUserNameAsync
        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            var sql = @"select *
                        from Users
                        where UserName = @UserName";

            var user = await _connection.QueryFirstOrDefaultAsync<User>(sql, new
            {
                UserName = userName
            });

            return user;
        }
        #endregion

        #region 获取员工总数量GetUserCountAsync
        public async Task<int> GetUserCountAsync(GetUserListRequest request)
        {
            var sql = @"select count(1)
                        from Users
                        where (@UserName is null
                               or @UserName = ''
                               or UserName like '%' + @UserName + '%')";

            var userCount = await _connection.ExecuteScalarAsync<int>(sql, new
            {
                UserName = request.UserName
            });

            return userCount;
        }
        #endregion

        #region 获取员工列表GetUserListAsync
        public async Task<List<User>> GetUserListAsync(GetUserListRequest request)
        {
            var skip = (request.Page - 1) * request.PageSize;

            var sql = @"select *
                        from Users
                        where (@UserName is null
                               or @UserName = ''
                               or UserName like '%' + @UserName + '%')
                        order by UserID
                        offset @Skip rows
                        fetch next @PageSize rows only";

            var userList = await _connection.QueryAsync<User>(sql, new
            {
                UserName = request.UserName,
                Skip = skip,
                PageSize = request.PageSize
            });

            return userList.ToList();
        }
        #endregion

        #region 新增员工AddUserAsync
        public async Task<bool> AddUserAsync(User user)
        {
            var sql = @"insert into Users(UserName,Password,Email,Phone,CompanyID,IsActive,CreateTime)
                        values(@UserName,@Password,@Email,@Phone,@CompanyID,@IsActive,getdate())";

            var result = await _connection.ExecuteAsync(sql, user);

            return result > 0;
        }
        #endregion

        #region 修改员工资料UpdateUserAsync
        public async Task<bool> UpdateUserAsync(User user)
        {
            var sql = @"update Users
                        set UserName = @UserName,
                            Phone = @Phone,
                            Email = @Email,
                            CompanyID = @CompanyID,
                            UpdateTime = getdate()
                        where UserID = @UserID";

            var result = await _connection.ExecuteAsync(sql, user);

            return result > 0;
        }
        #endregion

        #region 修改员工状态UpdateUserStatusAsync
        public async Task<bool> UpdateUserStatusAsync(int userID, bool isActive)
        {
            var sql = @"update Users
                        set IsActive = @IsActive,
                            UpdateTime = getdate()
                        where UserID = @UserID";

            var result = await _connection.ExecuteAsync(sql, new
            {
                UserID = userID,
                IsActive = isActive
            });

            return result > 0;
        }
        #endregion
    }
}