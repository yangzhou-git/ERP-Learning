using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response.Unit;

namespace ERP.Server.Repositories.Interfaces
{
    public interface IUnitRepository
    {
        #region 根据单位名称查询GetUnitByNameAsync
        Task<Unit?> GetUnitByNameAsync(string unitName);
        #endregion
        #region 根据单位编码查询GetUnitByCodeAsync
        Task<Unit?> GetUnitByCodeAsync(string unitCode);
        #endregion
        #region 查询单位下拉选项GetUnitOptionsAsync
        Task<List<UnitOptionResponse>> GetUnitOptionsAsync();
        #endregion
        #region 查询单位列表GetUnitListAsync
        Task<List<Unit>> GetUnitListAsync(GetUnitListRequest request);
        #endregion
        #region 查询单位数量GetUnitCountAsync
        Task<int> GetUnitCountAsync(GetUnitListRequest request);
        #endregion
        #region 新增单位AddUnitAsync
        Task<bool> AddUnitAsync(Unit unit);
        #endregion
        #region 根据单位ID查询GetUnitByIdAsync
        Task<Unit?> GetUnitByIdAsync(int unitID);
        #endregion
        #region 修改单位UpdateUnitAsync
        Task<bool> UpdateUnitAsync(Unit unit);
        #endregion
        #region 修改单位状态UpdateUnitStatusAsync
        Task<bool> UpdateUnitStatusAsync(Unit unit);
        #endregion
    }
}
