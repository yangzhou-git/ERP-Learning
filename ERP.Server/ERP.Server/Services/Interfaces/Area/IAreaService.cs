using ERP.Server.Common;
using ERP.Server.Models.Response;

namespace ERP.Server.Services.Interfaces
{ 
    public interface IAreaService
    {
        #region 查询地区树（懒加载）GetAreaTreeAsync
        Task<ApiResponse<List<AreaTreeResponse>>> GetAreaTreeAsync(int? ParentID);
        #endregion
    }
}
