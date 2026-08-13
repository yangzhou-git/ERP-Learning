using ERP.Server.Models.Entity;

namespace ERP.Server.Repositories.Interfaces
{
    public interface IAreaRepository
    {
        #region 查询地区树(懒加载)GetAreaTreeAsync
        Task<List<Area>> GetAreaTreeAsync(int? ParentID);
        #endregion
    }
}
