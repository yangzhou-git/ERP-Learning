using ERP.Server.Common;
using ERP.Server.Models.Request;

namespace ERP.Server.Services.Interfaces
{
    public interface IUnitService
    {
        #region 新增单位AddUnitAsync
        Task<ApiResponse<bool>> AddUnitAsync(AddUnitRequest request);
        #endregion
    }
}
