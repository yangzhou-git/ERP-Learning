using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;

namespace ERP.Server.Repositories.Interfaces
{
    public interface IUserRepository
    {
        #region 根据用户名获取用户信息GetUserByUserNameAsync
        Task<User?> GetUserByUserNameAsync(string userName);
        #endregion
        #region 获取员工总数GetUserCountAsync
        Task<int> GetUserCountAsync(GetUserListRequest request);
        #endregion
        #region 分页查询员工GetUserListAsync
        Task<List<User>> GetUserListAsync(GetUserListRequest request);
        #endregion
        #region 新增员工资料AddUserAsync
        Task<bool> AddUserAsync(User user);
        #endregion
        #region 修改员工资料UpdateUserAsync
        Task<bool> UpdateUserAsync(User user);
        #endregion
        #region 修改员工状态UpdateUserStatusAsync
        Task<bool> UpdateUserStatusAsync(int userID, bool isActive);
        #endregion
    }
}