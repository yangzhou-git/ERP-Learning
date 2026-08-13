using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;

namespace ERP.Server.Repositories.Interfaces
{
    public interface IUserRepository
    {
        #region 登录根据用户名获取用户信息GetUserByUserNameAsync
        Task<User?> GetUserByUserNameAsync(string UserName);
        #endregion
        #region 获取员工总数 GetUserCountAsync
        Task<int> GetUserCountAsync(GetUserListRequest Request);
        #endregion
        #region 分页查询员工 GetUserListAsync
        Task<List<User>> GetUserListAsync(GetUserListRequest Request);
        #endregion
        #region 新增员工资料AddUserAsync
        Task<bool> AddUserAsync(User User);
        #endregion
        #region 修改员工资料UpdateUserAsync
        Task<bool> UpdateUserAsync(User User);
        #endregion
        #region 修改员工状态 UpdateUserStatusAsync
        Task<bool> UpdateUserStatusAsync(int UserID, bool IsActive);
        #endregion
    }
}
