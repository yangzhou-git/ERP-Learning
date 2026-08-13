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
        #region 登陆根据用户名获取用户信息
        public async Task<User?> GetUserByUserNameAsync(string UserName)
        {
            var Sql = @"select * from Users where UserName =@UserName";
            var User = await _connection.QueryFirstOrDefaultAsync<User>(Sql,new
                {
                    UserName = UserName
                });
            return User;
        }
        #endregion

        #region 获取员工总数量GetUserCountAsync
        public async Task<int> GetUserCountAsync(GetUserListRequest Request)
        {
           var Sql = @"select count(1) from Users where(@UserName IS NULL 
                                                        OR @UserName = ''
                                                        OR UserName LIKE '%' + @UserName + '%'
                                                        )";
            var UserCount = await _connection.ExecuteScalarAsync<int>(Sql, new
            {
                UserName = Request.UserName
            });
            return UserCount;
        }
        #endregion

        #region 获取员工列表GetUserListAsync
        public async Task<List<User>> GetUserListAsync(GetUserListRequest Request)
        {
            int Skip = (Request.Page - 1) * Request.PageSize;
            string Sql = @"SELECT * FROM Users WHERE(@UserName IS NULL
                                                     OR @UserName = ''
                                                     OR UserName LIKE '%' + @UserName + '%'
                                                    )
                                                    ORDER BY UserID
                                                    OFFSET @Skip ROWS
                                                    FETCH NEXT @PageSize ROWS ONLY ";
            var UserList = await _connection.QueryAsync<User>(Sql, new
            {
                UserName = Request.UserName,
                Skip = Skip,
                PageSize = Request.PageSize
            });
            return UserList.ToList();
        }
        #endregion
        #region 新增员工AddUserAsync
        public async Task<bool> AddUserAsync(User user)
        {
            var Sql = @"insert into Users(UserName,Password,Email,Phone,CompanyID,IsActive,CreateTime) values(@UserName,@Password,@Email,@Phone,@CompanyID,@IsActive,GETDATE())";
            var Result = await _connection.ExecuteAsync(Sql, user);
            return Result > 0;
        }
        #endregion

        #region 修改员工资料UpdateUserAsync
        public async Task<bool> UpdateUserAsync(User user)
        {
            var Sql = @"update Users set UserName = @UserName, Phone=@Phone, Email=@Email, CompanyID=@CompanyID, UpdateTime=GETDATE() where UserID=@UserID";
            var Result = await _connection.ExecuteAsync(Sql, user);
            return Result > 0;
        }
        #endregion

        #region 修改员工状态UpdateUserStatusAsync
        public async Task<bool> UpdateUserStatusAsync(int UserID, bool IsActive)
        {
            var Sql = @"update Users set IsActive = @IsActive where UserID = @UserID";
            var Result = await _connection.ExecuteAsync(Sql, new
            {
                UserID = UserID,
                IsActive = IsActive
            });
            return Result > 0;
        }
        #endregion
    }
}
